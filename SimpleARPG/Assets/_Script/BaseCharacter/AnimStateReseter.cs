using UnityEngine;

public class AnimStateReseter : StateMachineBehaviour
{
    [SerializeField] string ResetTriggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        
        if (ResetTriggerName != null) animator.ResetTrigger(ResetTriggerName);
    }
}