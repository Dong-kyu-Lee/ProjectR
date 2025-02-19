using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 방 프리팹 하나의 정보를 나타내는 데이터 클래스
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
}
