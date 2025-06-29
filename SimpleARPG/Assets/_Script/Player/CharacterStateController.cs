using UnityEngine;

public class CharacterStateController : MonoBehaviour
{
    private Animator m_Animator;
    private ICharacterState m_CurrentState;

    // State 객체들 재사용
    private IdleState m_IdleState;
    private MovingState m_MovingState;
    private Attack1State m_Attack1State;
    private Attack2State m_Attack2State;
    private Attack3State m_Attack3State;
    private ParryState m_ParryState;
    private BlockState m_BlockState;

    // 입력값 저장
    public Vector2 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool BlockPressed { get; private set; }

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
        m_CurrentState = newState;
        m_CurrentState.EnterState();

        AttackPressed = false;
        ParryPressed = false;
        BlockPressed = false;
    }

    public IdleState IdleState => m_IdleState;
    public MovingState MovingState => m_MovingState;
    public Attack1State Attack1State => m_Attack1State;
    public Attack2State Attack2State => m_Attack2State;
    public Attack3State Attack3State => m_Attack3State;
    public ParryState ParryState => m_ParryState;
    public BlockState BlockState => m_BlockState;

    private void UpdateAnimatorParameters()
    {
        float moveDirection = 0;
        if (MoveInput.y > 0) moveDirection = 1;
        else if (MoveInput.y < 0) moveDirection = -1;
        else if (MoveInput.x > 0) moveDirection = 2;
        else if (MoveInput.x < 0) moveDirection = -2;

        m_Animator.SetFloat("MovingDirection", moveDirection);
        m_Animator.SetBool("IsMoving", m_CurrentState.GetState() == CharacterState.Move);
    }

    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.label);
        myStyle.normal.textColor = Color.green;
        myStyle.fontSize = 20;
        GUI.Label(new Rect(10, 10, 200, 30), $"Current State : {m_CurrentState.GetState()}", myStyle);
    }
}
