using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystemActions m_InputActions;
    private CharacterActionController m_CharacterActionController;

    private void Awake()
    {
        m_InputActions = new InputSystemActions();
        m_CharacterActionController = GetComponent<CharacterActionController>();
    }

    void OnEnable()
    {
        m_InputActions.Player.Move.performed += OnMovePerformed;
        m_InputActions.Player.Move.canceled += OnMoveCanceled;
        m_InputActions.Player.Attack.performed += OnAttackPerformed;
        m_InputActions.Player.Parry.performed += OnParryPerformed;
        m_InputActions.Player.Block.performed += OnBlockPerformed;

        m_InputActions.Enable();
    }

    void OnDisable()
    {
        m_InputActions.Disable();
    }

    void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveVec = context.ReadValue<Vector2>();
        m_CharacterActionController.HandleInput(moveVec, false, false, false);
    }

    void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Vector2 moveVec = Vector2.zero;
        m_CharacterActionController.HandleInput(moveVec, m_CharacterActionController.AttackPressed, m_CharacterActionController.ParryPressed, m_CharacterActionController.BlockPressed);
    }

    void OnAttackPerformed(InputAction.CallbackContext context)
    {
        m_CharacterActionController.HandleInput(m_CharacterActionController.MoveInput, true, false, false);
    }

    void OnParryPerformed(InputAction.CallbackContext context)
    {
        m_CharacterActionController.HandleInput(m_CharacterActionController.MoveInput, false, true, false);
    }

    void OnBlockPerformed(InputAction.CallbackContext context)
    {
        m_CharacterActionController.HandleInput(m_CharacterActionController.MoveInput, false, false, true);
    }
}
