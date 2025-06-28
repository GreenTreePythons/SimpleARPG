using UnityEngine;

public class CharacterActionController : MonoBehaviour
{
    private Animator m_Animator;
    private CharacterState m_CurrentState;

    public Vector2 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool BlockPressed { get; private set; }

    private int m_CurrentComboCount = 0;
    private const float m_ComboTimeLimit = 5.5f;
    private float m_LastAttackTime = 0f;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CurrentState = CharacterState.Idle;
        ChangeState(new IdleState(this));
    }

    public void Update()
    {
        // HandleState();
        UpdateAnimator();

        if (Time.time - m_LastAttackTime > m_ComboTimeLimit)
        {
            m_CurrentComboCount = 0;
        }
    }

    private void HandleState()
    {
        switch (m_CurrentState)
        {
            case CharacterState.Idle: HandleIdleState(); break;
            case CharacterState.Move: HandleMoveState(); break;
            case CharacterState.Attack1: HandleAttackState(); break;
            case CharacterState.Attack2: HandleAttackState(); break;
            case CharacterState.Attack3: HandleAttackState(); break;
            case CharacterState.Parry: HandleParryState(); break;
            case CharacterState.Block: HandleBlockState(); break;
        }
        Debug.Log($"m_CurrentState : {m_CurrentState}");
        Debug.Log($"m_CurrentComboCount : {m_CurrentComboCount}");
    }

    public void HandleInput(Vector2 moveInput, bool attackPressed, bool parryPressed, bool blockPressed)
    {
        this.MoveInput = moveInput;
        this.AttackPressed = attackPressed;
        this.ParryPressed = parryPressed;
        this.BlockPressed = blockPressed;

        if (attackPressed)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
            {
                m_CurrentComboCount++;
                m_CurrentComboCount = Mathf.Clamp(m_CurrentComboCount, 0, 3);
            }

            if (m_CurrentComboCount == 1) m_CurrentState = CharacterState.Attack1;
            else if (m_CurrentComboCount == 2) m_CurrentState = CharacterState.Attack2;
            else if (m_CurrentComboCount == 3) m_CurrentState = CharacterState.Attack3;
            else
            {
                m_CurrentState = CharacterState.Idle;
            }
        }
        else if (parryPressed)
        {
            m_CurrentState = CharacterState.Parry; // 패리 상태
            m_CurrentComboCount = 0;

        }
        else if (blockPressed)
        {
            m_CurrentState = CharacterState.Block; // 블록 상태
            m_CurrentComboCount = 0;
        }
        else if (moveInput.magnitude > 0)
        {
            m_CurrentState = CharacterState.Move; // 이동 상태
            m_CurrentComboCount = 0;
        }
        else
        {
            m_CurrentState = CharacterState.Idle; // 기본 상태 (Idle)
            m_CurrentComboCount = 0;
        }

        HandleState();
    }

    // 상태 전환 메서드
    public void ChangeState(ICharacterState newState)
    {
        m_CurrentState = newState.GetState();
        newState.EnterState();
    }

    private void HandleIdleState()
    {
        if (MoveInput.magnitude > 0) ChangeState(new MovingState(this));
        if (AttackPressed) ChangeState(new Attack1State(this));
    }

    private void HandleMoveState()
    {
        if (AttackPressed) ChangeState(new Attack1State(this));
        if (ParryPressed) ChangeState(new ParryingState(this));
        if (BlockPressed) ChangeState(new BlockingState(this));
        if (MoveInput.magnitude == 0) ChangeState(new IdleState(this));
    }

    private void HandleAttackState()
    {
        m_LastAttackTime = Time.time;

        if (m_CurrentComboCount == 1)
        {
            ChangeState(GetAttackState(m_CurrentComboCount));
            m_Animator.SetBool("Attack1", true);
        }

        if (m_CurrentComboCount >= 2 &&
        m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
        && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            ChangeState(GetAttackState(m_CurrentComboCount));
            m_Animator.SetBool("Attack1", false);
            m_Animator.SetBool("Attack2", true);
        }

        if (m_CurrentComboCount >= 3 &&
        m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
        && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            ChangeState(GetAttackState(m_CurrentComboCount));
            m_Animator.SetBool("Attack2", false);
            m_Animator.SetBool("Attack3", true);
        }
    }

    private void HandleParryState()
    {
        m_Animator.SetTrigger("Parry");
        ChangeState(new IdleState(this));
    }

    private void HandleBlockState()
    {
        m_Animator.SetTrigger("Block");
        ChangeState(new IdleState(this));
    }

    private void UpdateAnimator()
    {
        // 이동 상태에서 BlendTree에 방향을 설정
        float moveDirection = 0;

        if (MoveInput.y > 0) moveDirection = 1; // Forward
        else if (MoveInput.y < 0) moveDirection = -1; // Backward
        else if (MoveInput.x > 0) moveDirection = 2; // Right
        else if (MoveInput.x < 0) moveDirection = -2; // Left

        m_Animator.SetFloat("MovingDirection", moveDirection); // BlendTree에 사용할 변수
        m_Animator.SetBool("IsMoving", m_CurrentState == CharacterState.Move);

        bool IsAttack1End = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1");
        bool IsAttack2End = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2");
        bool IsAttack3End = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3");
        m_Animator.SetBool("Attack1", IsAttack1End);
        m_Animator.SetBool("Attack2", IsAttack2End);
        m_Animator.SetBool("Attack3", IsAttack3End);
    }

    ICharacterState GetAttackState(int comboCount) => comboCount switch
    {
        1 => new Attack1State(this),
        2 => new Attack2State(this),
        3 => new Attack3State(this),
        _ => new Attack1State(this)
    };
}
