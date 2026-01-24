using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueCutScene : MonoBehaviour
{
    public void CutSceneEnd()
    {
        GameManager.Instance.prologue.CompleteCutScene();
    }
}
