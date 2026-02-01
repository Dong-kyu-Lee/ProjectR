using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAnimationController : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayRunAnimation()
    {
        animator.SetBool("IsRunning", true);
    }

    public void StopRunAnimation()
    {
        animator.SetBool("IsRunning", false);
    }
}
