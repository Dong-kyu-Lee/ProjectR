using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTestCase : MonoBehaviour
{
    public float buffDuration = 5.0f;

    
    private BuffManager buffManager;

    // 적용 가능한 모든 버프 타입
    private BuffType[] availableBuffTypes;

    void Start()
    {
        buffManager = GetComponent<BuffManager>();
        if (buffManager == null)
        {
            Debug.LogError("없음");
        }

        availableBuffTypes = (BuffType[])System.Enum.GetValues(typeof(BuffType));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            int randomIndex = Random.Range(0, availableBuffTypes.Length);
            BuffType randomBuff = availableBuffTypes[randomIndex];

            Debug.Log("BuffTest: 적용할 버프/디버프: " + randomBuff);

            buffManager.ActivateBuff(randomBuff, buffDuration);
        }
    }
}