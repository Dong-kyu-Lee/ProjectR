using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static BuffFactory instance;

    public static BuffFactory Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject BuffFactoryObj = new GameObject("BuffFactory");
                instance = BuffFactoryObj.AddComponent<BuffFactory>();
                DontDestroyOnLoad(BuffFactoryObj);
            }
            return instance;
        }
    }
    // 버프 타입에 따른 버프 생성 함수를 저장하는 딕셔너리
    // key: BuffType (열거형) value: 함수로, 매개변수(지속시간, 적용할 대상 GameObject)를 받아서 Buff 객체를 반환
    private Dictionary<BuffType, Func<float, GameObject, Buff>> buffConstructors;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeBuffConstructors();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeBuffConstructors()
    {
        // 새로운 딕셔너리를 생성
        buffConstructors = new Dictionary<BuffType, Func<float, GameObject, Buff>>();
       
        buffConstructors.Add(BuffType.AttackDamageIncrease, (duration, target) =>
        {
            return new AtkDmgIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.AttackSpeedIncrease, (duration, target) =>
        {
            return new AttackSpeedIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Bless, (duration, target) =>
        {
            return new BlessBuff(duration, target);
        });
        buffConstructors.Add(BuffType.BulkUp, (duration, target) =>
        {
            return new BulkUpBuff(duration, target);
        });
        buffConstructors.Add(BuffType.CritDamageIncrease, (duration, target) =>
        {
            return new CritDmgIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.CritPercentIncrease, (duration, target) =>
        {
            return new CritPercentIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.DamageReductionIncrease, (duration, target) =>
        {
            return new DmgReductIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.EagleEye, (duration, target) =>
        {
            return new EagleEyeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.ExtremeSpeed, (duration, target) =>
        {
            return new ExtremeSpeedBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Invincible, (duration, target) =>
        {
            return new InvincibleBuff(duration, target);
        });
        buffConstructors.Add(BuffType.IronBody, (duration, target) =>
        {
            return new IronBodyBuff(duration, target);
        });
        buffConstructors.Add(BuffType.MoveSpeedIncrease, (duration, target) =>
        {
            return new MoveSpeedIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.PriceAdditionalIncrease, (duration, target) =>
        {
            return new PriceAdditionalIncBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Raging, (duration, target) =>
        {
            return new RagingBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Reflection, (duration, target) =>
        {
            return new ReflectionBuff(duration, target);
        });

        buffConstructors.Add(BuffType.Bleeding, (duration, target) =>
        {
            return new BleedingDebuff(duration, target);
        });
        buffConstructors.Add(BuffType.Confusion, (duration, target) =>
        {
            return new ConfusionDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Buzzed, (duration, target) =>
        {
            return new BuzzedDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Drunken, (duration, target) =>
        {
            return new DrunkenDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Burn, (duration, target) =>
        {
            return new BurnDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Freeze, (duration, target) =>
        {
            return new FreezeDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Poison, (duration, target) =>
        {
            return new PoisonDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Sleep, (duration, target) =>
        {
            return new SleepDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Slow, (duration, target) =>
        {
            return new SlowDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.StoneCurse, (duration, target) =>
        {
            return new StoneCurseDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Stun, (duration, target) =>
        {
            return new StunDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Curse, (duration, target) =>
        {
            return new CurseDeBuff(duration, target);
        });
        buffConstructors.Add(BuffType.Force7, (duration, target) =>
        {
            return new Force7Buff(duration, target);
        });
        buffConstructors.Add(BuffType.Force16, (duration, target) =>
        {
            return new Force16Buff(duration, target);
        });
        buffConstructors.Add(BuffType.Dexterity7, (duration, target) =>
        {
            return new Dexterity7Buff(duration, target);
        });
        buffConstructors.Add(BuffType.Dexterity13, (duration, target) =>
        {
            return new Dexterity13Buff(duration, target);
        });
        buffConstructors.Add(BuffType.Critical4, (duration, target) =>
        {
            return new Critical4Buff(duration, target);
        });
        buffConstructors.Add(BuffType.Critical7, (duration, target) =>
        {
            return new Critical7Buff(duration, target);
        });


        Debug.Log("BuffFactory: 모든 버프 생성 함수 초기화 완료.");
    }

    // 지정된 버프 타입에 따라 새로운 Buff 객체를 생성하는 메서드

    public Buff CreateBuff(BuffType type, float duration, GameObject target)
    {
        if (buffConstructors.TryGetValue(type, out Func<float, GameObject, Buff> constructor))
        {
            try
            {
                Buff newBuff = constructor(duration, target);
                Debug.Log($"BuffFactory: 버프 [{type}] 생성 완료, 지속 시간: {duration}");
                return newBuff;
            }
            catch (Exception e)
            {
                Debug.LogError($"BuffFactory: 버프 생성 중 오류 발생 [{type}] - {e.Message}");
                throw;
            }
        }
        else
        {
            Debug.LogError($"BuffFactory: [{type}] 버프 생성 실패.");
            throw new ArgumentException($"BuffFactory: '{type}'에 대한 생성자가 없음");
        }
    }
}