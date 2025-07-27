using UnityEngine;

public interface ICameraMode
{
    void Enable();
    void Disable();
    void UpdateCamera();
}

[RequireComponent(typeof(NormalCameraController))]
[RequireComponent(typeof(LockOnCameraController))]
public class CameraManager : MonoBehaviour
{
    public Transform LockOnTarget { get; private set; }
    
    private NormalCameraController m_NormalCameraController;
    private LockOnCameraController m_LockOnCameraController;
    
    private ICameraMode m_CurrentCameraMode;

    private void Awake()
    {
        m_NormalCameraController = GetComponent<NormalCameraController>();
        m_LockOnCameraController = GetComponent<LockOnCameraController>();
    }
    
    private void Start()
    {
        SetCameraMode(m_NormalCameraController);
    }
    
    public void SetPlayer(Transform playerTrans)
    {
        m_NormalCameraController.SetPlayer(playerTrans);
        m_LockOnCameraController.SetPlayer(playerTrans);
    }

    public void SetLockOnTarget(Transform enemyTrans)
    {
        LockOnTarget = enemyTrans;
        m_LockOnCameraController.SetTarget(enemyTrans);
    }

    public void SwitchToNormalCamera()
    {
        LockOnTarget = null;
        SetCameraMode(m_NormalCameraController);
    }

    public void SwitchToLockOnCamera()
    {
        SetCameraMode(m_LockOnCameraController);
    }

    private void SetCameraMode(ICameraMode mode)
    {
        if (m_CurrentCameraMode == mode) return;
        m_CurrentCameraMode?.Disable();
        m_CurrentCameraMode = mode;
        m_CurrentCameraMode?.Enable();
    }

    private void LateUpdate()
    {
        m_CurrentCameraMode?.UpdateCamera();
    }
}
