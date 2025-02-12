using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


//본 스크립트는 독 아이템 수류탄 용 독 장판을 임시로 정의하는 스크립트임.
public class TempPosionArea : MonoBehaviour
{
    private Vector2 size = new Vector2(10, 0.5f);
    private Dictionary<GameObject, Coroutine> targetPoisonTimes = new Dictionary<GameObject, Coroutine>();
    private float maxTimeToDelete = 5.0f;
    
    public Vector2 Size
    {
        get { return size; }
        set 
        { 
            if (value.x < 0.0f) value.x = 0;
            if (value.y < 0.0f) value.y = 0;
            size = value;
        }
    }

    private void Awake()
    {
        transform.localScale = size;
        StartCoroutine(SetTimerToDelete());
    }

    private IEnumerator SetTimerToDelete ()
    {
        yield return new WaitForSeconds(maxTimeToDelete);
        Destroy(gameObject);
    }
}