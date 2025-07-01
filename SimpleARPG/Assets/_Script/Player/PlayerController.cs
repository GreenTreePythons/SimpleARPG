using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    [SerializeField] Transform m_PlayerRoot;
    [SerializeField] CinemachineCamera m_NormalCamera;
    [SerializeField] CinemachineCamera m_LockOnCamera;

    private InputSystemActions m_InputActions;
    private CharacterStateController m_StateController;
    private CharacterAttackController m_AttackController;

    private Vector2 m_MoveInput = Vector2.zero;
    private bool m_ParryPressed = false;
    private bool m_BlockPressed = false;
    private bool m_IsLockOnTarget = false;
    private Coroutine m_CoLockOnRotate;

    protected override void Awake()
    {
        base.Awake();
        m_InputActions = new InputSystemActions();
        m_StateController = GetComponent<CharacterStateController>();
        m_AttackController = GetComponent<CharacterAttackController>();
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

    private CinemachineCamera GetCurrentCamera() => m_IsLockOnTarget ? m_LockOnCamera : m_NormalCamera;

    private void OnEnable()
    {
        m_InputActions.Player.Move.performed += OnMovePerformed;
        m_InputActions.Player.Move.canceled += OnMoveCanceled;

        m_InputActions.Player.LightAttack.performed += OnLightAtackPerformed;
        // m_InputActions.Player.HeavyAttack.performed += OnHeavyAtackPerformed;

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
        m_AttackController.EnqueueAttackInput(AttackType.Light);
    }

    private void OnHeavyAtackPerformed(InputAction.CallbackContext context)
    {
        m_AttackController.EnqueueAttackInput(AttackType.Heavy);
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
        m_StateController.CharacterAnimator.SetBool("IsLockOnTarget", m_IsLockOnTarget);
        
        if (m_IsLockOnTarget)
        {
            m_LockOnCamera.Priority = 20;
            m_NormalCamera.Priority = 10;

            // if (m_CoLockOnRotate != null) StopCoroutine(m_CoLockOnRotate);
            // m_CoLockOnRotate = StartCoroutine(LockOnRotateCoroutine(m_EnemyDetector.GetNearestEnemy()));
        }
        else
        {
            m_LockOnCamera.Priority = 10;
            m_NormalCamera.Priority = 20;

            // if (m_CoLockOnRotate != null) StopCoroutine(m_CoLockOnRotate);
        }
    }

    private IEnumerator LockOnRotateCoroutine(CharacterController enemy)
    {
        if (enemy == null) yield break;

        Quaternion startRotation = transform.rotation;
        Vector3 lookDir = enemy.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);

        while (m_IsLockOnTarget)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation;
        m_CoLockOnRotate = null;
    }

    public override bool IsEnemy(CharacterController other) => other is AIController;

    private void ApplyInput()
    {
        m_StateController.SetInput(
            m_MoveInput,
            false, // 공격 입력은 큐에서 관리하므로 false로
            m_ParryPressed,
            m_BlockPressed
        );
    }
}
