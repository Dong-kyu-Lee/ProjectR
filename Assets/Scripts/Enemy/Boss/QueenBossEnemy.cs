using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여왕 보스 전용 공격 패턴 정의
public enum QueenAttackPattern
{
    SingleLine,  // 1페이즈: 직선 단발
    FanShape,    // 2페이즈: 부채꼴
    RapidHoming  // 3페이즈: 조준 연사
}

public class QueenBossEnemy : Enemy
{
    [Header("Queen Boss Settings")]
    [SerializeField] private GameObject projectilePrefab; // 마법 구체 프리팹
    [SerializeField] private Transform firePoint;         // 발사 위치 (지팡이 끝)

    [Header("Pattern Settings")]
    [SerializeField] private float fanAngle = 20f;        // 부채꼴 각도
    [SerializeField] private float fanSpawnRadius = 6.5f;  // 투사체 생성 반지름 (이격 거리)
    [SerializeField] private int rapidFireCount = 5;      // 연사 횟수
    [SerializeField] private float rapidFireDelay = 0.15f;// 연사 간격

    [SerializeField] private float rapidFireSpreadY = 0.5f;

    private QueenAttackPattern currentPattern = QueenAttackPattern.SingleLine;
    private int currentPhase = 1;

    [Header("Cooldown Settings")]
    [SerializeField] private float attackCooldown = 2.0f; // 공격 후 다음 공격까지 쉴 시간 (이동 시간)
    private float lastAttackTime = 0f;                    // 마지막 공격 시간 저장
    private bool isPatternRunning = false;                // 현재 코루틴이 돌고 있는지 여부

    protected override void Awake()
    {
        base.Awake();
        // 초기 전략: 1페이즈는 여왕 전용 원거리 공격으로 시작
        // (필요 시 MeleeAttackStrategy로 시작했다가 거리가 멀어지면 바꾸는 등 조정 가능)
        SetAttackStrategy(new QueenProjectileStrategy());
    }

    void Start()
    {
        // EnemyAIController 초기화 (부모 클래스의 StateMachine 사용)
        if (StateMachine != null)
        {
            StateMachine.Initialize(this);
        }
    }

    protected void Update()
    {
        // 부모 클래스 Update 실행 (플립 등)
        CheckPhaseProgress();
    }

    // 체력에 따른 페이즈 모니터링
    private void CheckPhaseProgress()
    {
        if (EnemyStatus == null) return;

        float hpRatio = EnemyStatus.Hp / EnemyStatus.MaxHp;

        // 페이즈 2 진입 (체력 70% 이하)
        if (currentPhase == 1 && hpRatio <= 0.7f)
        {
            StartPhase2();
        }
        // 페이즈 3 진입 (체력 40% 이하)
        else if (currentPhase == 2 && hpRatio <= 0.4f)
        {
            StartPhase3();
        }
    }

    private void StartPhase2()
    {
        Debug.Log(">>> Queen Phase 2: Corruption (확산 공격)");
        currentPhase = 2;
        currentPattern = QueenAttackPattern.FanShape;

        // 이동 속도 20% 증가
        EnemyStatus.MoveSpeed *= 1.2f;

        // 전략 교체 (혹시 다른 전략을 쓰고 있었다면 강제로 원거리 전략 할당)
        SetAttackStrategy(new QueenProjectileStrategy());
    }

    private void StartPhase3()
    {
        Debug.Log(">>> Queen Phase 3: Rampage (폭주/연사)");
        currentPhase = 3;
        currentPattern = QueenAttackPattern.RapidHoming;

        // 3페이즈는 근접, 원거리, 디버프를 섞어서 사용 (랜덤 패턴)
        var mixedStrategies = new List<IAttackStrategy>()
        {
            new QueenProjectileStrategy(), // 연사 (가중치 높게 주려면 여러번 추가)
            new QueenProjectileStrategy(),
            new MeleeAttackStrategy(),     // 가까이 오면 지팡이 타격
            new DebuffAttackStrategy()     // 속박
        };

        SetAttackStrategy(new RandomCompositeStrategy(this, mixedStrategies));
    }

    // QueenProjectileStrategy에서 호출하는 실제 공격 함수
    public void CastMagicAttack()
    {
        // 애니메이션 트리거 (Animator에 "CastAttack" 같은 트리거 필요)
        if (enemyAnimator != null) enemyAnimator.SetTrigger("RangedAttack");

        switch (currentPattern)
        {
            case QueenAttackPattern.SingleLine:
                StartCoroutine(ShootSingle());
                break;
            case QueenAttackPattern.FanShape:
                StartCoroutine(ShootFan());
                break;
            case QueenAttackPattern.RapidHoming:
                StartCoroutine(ShootRapid());
                break;
        }
    }

    // 패턴 1: 직선 발사
    private IEnumerator ShootSingle()
    {
        isAttacking = true;
        isAttacking = true;

        yield return new WaitForSeconds(0.3f); // 선딜레이 (모션)

        Vector2 dir = GetTargetDirection();
        CreateProjectile(dir);

        yield return new WaitForSeconds(1.5f); // 후딜레이

        isAttacking = false;
        isPatternRunning = false;
        StateMachine.TransitionTo(StateMachine.chaseState); // 상태 전환
    }

    // 패턴 2: 부채꼴 발사
    private IEnumerator ShootFan()
    {
        isAttacking = true;
        isPatternRunning = true;

        yield return new WaitForSeconds(0.3f); // 선딜레이

        // 중앙 방향 (플레이어 쪽, Y축 평행 보정됨)
        Vector2 centerDir = GetTargetDirection();

        // [1] 중앙 발사
        // 위치: firePoint에서 centerDir 방향으로 radius만큼 떨어진 곳
        Vector3 centerPos = firePoint.position + (Vector3)(centerDir * fanSpawnRadius);
        CreateProjectile(centerPos, centerDir);

        // [2] 위쪽 발사 (+각도)
        Vector2 upperDir = RotateVector(centerDir, fanAngle);
        Vector3 upperPos = firePoint.position + (Vector3)(upperDir * fanSpawnRadius);
        CreateProjectile(upperPos, upperDir); // upperDir 방향으로 날아감

        // [3] 아래쪽 발사 (-각도)
        Vector2 lowerDir = RotateVector(centerDir, -fanAngle);
        Vector3 lowerPos = firePoint.position + (Vector3)(lowerDir * fanSpawnRadius);
        CreateProjectile(lowerPos, lowerDir); // lowerDir 방향으로 날아감

        yield return new WaitForSeconds(1.5f); // 후딜레이

        isAttacking = false;
        isPatternRunning = false;
        StateMachine.TransitionTo(StateMachine.chaseState);
    }

    // 패턴 3: 조준 연사
    private IEnumerator ShootRapid()
    {
        isPatternRunning = true;
        isAttacking = true; // 연사 중 다른 행동 방지

        for (int i = 0; i < rapidFireCount; i++)
        {
            // 1. 발사 방향 계산 (플레이어 쪽, 수평)
            Vector2 dir = GetTargetDirection();

            // 2. 생성 위치에 랜덤 Y 오프셋 적용
            // firePoint 기준 위(+) 아래(-)로 랜덤하게 위치 결정
            float randomY = Random.Range(-rapidFireSpreadY, rapidFireSpreadY);
            Vector3 spawnPos = firePoint.position + new Vector3(0, randomY, 0);

            // 3. 위치를 지정하여 생성하는 함수 호출
            CreateProjectile(spawnPos, dir);

            yield return new WaitForSeconds(rapidFireDelay);
        }

        // 후딜레이 (연사 후 잠시 멈춤)
        yield return new WaitForSeconds(1.5f);

        isAttacking = false;
        isPatternRunning = false;
        // 공격 후 추격 상태로 전환 (EnemyAIController 로직에 따라 자동 전환될 수 있음)
        StateMachine.TransitionTo(StateMachine.chaseState);
    }

    // --- 유틸리티 함수 ---

    // 투사체 생성 로직
    private void CreateProjectile(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // 투사체 방향 회전 (화살표가 진행 방향 보게)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 기존 투사체 스크립트 활용 (데이터 주입)
        RangedEnemyProjectile projScript = proj.GetComponent<RangedEnemyProjectile>();
        if (projScript != null)
        {
            projScript.enemy = this.gameObject; // 공격자 설정
            projScript.damage = EnemyStatus.Damage; // 데미지 설정
        }

        // 물리 속도 부여
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * 5f; // 투사체 속도 (필요시 변수화)
        }
    }

    // 위치 지정 함수 (부채꼴 패턴용)
    private void CreateProjectile(Vector3 spawnPos, Vector2 direction)
    {
        if (projectilePrefab == null) return;

        // 지정된 위치(spawnPos)에 생성
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 데이터 설정
        RangedEnemyProjectile projScript = proj.GetComponent<RangedEnemyProjectile>();
        if (projScript != null)
        {
            projScript.enemy = this.gameObject;
            projScript.damage = EnemyStatus.Damage;
        }

        // 속도 설정
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * 5f; // 속도
        }
    }

    // 플레이어 방향 벡터 계산
    private Vector2 GetTargetDirection()
    {
        if (PlayerTransform == null) 
            return transform.right * (transform.rotation.y == 0 ? 1 : -1);

        Vector2 dir = PlayerTransform.position - firePoint.position;

        dir.y = 0;

        return dir.normalized;
    }

    // 벡터 회전 함수
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;
    }

    public override void PerformAttack()
    {
        //if (isAttacking) return;

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("MeleeAttack");
        }

        // 보통 애니메이션 이벤트로 처리하거나, 여기서 코루틴으로 딜레이 후 처리
        StartCoroutine(MeleeAttackCoroutine());
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f); // 공격 모션 딜레이(타격 시점) 맞춰서

        // 여기에 공격 범위(HitBox) 켜기 or 데미지 주기 로직
        // base.PerformAttack(); // 부모의 기본 로직이 필요하다면 호출

        isAttacking = false;
        // 공격 끝나면 다시 추격 상태로 (AIController가 관리하지 않는다면 수동 전환)
        StateMachine.TransitionTo(StateMachine.chaseState);
    }

    // 전략에서 호출하는 진입점 함수 수정
    public void RunCurrentStrategy()
    {
        // 1. 이미 패턴 코루틴이 실행 중이면 중복 실행 방지
        if (isPatternRunning) return;

        // 2. 쿨타임 체크: 아직 쿨타임이라면 공격하지 않고 추격(이동) 상태로 돌려보냄
        if (Time.time < lastAttackTime + attackCooldown)
        {
            StateMachine.TransitionTo(StateMachine.chaseState);
            return;
        }

        // 3. 쿨타임이 지났다면 공격 실행
        lastAttackTime = Time.time; // 공격 시작 시간 기록

        if (attackStrategy != null)
        {
            attackStrategy.ExecuteAttack(this);
        }
    }
}
