using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    private InputSystemActions m_InputActions;
    private CharacterStateController m_CharacterStateController;

    private Vector2 m_MoveInput = Vector2.zero;
    private bool m_AttackPressed = false;
    private bool m_ParryPressed = false;
    private bool m_BlockPressed = false;

    protected override void Awake()
    {
        base.Awake();
        m_InputActions = new InputSystemActions();
        m_CharacterStateController = GetComponent<CharacterStateController>();
    }

    private void OnEnable()
    {
        m_InputActions.Player.Move.performed += OnMovePerformed;
        m_InputActions.Player.Move.canceled += OnMoveCanceled;

        m_InputActions.Player.Attack.performed += OnAttackPerformed;
        m_InputActions.Player.Attack.canceled += OnAttackCanceled;

        m_InputActions.Player.Parry.performed += OnParryPerformed;
        m_InputActions.Player.Parry.canceled += OnParryCanceled;

        m_InputActions.Player.Block.performed += OnBlockPerformed;
        m_InputActions.Player.Block.canceled += OnBlockCanceled;

        m_InputActions.Enable();
    }

    private void OnDisable()
    {
        m_InputActions.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
        ApplyInput();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        m_MoveInput = Vector2.zero;
        ApplyInput();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        m_AttackPressed = true;
        ApplyInput();
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        m_AttackPressed = false;
        ApplyInput();
    }

    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        m_ParryPressed = true;
        ApplyInput();
    }

    private void OnParryCanceled(InputAction.CallbackContext context)
    {
        m_ParryPressed = false;
        ApplyInput();
    }

    private void OnBlockPerformed(InputAction.CallbackContext context)
    {
        m_BlockPressed = true;
        ApplyInput();
    }

    private void OnBlockCanceled(InputAction.CallbackContext context)
    {
        m_BlockPressed = false;
        ApplyInput();
    }

    public override bool IsEnemy(CharacterController other) => other is AIController;

    /// <summary>
    /// 현재 입력값을 StateController에 전달
    /// </summary>
    private void ApplyInput()
    {
        m_CharacterStateController.SetInput(
            m_MoveInput,
            m_AttackPressed,
            m_ParryPressed,
            m_BlockPressed
        );
    }
}
