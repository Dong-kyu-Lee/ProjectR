using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneContentsPopup : MonoBehaviour
{
    [SerializeField] GameObject contents;

    public void OpenPopup()
    {
        contents.SetActive(true);
    }
}
