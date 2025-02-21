using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private ForceEffect forceEffect;
    private InduranceEffect induranceEffect;
    private CriticalEffect criticalEffect;
    private DexterityEffect dexterityEffect;
    private MysteryEffect mysteryEffect;

    // 컴포넌트 할당.
    private void Start()
    {
        //playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        forceEffect = GetComponent<ForceEffect>();
        induranceEffect = GetComponent<InduranceEffect>();
        criticalEffect = GetComponent<CriticalEffect>();
        dexterityEffect = GetComponent<DexterityEffect>();
        mysteryEffect = GetComponent<MysteryEffect>();
    }

    // 특수 효과 활성화.
    public void EnableEffect(string statName, int point)
    {
        switch (statName)
        {
            case "force":
                switch (point)
                {
                    case 1: forceEffect.EnableForceEffect1(playerStatus); break;
                    case 4: forceEffect.EnableForceEffect4(); break;
                    case 7: forceEffect.EnableForceEffect7(); break;
                    case 10: forceEffect.EnableForceEffect10(playerStatus); break;
                    case 13: forceEffect.EnableForceEffect13(); break;
                    case 16: forceEffect.EnableForceEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "indurance":
                switch (point)
                {
                    case 1: induranceEffect.EnableInduranceEffect1(playerStatus); break;
                    case 4: induranceEffect.EnableInduranceEffect4(); break;
                    case 7: induranceEffect.EnableInduranceEffect7(); break;
                    case 10: induranceEffect.EnableInduranceEffect10(); break;
                    case 13: induranceEffect.EnableInduranceEffect13(); break;
                    case 16: induranceEffect.EnableInduranceEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "critical":
                switch (point)
                {
                    case 1: criticalEffect.EnableCriticalEffect1(playerStatus); break;
                    case 4: criticalEffect.EnableCriticalEffect4(); break;
                    case 7: criticalEffect.EnableCriticalEffect7(); break;
                    case 10: criticalEffect.EnableCriticalEffect10(); break;
                    case 13: criticalEffect.EnableCriticalEffect13(); break;
                    case 16: criticalEffect.EnableCriticalEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "dexterity":
                switch (point)
                {
                    case 1: dexterityEffect.EnableDexterityEffect1(playerStatus); break;
                    case 4: dexterityEffect.EnableDexterityEffect4(); break;
                    case 7: dexterityEffect.EnableDexterityEffect7(); break;
                    case 10: dexterityEffect.EnableDexterityEffect10(); break;
                    case 13: dexterityEffect.EnableDexterityEffect13(); break;
                    case 16: dexterityEffect.EnableDexterityEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "mystery":
                switch (point)
                {
                    case 1: mysteryEffect.EnableMysteryEffect1(); break;
                    case 4: mysteryEffect.EnableMysteryEffect4(); break;
                    case 7: mysteryEffect.EnableMysteryEffect7(); break;
                    case 10: mysteryEffect.EnableMysteryEffect10(); break;
                    case 13: mysteryEffect.EnableMysteryEffect13(); break;
                    case 16: mysteryEffect.EnableMysteryEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            default:
                Debug.Log("잘못된 스테이터스 이름");
                return;
        }
    }

    // 특수 효과 비활성화.
    public void DisableEffect(string statName, int point)
    {
        switch (statName)
        {
            case "force":
                switch (point)
                {
                    case 1: forceEffect.DisableForceEffect1(playerStatus); break;
                    case 4: forceEffect.DisableForceEffect4(); break;
                    case 7: forceEffect.DisableForceEffect7(); break;
                    case 10: forceEffect.DisableForceEffect10(playerStatus); break;
                    case 13: forceEffect.DisableForceEffect13(); break;
                    case 16: forceEffect.DisableForceEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "indurance":
                switch (point)
                {
                    case 1: induranceEffect.DisableInduranceEffect1(playerStatus); break;
                    case 4: induranceEffect.DisableInduranceEffect4(); break;
                    case 7: induranceEffect.DisableInduranceEffect7(); break;
                    case 10: induranceEffect.DisableInduranceEffect10(); break;
                    case 13: induranceEffect.DisableInduranceEffect13(); break;
                    case 16: induranceEffect.DisableInduranceEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "critical":
                switch (point)
                {
                    case 1: criticalEffect.DisableCriticalEffect1(playerStatus); break;
                    case 4: criticalEffect.DisableCriticalEffect4(); break;
                    case 7: criticalEffect.DisableCriticalEffect7(); break;
                    case 10: criticalEffect.DisableCriticalEffect10(); break;
                    case 13: criticalEffect.DisableCriticalEffect13(); break;
                    case 16: criticalEffect.DisableCriticalEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "dexterity":
                switch (point)
                {
                    case 1: dexterityEffect.DisableDexterityEffect1(playerStatus); break;
                    case 4: dexterityEffect.DisableDexterityEffect4(); break;
                    case 7: dexterityEffect.DisableDexterityEffect7(); break;
                    case 10: dexterityEffect.DisableDexterityEffect10(); break;
                    case 13: dexterityEffect.DisableDexterityEffect13(); break;
                    case 16: dexterityEffect.DisableDexterityEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "mystery":
                switch (point)
                {
                    case 1: mysteryEffect.DisableMysteryEffect1(); break;
                    case 4: mysteryEffect.DisableMysteryEffect4(); break;
                    case 7: mysteryEffect.DisableMysteryEffect7(); break;
                    case 10: mysteryEffect.DisableMysteryEffect10(); break;
                    case 13: mysteryEffect.DisableMysteryEffect13(); break;
                    case 16: mysteryEffect.DisableMysteryEffect16(); break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            default:
                Debug.Log("잘못된 스테이터스 이름");
                return;
        }
    }
}
