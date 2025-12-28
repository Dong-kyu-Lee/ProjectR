using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroAttackPattern
{
    ComboAttack,   // 1페이즈: 2연격
    DashAttack,    // 2페이즈: 돌진 찌르기
    BladeDance     // 3페이즈: 난무
}

public class HeroBossEnemy : Enemy
{
    [Header("--- Hero Boss Settings ---")]
    [SerializeField] private GameObject hitBoxObj; // 공격 판정 박스
    [SerializeField] private float hitBoxDistance = 1.0f; // 히트박스가 생성될 전방 거리

    [Header("Collision Settings")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Combat Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float detectRange = 4.0f; // 공격 사거리

    private PhaseType currentPhase = PhaseType.Phase1;
    private HeroAttackPattern currentPattern = HeroAttackPattern.ComboAttack;

    private float lastAttackTime = 0f;
    private bool isPatternRunning = false;

    protected override void Awake()
    {
        base.Awake();

        deadDelayTime = 3f;

        // 초기 전략 설정 (HeroProxyStrategy를 통해 RunCurrentStrategy 호출)
        SetAttackStrategy(new HeroProxyStrategy());

        // HitBox 초기화
        if (hitBoxObj != null) hitBoxObj.SetActive(false);
    }

    void Start()
    {
        if (StateMachine != null) StateMachine.Initialize(this);
    }

    protected void Update()
    {
        // 부모(Enemy)의 Update (FlipX 등) 수행
        // 단, 공격 중(isPatternRunning)일 때는 방향 전환을 막고 싶다면 조건 추가 가능
        if (!isPatternRunning)
        {
            base.FixedUpdate(); // FlipX가 여기 들어있음
        }

        CheckPhaseProgress();

        // 애니메이션 파라미터 동기화
        if (enemyAnimator != null)
        {
            bool isMoving = StateMachine.CurrentState == StateMachine.chaseState;
            enemyAnimator.SetBool("isMove", isMoving);
        }
    }

    // 페이즈 관리
    private void CheckPhaseProgress()
    {
        if (EnemyStatus == null) return;
        float hpRatio = EnemyStatus.Hp / EnemyStatus.MaxHp;

        if (currentPhase == PhaseType.Phase1 && hpRatio <= 0.6f)
        {
            StartPhase2();
        }
        else if (currentPhase == PhaseType.Phase2 && hpRatio <= 0.3f)
        {
            StartPhase3();
        }
    }

    private void StartPhase2()
    {
        Debug.Log(">>> Hero Phase 2: Pursuit (돌진)");
        currentPhase = PhaseType.Phase2;
        EnemyStatus.MoveSpeed *= 1.3f; // 속도 증가
    }

    private void StartPhase3()
    {
        Debug.Log(">>> Hero Phase 3: Blade Dance (난무)");
        currentPhase = PhaseType.Phase3;
        attackCooldown = 0.5f; // 쿨타임 감소
    }

    // 전략 실행(AIController가 호출)
    public void RunCurrentStrategy()
    {
        if (isPatternRunning) return;

        // 쿨타임 체크
        if (Time.time < lastAttackTime + attackCooldown)
        {
            float dist = Vector2.Distance(transform.position, PlayerTransform.position);
            if (dist <= detectRange) StateMachine.TransitionTo(StateMachine.idleState);
            else StateMachine.TransitionTo(StateMachine.chaseState);
            return;
        }

        lastAttackTime = Time.time;
        SelectPattern(); // 패턴 결정
        ExecutePattern(); // 패턴 실행
    }

    private void SelectPattern()
    {
        // 페이즈별 패턴 확률 로직
        float rand = Random.value;

        switch (currentPhase)
        {
            case PhaseType.Phase1:
                // 2연격
                currentPattern = HeroAttackPattern.ComboAttack;
                break;
            case PhaseType.Phase2:
                // 돌진 비중 높음
                currentPattern = (rand > 0.4f) ? HeroAttackPattern.DashAttack : HeroAttackPattern.ComboAttack;
                break;
            case PhaseType.Phase3:
                // 난무 비중 추가
                if (rand > 0.6f) currentPattern = HeroAttackPattern.BladeDance;
                else if (rand > 0.3f) currentPattern = HeroAttackPattern.DashAttack;
                else currentPattern = HeroAttackPattern.ComboAttack;
                break;
        }
    }

    private void ExecutePattern()
    {
        switch (currentPattern)
        {
            case HeroAttackPattern.ComboAttack:
                StartCoroutine(ComboAttackRoutine());
                break;
            case HeroAttackPattern.DashAttack:
                StartCoroutine(DashAttackRoutine());
                break;
            case HeroAttackPattern.BladeDance:
                StartCoroutine(BladeDanceRoutine());
                break;
        }
    }

    // [패턴 1] 2연격 (전진 베기)
    private IEnumerator ComboAttackRoutine()
    {
        isPatternRunning = true;
        isAttacking = true;

        // 1타
        FacePlayer(); // 공격 전 방향 보정
        enemyAnimator.SetTrigger("Attack1");

        // 전진 효과 (Rigidbody에 힘을 줌)
        Vector2 dir = (transform.rotation.y == 0 ? Vector2.right : Vector2.left);
        enemyRigidbody.AddForce(dir * 3f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f); // 타격 타이밍
        StartCoroutine(ActivateHitBox(0.2f));  // 0.2초간 히트박스 켜기
        yield return new WaitForSeconds(0.4f); // 1타 후딜

        // 2타
        FacePlayer();
        enemyAnimator.SetTrigger("Attack2");
        enemyRigidbody.AddForce(dir * 5f, ForceMode2D.Impulse); // 2타는 더 크게 전진

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ActivateHitBox(0.3f));
        yield return new WaitForSeconds(0.8f); // 전체 후딜

        FinishAttack();
    }

    // [패턴 2] 돌진 찌르기
    private IEnumerator DashAttackRoutine()
    {
        isPatternRunning = true;
        isAttacking = true;

        enemyAnimator.SetTrigger("Roll"); // 구르기/준비 모션
        yield return new WaitForSeconds(0.3f); // 선딜

        // 플레이어 방향으로 고속 이동
        Vector2 dir = (PlayerTransform.position - transform.position).normalized;
        dir.y = 0; // 수평 이동만
        float dashTime = 0.5f;
        float timer = 0;

        while (timer < dashTime)
        {
            enemyRigidbody.velocity = new Vector2(dir.x * dashSpeed, enemyRigidbody.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }
        enemyRigidbody.velocity = Vector2.zero; // 정지

        FacePlayer();
        enemyAnimator.SetTrigger("Attack1");
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ActivateHitBox(0.2f));
        yield return new WaitForSeconds(0.5f);

        FinishAttack();
    }

    // [패턴 3] 난무 (좌우 왕복)
    private IEnumerator BladeDanceRoutine()
    {
        isPatternRunning = true;
        isAttacking = true;

        int count = 3; // 3회 왕복
        string[] triggers = { "Attack1", "Attack2", "Attack3" };
        float baseDashDuration = 0.25f; // 이동 시간
        float maxDashDistance = 8.0f;  // 한 번에 이동할 거리

        float bodyRadius = 0.6f;

        for (int i = 0; i < count; i++)
        {
            // 1. 방향 결정
            Vector2 dir = (PlayerTransform.position - transform.position).normalized;
            if (dir.x == 0) dir.x = (transform.rotation.y == 0 ? 1 : -1);
            dir.y = 0;
            dir.x = Mathf.Sign(dir.x);

            // 2. [핵심 수정] 목표 지점 계산 (Raycast 시작점 오프셋 적용)
            Vector2 startPos = transform.position;

            // 레이 시작점을 몸 중심이 아니라 진행 방향으로 약간 앞(bodyRadius)에서 시작
            Vector2 rayOrigin = startPos + new Vector2(dir.x * bodyRadius, 2.5f);

            float moveDistance = maxDashDistance;

            // 벽 감지
            if (obstacleLayer.value != 0)
            {
                Debug.DrawRay(rayOrigin, dir * maxDashDistance, Color.red, 1.0f);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dir, maxDashDistance, obstacleLayer);

                if (hit.collider != null)
                {
                    // [디버깅용] 레이가 무엇을 때렸는지 콘솔에 출력!
                    // 만약 여기에 "HitBox", "ChaseRange", "HeroBoss" 등이 뜨면 레이어 설정 문제임.
                    Debug.Log($"Raycast Hit: {hit.collider.name} / Dist: {hit.distance}");

                    // 만약 때린 게 '나 자신'이거나 '내 자식 오브젝트'라면 무시해야 함
                    if (hit.collider.transform.IsChildOf(this.transform) || hit.collider.transform == this.transform)
                    {
                        // 레이어가 꼬여서 자기를 때린 경우, 그냥 통과시킴 (최대 거리 이동)
                        moveDistance = maxDashDistance;
                    }
                    else
                    {
                        // 진짜 벽을 만난 경우
                        float distanceToWall = hit.distance;
                        moveDistance = distanceToWall + bodyRadius - 0.8f;
                        if (moveDistance < 0) moveDistance = 0;
                    }
                }
            }

            Vector2 targetPos = startPos + new Vector2(dir.x * moveDistance, 0);

            // 3. 거리 비례 시간 조절 (거리가 짧아져도 속도감 유지)
            // 원래 8만큼 갈 때 0.25초 걸렸다면, 4만큼 갈 땐 0.125초만 걸리게 함
            float actualDuration = baseDashDuration * (moveDistance / maxDashDistance);

            // 너무 순식간에 이동하면 어색하므로 최소 시간(0.1s) 보장
            if (actualDuration < 0.1f) actualDuration = 0.1f;

            // 4. 공격 및 히트박스
            FacePlayer();
            enemyAnimator.SetTrigger(triggers[i % 3]);
            StartCoroutine(ActivateHitBox(actualDuration + 0.1f));

            // 5. Lerp 이동 (고속 이동 복구)
            float elapsed = 0f;
            while (elapsed < actualDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / actualDuration;
                transform.position = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }
            transform.position = targetPos;

            // 다음 공격 전 아주 짧은 대기
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1.0f); // 후딜레이
        FinishAttack();
    }

    private IEnumerator ActivateHitBox(float duration)
    {
        if (hitBoxObj == null) yield break;

        hitBoxObj.transform.localPosition = new Vector2(-hitBoxDistance, 0.5f);

        hitBoxObj.SetActive(true);
        yield return new WaitForSeconds(duration);
        hitBoxObj.SetActive(false);
    }

    private void FinishAttack()
    {
        isAttacking = false;
        isPatternRunning = false;
        StateMachine.TransitionTo(StateMachine.chaseState);
    }
}
