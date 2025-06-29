using UnityEngine;

public class CharacterStateController : MonoBehaviour
{
    [SerializeField] float MoveInputLerpSpeed = 12.0f; // 클수록 빠르게 따라감
    [SerializeField] bool IsAI = true;

    private Vector2 m_AnimatorMoveInput = Vector2.zero;

    private Animator m_Animator;
    private ICharacterState m_CurrentState;
    private ICharacterState m_PreviousState;
    private CharacterController m_CharacterController;

    private IdleState m_IdleState;
    private MovingState m_MovingState;
    private Attack1State m_Attack1State;
    private Attack2State m_Attack2State;
    private Attack3State m_Attack3State;
    private ParryState m_ParryState;
    private BlockState m_BlockState;
    private DamagedState m_DamagedState;
    private DeadState m_DeadState;

    public Vector2 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool BlockPressed { get; private set; }

    public IdleState IdleState => m_IdleState;
    public MovingState MovingState => m_MovingState;
    public Attack1State Attack1State => m_Attack1State;
    public Attack2State Attack2State => m_Attack2State;
    public Attack3State Attack3State => m_Attack3State;
    public ParryState ParryState => m_ParryState;
    public BlockState BlockState => m_BlockState;
    public DamagedState DamagedState => m_DamagedState;
    public DeadState DeadState => m_DeadState;

    public CharacterController CharacterController => m_CharacterController;
    public Animator CharacterAnimator => m_Animator;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_IdleState = new IdleState(this);
        m_MovingState = new MovingState(this);
        m_Attack1State = new Attack1State(this);
        m_Attack2State = new Attack2State(this);
        m_Attack3State = new Attack3State(this);
        m_ParryState = new ParryState(this);
        m_BlockState = new BlockState(this);
        m_DamagedState = new DamagedState(this);
        m_DeadState = new DeadState(this);
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        ChangeState(m_IdleState);
    }

    void Update()
    {
        m_CurrentState.HandleInput();
        m_CurrentState.UpdateState();
        UpdateAnimatorParameters();
    }

    public void SetInput(Vector2 move, bool attack, bool parry, bool block)
    {
        MoveInput = move;
        AttackPressed = attack;
        ParryPressed = parry;
        BlockPressed = block;
    }

    public void ChangeState(ICharacterState newState)
    {
        if (m_CurrentState == m_DeadState) return;
        m_PreviousState?.ExitState();
        m_PreviousState = m_CurrentState;
        m_CurrentState = newState;
        m_CurrentState.EnterState();
    }

    private void UpdateAnimatorParameters()
    {
        m_AnimatorMoveInput = Vector2.Lerp(m_AnimatorMoveInput, MoveInput, Time.deltaTime * MoveInputLerpSpeed);

        m_Animator.SetFloat("MovingX", m_AnimatorMoveInput.x);
        m_Animator.SetFloat("MovingY", m_AnimatorMoveInput.y);
        m_Animator.SetBool("IsMoving", m_CurrentState.GetState() == CharacterState.Move);
    }

    public void OnDamaged(Vector3 attackerPosition, float knockbackForce, float duration = 0.25f, int damage = 0)
    {
        // 공격자 위치, 넉백, 애니메이션 등 처리
        Vector3 knockbackDir = (transform.position - attackerPosition).normalized;
        m_DamagedState.SetDamageInfo(knockbackDir, knockbackForce, duration);

        // 데미지 처리
        if (damage > 0)
        {
            // 방어력 반영하여 체력 감소
            int finalDamage = Mathf.Max(1, damage - m_CharacterController.GetStatValue(CharacterStat.Defence));
            m_CharacterController.ApplyDamage(finalDamage);
        }

        if (m_CharacterController.GetCurrentHp() <= 0)
            ChangeState(m_DeadState);
        else
            ChangeState(m_DamagedState);
    }

    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.label);
        myStyle.normal.textColor = Color.green;
        myStyle.fontSize = 20;

        if (this.tag == "Player")
            GUI.Label(new Rect(10, 10, 200, 30), $"Current State : {m_CurrentState.GetState()}", myStyle);
        else if (this.tag == "AI")
            GUI.Label(new Rect(1000, 10, 200, 30), $"Current State : {m_CurrentState.GetState()}", myStyle);
    }
}
