using UnityEngine;

public class CharacterMovingState : CharacterBaseState
{
    public CharacterMovingState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 카메라 기준 이동 방향 변환
        Vector2 moveInput = GameManager.Instance.InputManager.MoveDirection;
        
        CameraManager cam = GameManager.Instance.CameraManager;
        Transform camTransform = cam.transform;
        
        Vector3 camForward = Vector3.Scale(camTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = camTransform.right;

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir = moveDir.normalized;

        // 애니메이션
        m_AnimController.PlayWalking(moveInput);

        // 실제 이동
        var walkSpeed = m_StateController.WalkSpeed * Time.deltaTime;
        m_StateController.transform.position += moveDir * walkSpeed;

        float rotateSpeed = m_StateController.RotationSpeed * Time.deltaTime;

        // LockOn 여부에 따라 회전 방식 분기
        if (GameManager.Instance.InputManager.IsLockOnTarget && cam.LockOnTarget != null)
        {
            // 록온: 적 방향으로만 회전
            Vector3 lookDir = (cam.LockOnTarget.position - m_StateController.transform.position).normalized;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                m_StateController.transform.rotation = Quaternion.Slerp(m_StateController.transform.rotation, targetRotation, rotateSpeed);
            }
        }
        else
        {
            // 평소: 이동 입력 방향으로 회전
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                m_StateController.transform.rotation = Quaternion.Slerp(m_StateController.transform.rotation, targetRotation, rotateSpeed);
            }
        }

        m_StateController.CheckStateTransition(TransitionType.Idle | TransitionType.Attack);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}