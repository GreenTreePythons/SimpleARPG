using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public Animator animator;

    private CharacterController characterController;
    private Vector2 moveInput;

    private InputSystemActions playerControls;

    void Awake()
    {
        // PlayerControls 액션 인스턴스 생성
        playerControls = new InputSystemActions();
    }

    void OnEnable()
    {
        // 이동 액션을 바인딩
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;

        // 공격 액션을 바인딩
        playerControls.Player.Attack.performed += OnAttackPerformed;

        // Input 활성화
        playerControls.Enable();
    }

    void OnDisable()
    {
        // 입력 해제
        playerControls.Disable();
    }

    // 이동 입력 처리
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero; // 이동이 멈추면 입력을 초기화
    }

    // 공격 입력 처리
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack");
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 이동 처리
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // 회전 처리
        if (move.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        // 애니메이션 처리
        animator.SetBool("IsMoving", move.magnitude > 0.1f);
    }
}
