using UnityEngine;

// 강화 모션의 실제 종료 여부만 전달하고, 결과 적용은 컨트롤러의 LateUpdate에서 처리한다.
public class BlacksmithEnchantStateBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<BlacksmithControllerV2>()
            ?.NotifyEnchantAnimationEnded(stateInfo.normalizedTime >= 1f);
    }
}
