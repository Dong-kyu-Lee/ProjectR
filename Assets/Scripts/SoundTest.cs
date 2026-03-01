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
            GameManager.Sound.Play("Sounds/BGM/BGM1", Sound.Bgm);
            GameManager.Sound.Play(ac1);
        }
    }
}
