using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum CharacterStateType
{
    Idle,
    Moving,
    Attacking,
    Attacked,
    Equipping,
    Unequipping,
}

public enum ComboType { None, Light, Heavy }

public enum TransitionType
{
    None = 0,
    Idle = 1 << 0,
    Move = 1 << 1,
    Attack = 1 << 2,
}

[RequireComponent(typeof(CharacterAnimationController))]
public class CharacterFSMStatesController : MonoBehaviour
{
    [SerializeField] public float WalkSpeed = 2.0f;
    [SerializeField] public float RunSpeed = 4.0f;
    [SerializeField] public float SprintSpeed = 7.0f;
    [SerializeField] public float RotationSpeed = 10.0f;
    [SerializeField] private AttackComboDatas m_AttackComboDatas;
    
    public CharacterStateType CurrentStateType { get; private set; }
    
    private CharacterBaseState m_CurrentBaseState;
    private CharacterAnimationController m_AnimController;
    private PlayerController m_PlayerController;
    private bool m_InputLocked;
    private bool m_MoveLocked;
    
    private Dictionary<CharacterStateType, CharacterBaseState> m_States;
    
    // for debug ui
    public int CurrentComboStep;
    public float ComboTimer;
    public bool NextComboQueued;

    private void Awake()
    {
        m_AnimController = GetComponent<CharacterAnimationController>();
        m_PlayerController = GetComponent<PlayerController>();

        m_States = new Dictionary<CharacterStateType, CharacterBaseState>
        {
            { CharacterStateType.Idle,      new CharacterIdleState(this, m_AnimController) },
            { CharacterStateType.Moving,    new CharacterMovingState(this, m_AnimController) },
            { CharacterStateType.Attacking, new CharacterAttackingState(this, m_AnimController) },
        };
    }

    private void OnEnable()
    {
        CurrentStateType = CharacterStateType.Idle;
        m_CurrentBaseState = m_States[CurrentStateType];
    }

    private void Update()
    {   
        m_CurrentBaseState?.OnUpdate();
    }

    public void ChangeState(CharacterStateType newState)
    {
        if (CurrentStateType == newState) return;
        m_CurrentBaseState?.OnExit();
        CurrentStateType = newState;
        m_CurrentBaseState = m_States[newState];
        m_CurrentBaseState.OnEnter();
    }

    public void CheckStateTransition(TransitionType allowedTransitions)
    {
        var inputManager = GameManager.Instance.InputManager;
        if (allowedTransitions.HasFlag(TransitionType.Attack))
        {
            if (inputManager.IsLightAttackInput || inputManager.IsHeavyAttackInput)
            {
                ChangeState(CharacterStateType.Attacking);
                return;
            }
        }
        
        if (allowedTransitions.HasFlag(TransitionType.Move) && IsMoving())
        {
            ChangeState(CharacterStateType.Moving);
            return;
        }
        
        if ((allowedTransitions.HasFlag(TransitionType.Idle) && !IsMoving()))
        {
            ChangeState(CharacterStateType.Idle);
            return;
        }
    }
    
    public void OnTakeHit(float damage, Vector3 hitPoint, Vector3 hitNormal, float stunTime = 0.25f, float knockbackForce = 2.0f)
    {
        var hitState = new CharacterAttackedState(this, m_AnimController);
        m_States[CharacterStateType.Attacked] = hitState;

        ChangeState(CharacterStateType.Attacked);
    }


    public AttackComboInfo[] GetComboDatas(ComboType comboType) => m_AttackComboDatas.GetComboData(comboType).GetAttackComboInfos();

    public bool IsMoving()
    {
        if (m_MoveLocked) return false;
        return m_PlayerController.HasMoveInput();
    }
    
    public void SetInputLocked(bool locked)  { m_InputLocked = locked; }
    public bool IsInputLocked() => m_InputLocked;
    
    public void SetMoveLocked(bool locked)   { m_MoveLocked = locked;  }
    public bool IsMoveLocked()  => m_MoveLocked;
    
    public PlayerController GetPlayerController() => m_PlayerController;

    private void OnDisable() { }
}