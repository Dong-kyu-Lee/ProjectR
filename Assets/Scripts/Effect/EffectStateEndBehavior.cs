using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class EffectStateEndBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
        animator.gameObject.SetActive(false);
    }
}
