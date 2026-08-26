using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffectSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private LevelUpText levelUpText;

    private void Awake()
    {
        levelUpText = GetComponentInChildren<LevelUpText>();
    }
    public void Play()
    {
        if (levelUpParticle != null)
        {
            // Reset existing particle emission before playing
            levelUpParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            levelUpParticle.Play();
        }
        
        if (levelUpText != null)
        {
            levelUpText.PlayLevelUpText();
        }
    }
}
