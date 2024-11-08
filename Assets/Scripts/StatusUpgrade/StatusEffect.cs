using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private Status status;
    [SerializeField] private ForceEffect forceEffect;

    private void Update()
    {
        Debug.Log("Damage : " + status.Damage);
    }

    // 특수 효과 활성화
    public void EnableEffect(string statName, int point)
    {
        switch (statName)
        {
            case "force":
                switch (point)
                {
                    case 1: forceEffect.EnableForceEffect1(status); break;
                    case 4: forceEffect.EnableForceEffect4(); break;
                    case 7: forceEffect.EnableForceEffect7(); break;
                    case 10: forceEffect.EnableForceEffect10(); break;
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
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "critical":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "dexterity":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "mystery":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
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

    // 특수 효과 비활성화
    public void DisableEffect(string statName, int point)
    {
        switch (statName)
        {
            case "force":
                switch (point)
                {
                    case 1: forceEffect.DisableForceEffect1(status); break;
                    case 4: forceEffect.DisableForceEffect4(); break;
                    case 7: forceEffect.DisableForceEffect7(); break;
                    case 10: forceEffect.DisableForceEffect10(); break;
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
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "critical":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "dexterity":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
                    default:
                        Debug.Log("올바르지 않는 스텟포인트");
                        return;
                }
                break;
            case "mystery":
                switch (point)
                {
                    case 1: break;
                    case 4: break;
                    case 7: break;
                    case 10: break;
                    case 13: break;
                    case 16: break;
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
