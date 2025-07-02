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
    private int m_ComboStep;
    private float m_CurrentComboElapsedTime;
    private AttackComboInfo m_CurrentComboInfo;
    private bool m_IsNextComboReserved;
    private ComboType m_ReservedNextComboType;
    
    public CharacterAttackingState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }

    public override void OnEnter()
    {
        base.OnEnter();
        m_ComboStep = 0;
        ClearComboState();
        m_ComboDatas = m_StateController.GetComboDatas(GameManager.Instance.InputManager.LatestComboTypeInput);
        m_CurrentComboInfo = m_ComboDatas[m_ComboStep];
    }

    public override void Update()
    {
        base.Update();
        m_CurrentComboElapsedTime += Time.deltaTime;
        m_StateController.ComboTimer = m_CurrentComboElapsedTime;

        var currentComboValidTime = m_CurrentComboElapsedTime >= m_CurrentComboInfo.ComboValidStartTime &&
                                    m_CurrentComboElapsedTime <= m_CurrentComboInfo.ComboValidTime;
        var canTransitionToNextCombo = m_ComboStep == 0 || m_CurrentComboElapsedTime >= m_CurrentComboInfo.ComboValidTime;
        
        // reserve next combo
        if (currentComboValidTime && !m_IsNextComboReserved )
        {
            var input = GameManager.Instance.InputManager.LatestComboTypeInput;
            if (input != ComboType.None)
            {
                m_ReservedNextComboType = input;
                m_IsNextComboReserved = true;
                m_StateController.NextComboQueued = true;
            }
        }
        
        // play next combo
        if (m_IsNextComboReserved && canTransitionToNextCombo)
        {
            if (m_ComboStep < m_ComboDatas.Length)
            {
                m_CurrentComboInfo = m_ComboDatas[m_ComboStep];
                m_AnimController.PlayAttack(m_CurrentComboInfo.AnimStateName);
                
                m_ComboStep++;
                ClearComboState();
                return;
            }
        }
        
        // exit combo
        if (m_CurrentComboElapsedTime > m_CurrentComboInfo.ComboValidTime)
        {
            m_ComboStep = 0;
            ClearComboState();
            m_StateController.ChangeState(CharacterStateType.Idle);
        }
    }

    private void ClearComboState()
    {
        m_CurrentComboElapsedTime = 0f;
        m_IsNextComboReserved = false;
        m_ReservedNextComboType = ComboType.None;
            
        m_StateController.NextComboQueued = false;
        m_StateController.ComboTimer = 0.0f;
        m_StateController.CurrentComboStep = m_ComboStep;
    }

    public override void OnExit()
    {
        GameManager.Instance.InputManager.MoveInputPressed = false;
        base.OnExit();
    }
}
