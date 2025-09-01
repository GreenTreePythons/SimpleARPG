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
        // m_AnimController.PlayNormalIdle();
        m_AnimController.PlayBattleIdle();
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

        // 카메라 기준 이동 방향 변환
        Vector2 moveInput = GameManager.Instance.InputManager.MoveDirection;
        
        // Transform cam = m_StateController.GetPlayerController().GetCameraManager().transform;
        CameraManager cam = GameManager.Instance.CameraManager;
        Transform camTransform = cam.transform;
        
        Vector3 camForward = Vector3.Scale(camTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = camTransform.right;

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir = moveDir.normalized;

        // 애니메이션
        m_AnimController.PlayWalking(moveInput);

        // 실제 이동
        var walkSpeed = m_StateController.WalkSpeed * Time.deltaTime;
        m_StateController.transform.position += moveDir * walkSpeed;

        float rotateSpeed = m_StateController.RotationSpeed * Time.deltaTime;

        // LockOn 여부에 따라 회전 방식 분기
        if (GameManager.Instance.InputManager.IsLockOnTarget && cam.LockOnTarget != null)
        {
            // 록온: 적 방향으로만 회전
            Vector3 lookDir = (cam.LockOnTarget.position - m_StateController.transform.position).normalized;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                m_StateController.transform.rotation = Quaternion.Slerp(m_StateController.transform.rotation, targetRotation, rotateSpeed);
            }
        }
        else
        {
            // 평소: 이동 입력 방향으로 회전
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                m_StateController.transform.rotation = Quaternion.Slerp(m_StateController.transform.rotation, targetRotation, rotateSpeed);
            }
        }

        m_StateController.CheckStateTransition(TransitionType.Idle | TransitionType.Attack);
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
        base.OnExit();
    }
}
