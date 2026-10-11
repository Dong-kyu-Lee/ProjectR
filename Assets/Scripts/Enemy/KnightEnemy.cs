using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemy : Enemy
{
    [SerializeField] private GameObject airAttackHitBox;
    [SerializeField] private GameObject[] comboHitBoxes;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform rendererTransform;
    [SerializeField] private GameObject hpBarCanvas;
    [SerializeField] private float airAttackJumpHeight = 1f; // 상승할 높이

    private Vector3 originalRendererLocalPos;

    void Start()
    {
        StateMachine.Initialize(this);
        attackStrategy = new KnightEnemyStrategy();

        if (rendererTransform != null)
        {
            originalRendererLocalPos = rendererTransform.localPosition;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SetAttackStrategy(new KnightEnemyStrategy());
    }

    void Update()
    {

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    // 렌더러 가시성 설정
    public void SetRendererVisible(bool visible)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }

        if (hpBarCanvas != null)
        {
            hpBarCanvas.SetActive(visible);
        }
    }

    // 공중 공격 렌더러 위치 업데이트
    public void UpdateAirAttackDescend(float t)
    {
        if (rendererTransform == null) return;

        float targetY = Mathf.Lerp(originalRendererLocalPos.y + airAttackJumpHeight, originalRendererLocalPos.y, t);

        rendererTransform.localPosition = new Vector3(originalRendererLocalPos.x, targetY, originalRendererLocalPos.z);
    }

    // 공중 공격 히트박스 활성화
    public void ActivateAirAttackHitBox()
    {
        DisableAllHitBoxes();
        if (airAttackHitBox != null)
        {
            airAttackHitBox.SetActive(true);
        }
    }

    // 특정 인덱스 콤보 공격 히트박스 활성화
    public void ActivateHitBoxForCombo(int comboIndex)
    {
        // 안전한 범위 체크
        if (comboHitBoxes == null || comboIndex < 0 || comboIndex >= comboHitBoxes.Length) return;

        // 다른 모든 히트박스는 우선 끄고 안전하게 시작
        DisableAllHitBoxes();

        if (comboHitBoxes[comboIndex] != null)
        {
            comboHitBoxes[comboIndex].SetActive(true);
        }
    }

    /*public override void PerformAttack()
    {
        base.PerformAttack();
        StartCoroutine(EnableHitbox());
    }

    IEnumerator EnableHitbox()
    {
        yield return null;
        float hitOffsetX = 0.35f;

        if (hitBoxObj == null) yield break;

        hitBoxObj.transform.localPosition = new Vector2(-hitOffsetX, 0.3f);
        hitBoxObj.SetActive(true);
    }*/

    // 공격 중단
    public override void CancelAttack()
    {
        base.CancelAttack();
        DisableAllHitBoxes();
    }

    // 모든 공격 히트박스 비활성화
    private void DisableAllHitBoxes()
    {
        if (airAttackHitBox != null) airAttackHitBox.SetActive(false);
        if (comboHitBoxes != null)
        {
            foreach (var box in comboHitBoxes)
            {
                if (box != null) box.SetActive(false);
            }
        }
    }
}
