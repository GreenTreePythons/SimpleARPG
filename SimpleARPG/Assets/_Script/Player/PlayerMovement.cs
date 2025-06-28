using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float RotationSpeed = 700f;

    private CharacterController characterController;
    private Vector2 moveInput;
    Animator m_Animator;

    private InputSystemActions playerControls;

    void Awake()
    {
        playerControls = new InputSystemActions();
        m_Animator = this.GetComponent<Animator>();
    }

    void OnEnable()
    {
        // binding actions
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Attack.performed += OnAttackPerformed;

        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isMoving = moveInput.x != 0f || moveInput.y != 0f;
        m_Animator.SetBool("IsMoving", isMoving);

        float moveDirection = 0f;

        if (moveInput.y > 0) // forward
        {
            moveDirection = 1f;
        }
        else if (moveInput.y < 0) // back
        {
            moveDirection = -1f;
        }
        else if (moveInput.x > 0) // right
        {
            moveDirection = 2f;
        }
        else if (moveInput.x < 0) // left
        {
            moveDirection = -2f;
        }

        m_Animator.SetFloat("MovingDirection", moveDirection);
    }
    
    // input move
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // input attack
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        m_Animator.SetTrigger("Attack");
    }
}
