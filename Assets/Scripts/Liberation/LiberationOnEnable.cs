using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LiberationUI;

public class LiberationOnEnable : MonoBehaviour
{
    public event Action<bool> OnStatusChanged;

    private void OnEnable()
    {
        // 활성화될 때 실행 (true 전달)
        OnStatusChanged?.Invoke(true);
    }

    private void OnDisable()
    {
        // 비활성화될 때 실행 (false 전달)
        OnStatusChanged?.Invoke(false);
    }
}
