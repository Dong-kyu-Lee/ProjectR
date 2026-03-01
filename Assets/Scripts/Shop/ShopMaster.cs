using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopMaster : MonoBehaviour
{
    float timeCount;
    int sayNum;

    private void Start()
    {
        timeCount = 0;
    }
    private void Update()
    {
        timeCount += Time.deltaTime;
        if(timeCount >= 3)
        {
            sayNum = Random.Range(0, 3);
            //정해진 말 중 렌덤으로 정해진 말풍선 출력
            timeCount = 0;
        }
    }
}
