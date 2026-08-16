using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackPhase
{
    None,
    Windup,
    Active,
    Recovery
}

public struct EnemyAttackTiming
{
    public static readonly EnemyAttackTiming Immediate = new EnemyAttackTiming(0f, 0f, 0f, false, false, false);

    public float windupTime;
    public float activeTime;
    public float recoveryTime;
    public bool interruptibleDuringWindup;
    public bool interruptibleDuringActive;
    public bool interruptibleDuringRecovery;

    public EnemyAttackTiming(
        float windupTime,
        float activeTime,
        float recoveryTime,
        bool interruptibleDuringWindup,
        bool interruptibleDuringActive,
        bool interruptibleDuringRecovery)
    {
        this.windupTime = Mathf.Max(0f, windupTime);
        this.activeTime = Mathf.Max(0f, activeTime);
        this.recoveryTime = Mathf.Max(0f, recoveryTime);
        this.interruptibleDuringWindup = interruptibleDuringWindup;
        this.interruptibleDuringActive = interruptibleDuringActive;
        this.interruptibleDuringRecovery = interruptibleDuringRecovery;
    }

    public float TotalTime
    {
        get { return windupTime + activeTime + recoveryTime; }
    }

    public EnemyAttackTiming WithMinimumTotalTime(float minimumTotalTime)
    {
        EnemyAttackTiming timing = this;
        float remainingTime = minimumTotalTime - TotalTime;
        if (remainingTime > 0f)
        {
            timing.recoveryTime += remainingTime;
        }

        return timing;
    }
}

public interface IAttackStrategy
{
    EnemyAttackTiming Timing { get; }
    void BeginAttack(Enemy enemy);
    void ExecuteAttack(Enemy enemy);
}


public class AttackStrategy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
