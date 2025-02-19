using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState { Default, Playing, Finish };

public class RoomOnGame : MonoBehaviour
{
    public bool OpenGateTest = false;

    private List<Door> doors = new List<Door>(); // Door는 방 프리펩 위에 그린 GameObject 타일
    private bool[] doorDirection = { false, false, false, false }; // // 위, 오른쪽, 아래, 왼쪽 (시계방향)
    private BoxCollider2D enterBox; // 플레이어가 방 안에 들어왔는지 확인하기 위한 콜라이더
    // ToDo : 적 정보 추가

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {

        }
    }
}
