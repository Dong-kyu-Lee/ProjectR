using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class LiberationSystem : MonoBehaviour
{
    private float crystal;

    public float Crystal
    {
        get { return crystal; }
        set
        {
            crystal = value;
        }
    }

    // 해방 특수 효과 활성화.
    public void EnableLiberationEffect(string characterName, int point)
    {
        switch (characterName)
        {
            case "bartender":
                switch (point)
                {
                    case 1: break;
                    case 2: break;
                    case 3: break;
                    case 4: break;
                    case 5: break;
                    case 6: break;
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
