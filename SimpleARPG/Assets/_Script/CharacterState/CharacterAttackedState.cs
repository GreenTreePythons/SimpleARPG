using UnityEngine;

public class CharacterAttackedState : CharacterBaseState
{
    private float m_EnterTime;
    
    public CharacterAttackedState(CharacterFSMStatesController controller, CharacterAnimationController animController) 
        : base(controller, animController) { }
    
    public override void OnEnter()
    {   
        base.OnEnter();
        m_EnterTime = Time.time;
        m_AnimController.PlayHit();
        
        m_StateController.SetInputLocked(true);
        m_StateController.SetMoveLocked(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        AnimatorStateInfo info = m_AnimController.GetAnimator().GetCurrentAnimatorStateInfo(0);

        // bool isInHit = info.shortNameHash == m_HitStateHash || info.IsTag("Hit");
        if (info.normalizedTime >= 0.95f)
        {
            // if (m_StateController.IsMoving())
            //     m_StateController.ChangeState(CharacterStateType.Moving);
            // else
                m_StateController.ChangeState(CharacterStateType.Idle);
                Debug.Log($"{info.shortNameHash} end");
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        m_StateController.SetInputLocked(false);
        m_StateController.SetMoveLocked(false);
    }
}