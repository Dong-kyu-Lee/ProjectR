using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Room : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    public Tilemap groundTilemap;
    public Tilemap floatingTilemap;
    public Tilemap decorationTilemap;
    public GameObject gateTilemap;

    public List<Transform> enemySpawnPoint = new List<Transform>();
    public List<GameObject> interactableObjPoint = new List<GameObject>();

    public Transform playerSpawnPosition;
    public Transform finishSpotPosition;

    private Vector3 bottomLeftCorner;
    private Vector3 topRightCorner;

    public Vector3 BottomLeftCorner { get => bottomLeftCorner; }
    public Vector3 TopRightcorner { get => topRightCorner; }

    [Header("Gate")]
    public bool isUpOpenable;
    public bool isDownOpenable;
    public bool isRightOpenable;
    public bool isLeftOpenable;

    void OnEnable()
    {
        if(playerSpawnPosition == null)
        {
            Debug.LogError($"Player Spawn Position is null");
        }
        if(finishSpotPosition == null)
        {
            Debug.LogError("Finish Spot Position is null");
        }
        playerSpawnPosition.gameObject.SetActive(false);
        finishSpotPosition.gameObject.SetActive(false);
    }

    // 해당 방의 경계값을 왼쪽 아래 좌표와 오른쪽 위 좌표로 설정하는 함수
    public void SetRoomBoundary(Vector3 bottomLeftCorner, Vector3 topRightCorner)
    {
        this.bottomLeftCorner = bottomLeftCorner;
        this.topRightCorner = topRightCorner;
    }
}
