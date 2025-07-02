using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum CharacterStateType
{
    Idle,
    Moving,
    Attacking
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
    
    private CharacterStateBase m_CurrentState;
    private CharacterAnimationController m_AnimController;
    
    private Dictionary<CharacterStateType, CharacterStateBase> m_States;

    private void Awake()
    {
        m_AnimController = GetComponent<CharacterAnimationController>();

        m_States = new Dictionary<CharacterStateType, CharacterStateBase>
        {
            { CharacterStateType.Idle,      new CharacterIdleState(this, m_AnimController) },
            { CharacterStateType.Moving,    new CharacterMovingState(this, m_AnimController) },
            { CharacterStateType.Attacking, new CharacterAttackingState(this, m_AnimController) },
        };
    }

    private void OnEnable()
    {
        CurrentStateType = CharacterStateType.Idle;
        m_CurrentState = m_States[CurrentStateType];
    }

    private void Update()
    {   
        m_CurrentState?.Update();
    }

    public void ChangeState(CharacterStateType newState)
    {
        if (CurrentStateType == newState) return;
        m_CurrentState?.OnExit();
        CurrentStateType = newState;
        m_CurrentState = m_States[newState];
        m_CurrentState.OnEnter();
    }

    public void CheckStateTransition(TransitionType allowedTransitions)
    {
        var inputManager = GameManager.Instance.InputManager;
        if (allowedTransitions.HasFlag(TransitionType.Attack))
        {
            if (inputManager.IsLightAttackInput || inputManager.IsHeavyAttackInput)
                return;
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

    public AttackComboInfo[] GetComboDatas(ComboType comboType) => m_AttackComboDatas.GetComboData(comboType).GetAttackComboInfos();
    
    public bool IsMoving() => GameManager.Instance.InputManager.IsMoving;

    private void OnDisable() { }
}