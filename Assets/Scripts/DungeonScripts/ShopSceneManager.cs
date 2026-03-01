using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject finishSpot;
    [SerializeField]
    GameObject playerSpawnPosition;

    void Start()
    {
        if (finishSpot == null || playerSpawnPosition == null)
        {
            Debug.LogError("One or more required GameObjects are not assigned in the inspector.");
            return;
        }

        GameManager.Instance.PlacePlayerObject(playerSpawnPosition.transform.position);
        GameManager.Instance.CurrentPlayer.SetActive(true);
        finishSpot.GetComponent<FinishSpot>().isWaveEnd = true;
    }
}
