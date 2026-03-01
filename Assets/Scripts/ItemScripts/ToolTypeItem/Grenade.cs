using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grenade : MonoBehaviour
{
    [SerializeField]
    protected float timeToExplode = 5.0f;     //폭발 시간
    public float TimeToExplode => timeToExplode;


    private void Start()
    {
        StartCoroutine(StartExplodeTimer());
    }

    private IEnumerator StartExplodeTimer()
    {
        yield return new WaitForSeconds(timeToExplode);
        Explode();
    }

    protected abstract void Explode();
}
