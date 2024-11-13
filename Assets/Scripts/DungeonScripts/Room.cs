using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public GameObject roomPrefab;
    public Tilemap groundTilemap;
    public GameObject gateTilemap;

    // 프로토타입을 위한 임시 맴버변수. 추후 던전 데이터 관리 클래스로 옮길 값.
    public TileBase backgroundTileBase;

    [Header("Gate")]
    public bool isUpOpenable;
    public bool isDownOpenable;
    public bool isRightOpenable;
    public bool isLeftOpenable;

    void Start()
    {
        if(gateTilemap == null)
        {
            Debug.LogError($"{gameObject.name} doesn't have Gate Tilemap property");
        }
        if(groundTilemap == null)
        {
            Debug.LogError($"{gameObject.name} doesn't have Ground Tilemap property");
        }
    }

    // 통로가 되어야 할 곳의 벽 타일을 지우는 함수
    public void OpenGateTile(bool[] openedGateArray)
    {
        if(openedGateArray[0] == true) // 위
        {
            groundTilemap.SetTile(new Vector3Int(8, 19, 0), null);
            groundTilemap.SetTile(new Vector3Int(9, 19, 0), null);
            groundTilemap.SetTile(new Vector3Int(10, 19, 0), null);
            groundTilemap.SetTile(new Vector3Int(11, 19, 0), null);
        }
        if (openedGateArray[1] == true) // 오른쪽
        {
            groundTilemap.SetTile(new Vector3Int(19, 8, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 9, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 10, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 11, 0), null);
        }
        if(openedGateArray[2] == true) // 아래
        {
            groundTilemap.SetTile(new Vector3Int(8, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(9, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(10, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(11, 0, 0), null);
        }
        if(openedGateArray[3] == true) // 왼쪽
        {
            groundTilemap.SetTile(new Vector3Int(0, 8, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 9, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 10, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 11, 0), null);
        }

        gateTilemap.SetActive(false);
    }
}
