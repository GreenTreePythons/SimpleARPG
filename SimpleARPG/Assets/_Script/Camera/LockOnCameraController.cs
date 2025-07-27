using UnityEngine;

public class LockOnCameraController : MonoBehaviour, ICameraMode
{
    [SerializeField] private Vector3 m_Offset = new Vector3(0, 2, -5); // (Y: 높이, Z: 뒤로)
    [SerializeField] private float m_LerpSpeed = 0.10f;

    private bool m_IsActive = false;
    private Transform m_PlayerTransform;
    private Transform m_TargetTransform;

    public void SetPlayer(Transform playerTrans) { m_PlayerTransform = playerTrans; }
    public void SetTarget(Transform targetTrans) { m_TargetTransform = targetTrans; }

    public void Enable()
    {
        m_IsActive = true;
        SnapToPlayerBackward();
    }

    public void Disable()
    {
        m_IsActive = false;
    }

    public void UpdateCamera()
    {
        if (m_PlayerTransform == null || m_TargetTransform == null || !m_IsActive) return;
        
        // 1. 플레이어가 바라보는 방향의 반대편(뒤쪽, -forward)으로 카메라 위치
        Vector3 desiredPosition = m_PlayerTransform.position
                                + (-m_PlayerTransform.forward) * Mathf.Abs(m_Offset.z)
                                + Vector3.up * m_Offset.y;
        
        Debug.Log($"pos : {m_PlayerTransform.position}");
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, m_LerpSpeed);

        // 2. Target(적)을 항상 바라보기
        Vector3 lookTarget = m_TargetTransform.position + Vector3.up * 1.0f;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_LerpSpeed);
    }

    private void SnapToPlayerBackward()
    {
        if (m_PlayerTransform == null || m_TargetTransform == null) return;

        Vector3 snapPosition = m_PlayerTransform.position
                             + (-m_PlayerTransform.forward) * Mathf.Abs(m_Offset.z)
                             + Vector3.up * m_Offset.y;

        transform.position = snapPosition;

        Vector3 lookTarget = m_TargetTransform.position + Vector3.up * 1.0f;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - snapPosition, Vector3.up);
        transform.rotation = targetRotation;
    }
}
