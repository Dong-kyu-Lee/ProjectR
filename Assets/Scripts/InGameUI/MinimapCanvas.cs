using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCanvas : MonoBehaviour
{
    public GameObject fullMap;
    public GameObject minimap;

    void Start()
    {
        
    }

    public void ShowFullMap()
    {
        if (fullMap != null)
        {
            fullMap.SetActive(true);
            minimap.SetActive(false);
        }
    }

    public void CloseFullMap()
    {
        if (minimap != null)
        {
            minimap.SetActive(true);
            fullMap.SetActive(false);
        }
    }
}
