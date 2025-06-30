using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    [SerializeField] Transform m_PlayerRoot;
    [SerializeField] private CinemachineCamera m_VcamNormal;
    [SerializeField] private CinemachineCamera m_VcamLockOnTarget;

    private InputSystemActions m_InputActions;
    private CharacterStateController m_CharacterStateController;
    private CharacterAttackController m_CharacterAttackController;

    private Vector2 m_MoveInput = Vector2.zero;
    private bool m_ParryPressed = false;
    private bool m_BlockPressed = false;
    private bool m_IsLockOnTarget = false;

    protected override void Awake()
    {
        base.Awake();
        m_InputActions = new InputSystemActions();
        m_CharacterStateController = GetComponent<CharacterStateController>();
        m_CharacterAttackController = GetComponent<CharacterAttackController>();
    }

    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (m_MoveInput.magnitude <= 0.01f) return;

        Vector3 inputDirection = new Vector3(m_MoveInput.x, 0, m_MoveInput.y).normalized;
        var moveSpeed = m_IsLockOnTarget ? m_BattleMoveSpeed : m_NormalMoveSpeed;

        if (m_IsLockOnTarget)
        {
            m_PlayerRoot.position += inputDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            var rotateSpeed = 12.0f;
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            m_PlayerRoot.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        m_InputActions.Player.Move.performed += OnMovePerformed;
        m_InputActions.Player.Move.canceled += OnMoveCanceled;

        m_InputActions.Player.LightAttack.performed += OnLightAtackPerformed;
        m_InputActions.Player.HeavyAttack.performed += OnHeavyAtackPerformed;

        m_InputActions.Player.Parry.performed += OnParryPerformed;
        m_InputActions.Player.Parry.canceled += OnParryCanceled;

        m_InputActions.Player.Block.performed += OnBlockPerformed;
        m_InputActions.Player.Block.canceled += OnBlockCanceled;

        m_InputActions.Player.LockOnTarget.performed += OnLockOnPerformed;

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

    private void OnLightAtackPerformed(InputAction.CallbackContext context)
    {
        m_CharacterAttackController.EnqueueAttackInput(AttackType.Light);
    }

    private void OnHeavyAtackPerformed(InputAction.CallbackContext context)
    {
        m_CharacterAttackController.EnqueueAttackInput(AttackType.Heavy);
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

    private void OnLockOnPerformed(InputAction.CallbackContext context)
    {
        m_IsLockOnTarget = !m_IsLockOnTarget;
        m_CharacterStateController.CharacterAnimator.SetBool("IsLockOnTarget", m_IsLockOnTarget);
        if (m_IsLockOnTarget)
        {
            m_VcamLockOnTarget.Priority = 20;
            m_VcamNormal.Priority = 10;
        }
        else
        {
            m_VcamLockOnTarget.Priority = 10;
            m_VcamNormal.Priority = 20;
        }
    }

    public override bool IsEnemy(CharacterController other) => other is AIController;

    private void ApplyInput()
    {
        m_CharacterStateController.SetInput(
            m_MoveInput,
            false, // 공격 입력은 큐에서 관리하므로 false로
            m_ParryPressed,
            m_BlockPressed
        );
    }
}
