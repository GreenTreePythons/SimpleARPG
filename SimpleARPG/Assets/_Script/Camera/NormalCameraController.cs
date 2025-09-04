using UnityEngine;
using UnityEngine.Serialization;

public class NormalCameraController : MonoBehaviour, ICameraMode
{
    [SerializeField] Vector3 Offset = new Vector3(0, 2, -5); // 기준 거리
    [SerializeField] private float m_MouseSensitivity = 3.0f;
    [SerializeField] private float m_MinPitch = -30f;
    [SerializeField] private float m_MaxPitch = 70f;

    private float m_Yaw = 0f;   // 좌우 회전
    private float m_Pitch = 20f; // 상하 회전
    private bool m_IsActive = false;
    private Transform m_PlayerTransform;

    public void SetPlayer(Transform playerTrans)
    {
        m_PlayerTransform = playerTrans;
    }

    public void Enable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_IsActive = true;
    }

    public void Disable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_IsActive = false;
    }

    public void UpdateCamera()
    {
        if (m_PlayerTransform == null || !m_IsActive) return;

        Vector2 lookDelta = GameManager.Instance.InputManager.LookDirection;
        m_Yaw += lookDelta.x * m_MouseSensitivity;
        m_Pitch -= lookDelta.y * m_MouseSensitivity;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        Quaternion rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0);
        Vector3 desiredPosition = m_PlayerTransform.position + rotation * Offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.15f);
        transform.LookAt(m_PlayerTransform.position + Vector3.up * 1.2f);
    }
}