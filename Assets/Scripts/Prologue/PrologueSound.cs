using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PrologueSound : MonoBehaviour
{
    public AudioClip firstBGM;  // 왕궁 배경음
    public AudioClip secondBGM; // 불안한 배경음
    // public AudioClip thirdBGM; 
    public AudioClip thunderSound;

    void Start()
    {
        if (GameManager.Instance == null) Debug.Log("GameManager Instance is null");
        SoundManager.Instance.Play(firstBGM, Sound.Bgm);
    }

    public void PlaySecondBGM()
    {
        SoundManager.Instance.Play(secondBGM, Sound.Bgm);
    }
    public void PlayThunderSound()
    {
        SoundManager.Instance.Play(thunderSound, Sound.Effect);
    }
    public void StopBGM()
    {
        SoundManager.Instance.Clear();
    }
}
