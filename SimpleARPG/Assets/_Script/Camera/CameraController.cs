using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetTransfom;
    public Transform CameraTransform;
    public Transform CameraPivotTransfom;

    private Transform m_MyTransform;
    private Vector3 m_CameraFransformPos;

    public static CameraController Instance;

    public float LookSpeed = 0.1f;
    public float FollowSpeed = 0.1f;
    public float PivotSpeed = 0.03f;

    private float m_DefaultPosZ;
    private float m_LookAngle;
    private float m_PivotAngle;

    public float MinimumPivot = -35;
    public float MaximumPivot = 35;

    private void Awake()
    {
        Instance = this;
        m_MyTransform = this.transform;
        m_DefaultPosZ = CameraTransform.localPosition.z;
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPos = Vector3.Lerp(m_MyTransform.position, TargetTransfom.position, delta / FollowSpeed);
        m_MyTransform.position = targetPos;
    }

    public void HandleCameraRotation(float delta, float mouseX, float mouseY)
    {
        m_LookAngle += (mouseX * LookSpeed) / delta;
        m_PivotAngle -= (mouseY * PivotSpeed) / delta;
        m_PivotAngle = Mathf.Clamp(m_PivotAngle, MinimumPivot, MaximumPivot);

        Vector3 rot = Vector3.zero;
        rot.y = m_LookAngle;
        Quaternion targetRot = Quaternion.Euler(rot);
        m_MyTransform.rotation = targetRot;

        rot = Vector3.zero;
        rot.x = m_PivotAngle;

        targetRot = Quaternion.Euler(rot);
        CameraPivotTransfom.localRotation = targetRot;
    }
}