using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackProfile", menuName = "Scriptable Objects/Enemy Attack Profile", order = int.MaxValue)]
public class EnemyAttackProfile : ScriptableObject
{
    [SerializeField]
    private float windupTime = 0.85f;

    [SerializeField]
    private float activeTime = 0.1f;

    [SerializeField]
    private float recoveryTime = 0.6f;

    [SerializeField]
    private bool interruptibleDuringWindup = true;

    [SerializeField]
    private bool interruptibleDuringActive;

    [SerializeField]
    private bool interruptibleDuringRecovery;

    [SerializeField]
    private float hitStunDurationOnInterrupt = 0.15f;

    [SerializeField]
    private float attackRetryDelayAfterInterrupt = 0.6f;

    [SerializeField]
    private bool hasSuperArmor;

    [SerializeField]
    private float maxPoise = 1f;

    [SerializeField]
    private float poiseDamagePerHit = 1f;

    [SerializeField]
    private float poiseRecoveryDelay = 1.5f;

    [SerializeField]
    private float poiseRecoveryPerSecond = 1f;

    public EnemyAttackTiming Timing
    {
        get
        {
            return new EnemyAttackTiming(
                windupTime,
                activeTime,
                recoveryTime,
                interruptibleDuringWindup,
                interruptibleDuringActive,
                interruptibleDuringRecovery);
        }
    }

    public float HitStunDurationOnInterrupt { get { return Mathf.Max(0f, hitStunDurationOnInterrupt); } }
    public float AttackRetryDelayAfterInterrupt { get { return Mathf.Max(0f, attackRetryDelayAfterInterrupt); } }
    public bool HasSuperArmor { get { return hasSuperArmor; } }
    public float MaxPoise { get { return Mathf.Max(1f, maxPoise); } }
    public float PoiseDamagePerHit { get { return Mathf.Max(0f, poiseDamagePerHit); } }
    public float PoiseRecoveryDelay { get { return Mathf.Max(0f, poiseRecoveryDelay); } }
    public float PoiseRecoveryPerSecond { get { return Mathf.Max(0f, poiseRecoveryPerSecond); } }
}
