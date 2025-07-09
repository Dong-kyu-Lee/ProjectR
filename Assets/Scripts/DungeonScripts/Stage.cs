using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageFlow
{
    Lobby, Stage1, Stage2, MiddleBoss, Stage3, Stage4, FinalBoss
}

public class Stage : MonoBehaviour
{
    [SerializeField]
    private StageData stageData;
    private List<RoomInstance> rooms;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
