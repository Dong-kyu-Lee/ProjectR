using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BartenderControllerV2 : PlayerControllerBase
{
    public GameObject projectilePrefab;
    [SerializeField] private BartenderTrajectoryPreview trajectoryPreview;

    private BartenderAbilityV2 bartenderAbility;
    private ProjectileV2 projectileSettings;
    private readonly BartenderThrowInput throwInput = new BartenderThrowInput();

    protected override void Start()
    {
        base.Start();
        bartenderAbility = GetComponent<BartenderAbilityV2>();
        characterAbility = bartenderAbility;
        if (trajectoryPreview == null)
            trajectoryPreview = GetComponent<BartenderTrajectoryPreview>();
    }

    // 다른 캐릭터의 즉시 공격 입력은 그대로 두고 바텐더만 놓을 때 투척한다.
    protected override void HandleAttackInput(bool isUIActive)
    {
        // UI 열기/일시정지와 같은 프레임에 놓아도 발사되지 않도록
        // 다른 Update 처리가 끝난 LateUpdate에서 입력을 확정한다.
    }

    private void UpdateThrowInput(bool isUIActive)
    {
        bool pressed = Input.GetMouseButtonDown(0);
        bool held = Input.GetMouseButton(0);
        bool released = Input.GetMouseButtonUp(0);
        bool canAim = CanAim(isUIActive);

        // UI 위에서 시작한 클릭은 UI를 벗어나도 공격으로 이어지지 않는다.
        if (pressed && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            canAim = false;

        Vector2 direction = Vector2.zero;
        if (canAim && (pressed || held || released || throwInput.IsAiming))
            canAim = TryGetThrowDirection(out direction);

        bool shouldThrow = throwInput.Update(pressed, held, released, canAim);
        if (!throwInput.IsAiming && trajectoryPreview != null)
            trajectoryPreview.Hide();

        if (shouldThrow)
            StartAimedAttack(direction);
    }

    private void LateUpdate()
    {
        // base.Update가 일시정지/사망으로 반환하거나 UI가 나중에 열린 프레임도 처리한다.
        bool isUIActive = InGameUIManager.Instance != null && InGameUIManager.Instance.IsUIActive;
        if (!CanAim(isUIActive))
        {
            CancelAim();
            return;
        }

        UpdateThrowInput(isUIActive);
        if (!throwInput.IsAiming) return;
        if (!TryGetThrowDirection(out Vector2 direction))
        {
            CancelAim();
            return;
        }

        if (direction.x != 0f) Flip(direction.x);
        if (trajectoryPreview != null)
        {
            trajectoryPreview.Show(GetProjectileSettings(), GetThrowOrigin(direction),
                direction, HasSecondThrow(), GetSprite());
        }
    }

    private bool CanAim(bool isUIActive)
    {
        return enableAttack && !isPause && !isDead && !isDashing && !isUIActive
            && Time.timeScale > 0f && GetProjectileSettings() != null;
    }

    private ProjectileV2 GetProjectileSettings()
    {
        if (projectilePrefab == null) return null;
        if (projectileSettings == null || projectileSettings.gameObject != projectilePrefab)
            projectileSettings = projectilePrefab.GetComponent<ProjectileV2>();
        return projectileSettings != null && projectileSettings.projectileRigidbody != null
            ? projectileSettings : null;
    }

    private bool TryGetThrowDirection(out Vector2 direction)
    {
        direction = Vector2.zero;
        Camera cam = GetActiveCamera();
        if (cam == null) return false;

        // 마우스 위치를 플레이어가 있는 XY 평면으로 변환한다.
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, transform.position);
        if (!plane.Raycast(ray, out float distance)) return false;

        direction = (Vector2)(ray.GetPoint(distance) - transform.position);
        if (direction.sqrMagnitude < 0.0001f)
        {
            SpriteRenderer sprite = GetSprite();
            direction = sprite != null && sprite.flipX ? Vector2.right : Vector2.left;
        }
        direction.Normalize();
        return true;
    }

    private Vector3 GetThrowOrigin(Vector2 direction)
    {
        return transform.position + (Vector3)direction * projectileSpawnOffset;
    }

    private bool HasSecondThrow()
    {
        return AbilityManager.Instance != null && AbilityManager.Instance.bartenderAbility != null
            && AbilityManager.Instance.bartenderAbility.Length > 0
            && AbilityManager.Instance.bartenderAbility[0];
    }

    protected override IEnumerator Attack()
    {
        if (GetProjectileSettings() == null || playerStatus == null) yield break;

        enableAttack = false;
        isAttaking = true;
        Vector2 direction = ConsumeQueuedAimDirection();
        Vector3 origin = GetThrowOrigin(direction);

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Attack");
        SoundManager.Instance.Play("Sounds/battle/swing2", Sound.AttackEffect, 0.7f);

        LaunchBottle(origin, direction);
        if (HasSecondThrow())
            LaunchBottle(origin, ProjectileV2.GetSecondThrowDirection(direction));

        // 공격 속도가 높아져도 음수 대기 시간이 생기지 않도록 한다.
        float interval = 1f / Mathf.Max(0.01f, playerStatus.TotalAttackSpeed);
        float animationLock = Mathf.Clamp(attackCoolTimeA, 0f, interval);
        yield return new WaitForSeconds(animationLock);
        isAttaking = false;
        if (interval > animationLock)
            yield return new WaitForSeconds(interval - animationLock);
        enableAttack = true;
    }

    private void LaunchBottle(Vector3 origin, Vector2 direction)
    {
        ProjectileV2 projectile = Instantiate(GetProjectileSettings(), origin, Quaternion.identity);
        projectile.playerStatus = playerStatus;
        projectile.player = gameObject;
        projectile.bartenderAbility = bartenderAbility;
        projectile.Velocity = direction;

        // 조준에는 병을 소모하지 않고 실제 생성된 병마다 한 번만 소모한다.
        if (bartenderAbility != null)
            projectile.bottle = bartenderAbility.UseBartenderBottle();
    }

    private void CancelAim()
    {
        throwInput.Cancel();
        if (trajectoryPreview != null) trajectoryPreview.Hide();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    protected override void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        CancelAim();
        base.OnDisable();
    }

    public override void ResetPlayerState()
    {
        CancelAim();
        base.ResetPlayerState();
    }

    public override void Dead()
    {
        CancelAim();
        base.Dead();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) CancelAim();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) CancelAim();
    }

    private void OnActiveSceneChanged(Scene previous, Scene current)
    {
        CancelAim();
    }
}
