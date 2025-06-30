using UnityEngine;
using System.Collections.Generic;

public class CharacterStateController : MonoBehaviour
{
    [SerializeField] bool IsAI = true;

    private Animator m_Animator;
    private ICharacterState m_CurrentState;
    private ICharacterState m_PreviousState;
    private CharacterController m_CharacterController;

    public Vector2 MoveInput { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool BlockPressed { get; private set; }

    public IdleState IdleState { get; private set; }
    public MovingState MovingState { get; private set; }
    public ParryState ParryState { get; private set; }
    public BlockState BlockState { get; private set; }
    public DamagedState DamagedState { get; private set; }
    public DeadState DeadState { get; private set; }

    public CharacterController CharacterController => m_CharacterController;
    public Animator CharacterAnimator => m_Animator;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        IdleState = new IdleState(this);
        MovingState = new MovingState(this);
        ParryState = new ParryState(this);
        BlockState = new BlockState(this);
        DamagedState = new DamagedState(this);
        DeadState = new DeadState(this);
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        ChangeState(IdleState);
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
        ParryPressed = parry;
        BlockPressed = block;
    }

    public void ChangeState(ICharacterState newState)
    {
        if (m_CurrentState == DeadState) return;

        m_PreviousState?.ExitState();
        m_PreviousState = m_CurrentState;
        m_CurrentState = newState;
        m_CurrentState.EnterState();
    }

    private void UpdateAnimatorParameters()
    {
        m_Animator.SetFloat("MovingX", MoveInput.x, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("MovingY", MoveInput.y,0.1f, Time.deltaTime);
        m_Animator.SetBool("IsMoving", m_CurrentState.GetState() == CharacterState.Move);
    }

    // public void OnDamaged(Vector3 attackerPosition, float knockbackForce, float duration = 0.25f, float damage = 0)
    // {
    //     Vector3 knockbackDir = (transform.position - attackerPosition).normalized;
    //     DamagedState.SetDamageInfo(knockbackDir, knockbackForce, duration);

    //     if (damage > 0)
    //     {
    //         float finalDamage = Mathf.Max(1, damage - m_CharacterController.GetStatValue(CharacterStat.Defence));
    //         m_CharacterController.ApplyDamage(finalDamage);
    //     }

    //     if (m_CharacterController.GetCurrentHp() <= 0)
    //         ChangeState(DeadState);
    //     else
    //         ChangeState(DamagedState);
    // }
}
