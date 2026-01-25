using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueSound : MonoBehaviour
{
    public AudioClip firstBGM;  // 왕궁 배경음
    public AudioClip secondBGM; // 불안한 배경음
    public AudioClip thirdBGM; 
    public AudioClip thunderSound;

    void Start()
    {
        GameManager.Sound.Play(firstBGM, Sound.Bgm);
    }
}
