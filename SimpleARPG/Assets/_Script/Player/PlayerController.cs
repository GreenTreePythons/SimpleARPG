using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_LockOnTargetEnemy;
    [SerializeField] CameraManager m_CameraManager;

    private bool m_IsCameraModeUpdated = false;

    private void Start()
    {
        m_CameraManager.SetPlayer(this.transform); // 최초 1회만 세팅
        m_CameraManager.SwitchToNormalCamera();
    }

    private void Update()
    {
        HandleCameraSwitchInput();
        // (이동, 공격 등 다른 처리)
    }

    private void HandleCameraSwitchInput()
    {
        bool isLockOn = GameManager.Instance.InputManager.IsLockOnTarget;

        if (m_IsCameraModeUpdated != isLockOn)
        {
            if (isLockOn && m_LockOnTargetEnemy != null)
            {
                m_CameraManager.SetLockOnTarget(m_LockOnTargetEnemy);
                m_CameraManager.SwitchToLockOnCamera();
            }
            else
            {
                m_CameraManager.SwitchToNormalCamera();
            }
            m_IsCameraModeUpdated = isLockOn;
        }
    }
    
    public CameraManager GetCameraManager() => m_CameraManager;
}