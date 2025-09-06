using UnityEngine;

public class CharacterIdleState : CharacterBaseState
{
    public CharacterIdleState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }
    private bool m_IsLockOnTarget = false;
    
    public override void OnEnter()
    {
        base.OnEnter();
        m_IsLockOnTarget = GameManager.Instance.InputManager.IsLockOnTarget;
        m_AnimController.PlayIdle();
        
        m_AnimController.CharacterWeaponIKController.ForceImmediateState(m_IsLockOnTarget);
    }

    public override void Update()
    {
        base.Update();
        var isTargetLockOn = GameManager.Instance.InputManager.IsLockOnTarget;
        if (m_IsLockOnTarget != isTargetLockOn)
        {
            m_IsLockOnTarget = isTargetLockOn;
            if (m_IsLockOnTarget)
            {
                m_AnimController.PlayEquipping();
                m_AnimController.CharacterWeaponIKController.OnEquipSwitch();
            }
            else
            {
                m_AnimController.PlayUnequipping();
                m_AnimController.CharacterWeaponIKController.OnUnequipSwitch();
            }
        }
        m_StateController.CheckStateTransition(TransitionType.Move | TransitionType.Attack);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}