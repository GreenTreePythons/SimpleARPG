using UnityEngine;

// State Base
public abstract class CharacterStateBase
{
    protected CharacterFSMStatesController m_StateController;
    protected CharacterAnimationController m_AnimController;

    protected CharacterStateBase(CharacterFSMStatesController controller, CharacterAnimationController animController)
    {
        this.m_StateController = controller;
        this.m_AnimController = animController;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update() { }
}

// Idle State
public class CharacterIdleState : CharacterStateBase
{
    public CharacterIdleState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }
    
    public override void OnEnter()
    {
        base.OnEnter();
        m_AnimController.PlayNormalIdle();
    }

    public override void Update()
    {
        base.Update();
        m_StateController.CheckStateTransition(TransitionType.Move | TransitionType.Attack);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

// Moving State
public class CharacterMovingState : CharacterStateBase
{
    public CharacterMovingState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void Update()
    {
        base.Update();
        var m_InputDir = GameManager.Instance.InputManager.MoveDirection;
        m_AnimController.PlayWalking(m_InputDir);
        
        m_StateController.CheckStateTransition(TransitionType.Idle | TransitionType.Attack);
        
        var walkSpeed = m_StateController.WalkSpeed * Time.deltaTime;
        m_StateController.transform.position += m_StateController.transform.forward * walkSpeed;
        
        Vector3 inputDirection = new Vector3(m_InputDir.x, 0, m_InputDir.y).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        var rotateSpeed = m_StateController.RotationSpeed * Time.deltaTime;
        m_StateController.transform.rotation = Quaternion.Slerp(m_StateController.transform.rotation, targetRotation, rotateSpeed);
    }
    
    public override void OnExit()
    {
        base.OnExit();
    }
}

// Attacking State
public class CharacterAttackingState : CharacterStateBase
{
    private AttackComboInfo[] m_ComboDatas;
    
    public CharacterAttackingState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }

    public override void OnEnter()
    {
        base.OnEnter();
        m_ComboDatas = m_StateController.GetComboDatas(GameManager.Instance.InputManager.LatestComboTypeInput);
        m_AnimController.PlayAttack(m_ComboDatas[0].AnimStateName);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
