using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUI_BG : MonoBehaviour
{
    public Action onAnimationEnd;

    public void OnAnimationEnd()
    {
        if(onAnimationEnd != null)
        {
            onAnimationEnd.Invoke();
            onAnimationEnd = null;
        }
    }
}
