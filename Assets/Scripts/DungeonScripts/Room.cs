using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public GameObject roomPrefab;
    public Tilemap groundTilemap;
    public GameObject gateTilemap;

    public List<GameObject> enemySpawnPoint = new List<GameObject>();
    public List<GameObject> interactableObjPoint = new List<GameObject>();

    [Header("Gate")]
    public bool isUpOpenable;
    public bool isDownOpenable;
    public bool isRightOpenable;
    public bool isLeftOpenable;

    [Header("Corner Tiles")]
    // 통로 가장자리 부분의 자연스럽지 않은 일자 타일을 코너 타일로 바꾸기 위한 타일 데이터
    [SerializeField] private TileBase rightDownCornerTile;
    [SerializeField] private TileBase leftDownCornerTile;
    [SerializeField] private TileBase rightUpCornerTile;
    [SerializeField] private TileBase leftUpCornerTile;

    [Header("Side Tiles")]
    [SerializeField] private TileBase leftSideTile;
    [SerializeField] private TileBase rightSideTile;
    [SerializeField] private TileBase upSideTile;
    [SerializeField] private TileBase downSideTile;


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

            // 통로 가장자리 마감처리
            // 통로 가장자리 타일과 접한 땅 타일이 없다면 "ㄱ"자 타일 배치
            if(groundTilemap.GetTile(new Vector3Int(7, 18, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(7, 19, 0), rightDownCornerTile);
            // 접한 땅 타일이 있다면 그와 연결되도록 "ㅡ"자 타일 배치
            else groundTilemap.SetTile(new Vector3Int(7, 19, 0), rightSideTile);

            if (groundTilemap.GetTile(new Vector3Int(12, 18, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(12, 19, 0), leftDownCornerTile);
            else groundTilemap.SetTile(new Vector3Int(12, 19, 0), leftSideTile);
        }
        if (openedGateArray[1] == true) // 오른쪽
        {
            groundTilemap.SetTile(new Vector3Int(19, 8, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 9, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 10, 0), null);
            groundTilemap.SetTile(new Vector3Int(19, 11, 0), null);

            if(groundTilemap.GetTile(new Vector3Int(18, 7, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(19, 7, 0), leftUpCornerTile);                
            else groundTilemap.SetTile(new Vector3Int(19, 7, 0), upSideTile);

            if (groundTilemap.GetTile(new Vector3Int(18, 12, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(19, 12, 0), leftDownCornerTile);
            else groundTilemap.SetTile(new Vector3Int(19, 12, 0), downSideTile);
        }
        if (openedGateArray[2] == true) // 아래
        {
            groundTilemap.SetTile(new Vector3Int(8, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(9, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(10, 0, 0), null);
            groundTilemap.SetTile(new Vector3Int(11, 0, 0), null);

            if (groundTilemap.GetTile(new Vector3Int(7, 1, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(7, 0, 0), rightUpCornerTile);
            else groundTilemap.SetTile(new Vector3Int(7, 0, 0), rightSideTile);

            if (groundTilemap.GetTile(new Vector3Int(12, 1, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(12, 0, 0), leftUpCornerTile);
            else groundTilemap.SetTile(new Vector3Int(12, 0, 0), leftSideTile);
        }
        if (openedGateArray[3] == true) // 왼쪽
        {
            groundTilemap.SetTile(new Vector3Int(0, 8, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 9, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 10, 0), null);
            groundTilemap.SetTile(new Vector3Int(0, 11, 0), null);

            if(groundTilemap.GetTile(new Vector3Int(1, 7, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(0, 7, 0), rightUpCornerTile);
            else groundTilemap.SetTile(new Vector3Int(0, 7, 0), upSideTile);
            
            if(groundTilemap.GetTile(new Vector3Int(1, 12, 0)) == null)
                groundTilemap.SetTile(new Vector3Int(0, 12, 0), rightDownCornerTile);
            else groundTilemap.SetTile(new Vector3Int(0, 12, 0), downSideTile);
        }

        gateTilemap.SetActive(false);
    }
}
