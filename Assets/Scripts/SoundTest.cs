using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public AudioClip ac1;
    bool isEntered = false;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isEntered == false && collision.CompareTag("Player"))
        {
            isEntered = true;
            SoundManager.Instance.Play("Sounds/BGM/BGM1", Sound.Bgm);
            SoundManager.Instance.Play(ac1);
        }
    }
}
