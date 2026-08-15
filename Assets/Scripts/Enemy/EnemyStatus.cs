using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStatus : Status
{
    [SerializeField]
    private EnemyData enemyData;
    private EnemyLoot enemyLoot;

    [SerializeField]
    private bool isBoss;

    [SerializeField]
    private Color hitFlashColor = Color.red;

    [SerializeField]
    private float hitFlashDuration = 0.08f;

    [SerializeField]
    private int hitFlashCount = 2;

    [SerializeField]
    private float hitFlashInterval = 0.05f;

    public bool IsBoss => isBoss;

    public EnemyData EnemyStatusData { get { return enemyData; } }

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine hitFlashCoroutine;

    void Awake()
    {
        // 스테이터스 데이터 동기화. 추후 scriptableObject 접근으로 변경 예정.
        MaxHp = enemyData.Hp;
        Hp = MaxHp;
        Damage = enemyData.Damage;
        DamageReduction = enemyData.DamageReduction;
        AttackSpeed = enemyData.AttackSpeed;
        MoveSpeed = enemyData.MoveSpeed;
        AdditionalAttackSpeed = 0;
        AdditionalMoveSpeed = 0;
        enemyLoot = transform.GetComponent<EnemyLoot>();
        CacheSpriteRenderers();
    }

    void Start()
    {
        
    }

    protected override void HitImpact()
    {
        PlayHitFlash();

        if (isBoss)
        {
            Vector2 spawnPosition = GetComponent<CapsuleCollider2D>().bounds.min;
            RuneSpawner.Instance.TrySpawnRune(spawnPosition + Vector2.up);
            return;
        }

        GetComponent<Enemy>()?.TryInterruptAttack();
    }

    private void CacheSpriteRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void PlayHitFlash()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0) return;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
            RestoreOriginalColors();
        }

        hitFlashCoroutine = StartCoroutine(HitFlashCoroutine());
    }

    private IEnumerator HitFlashCoroutine()
    {
        int repeatCount = Mathf.Max(1, hitFlashCount);

        for (int i = 0; i < repeatCount; i++)
        {
            SetSpriteColors(hitFlashColor);
            yield return new WaitForSeconds(hitFlashDuration);

            RestoreOriginalColors();

            if (i < repeatCount - 1 && hitFlashInterval > 0f)
            {
                yield return new WaitForSeconds(hitFlashInterval);
            }
        }

        hitFlashCoroutine = null;
    }

    private void SetSpriteColors(Color color)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = color;
            }
        }
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = originalColors[i];
            }
        }
    }

    protected override void Dead()
    {
        if (isDead) return;
        isDead = true;

        // 물리 연산 제외 (적 타격 불가능)
        GetComponent<Collider2D>().enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;   
        }

        EnemyAIController enemyAIController = GetComponent<Enemy>().StateMachine;
        LevelUp.Instance?.IncreaseExp(enemyData.ExpValue);
        AbilityManager.Instance?.IncreaseSoulShard(enemyData.SoulShardValue);
        enemyLoot.DropLoot();
        Vector2 spawnPosition = GetComponent<CapsuleCollider2D>().bounds.min;
        RuneSpawner.Instance.TrySpawnRune(spawnPosition + Vector2.up);
        enemyAIController.TransitionTo(enemyAIController.deadState);
    }
}
