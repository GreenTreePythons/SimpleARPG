using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

#if UNITY_EDITOR
public class DrawVector : MonoBehaviour
{
    [SerializeField] private bool m_ShowVector = false;
    [SerializeField] private Color m_VectorXColor = Color.red;
    [SerializeField] private Color m_VectorYColor = Color.green;
    [SerializeField] private Color m_VectorZColor = Color.blue;
    [SerializeField] private float m_VectorLength = 0.2f;
    [SerializeField] private float m_VectorArrowHeadLength = 0.1f;
    [SerializeField] private float m_VectorArrowHeadAngle = 20.0f;
    [SerializeField] private float m_VectorLineThickness = 10.0f;

    private void OnDrawGizmos()
    {
        if (!m_ShowVector) return;
        
        // X축 (빨강)
        DrawArrow(transform.position, transform.right, m_VectorXColor);

        // Y축 (초록)
        DrawArrow(transform.position, transform.up, m_VectorYColor);

        // Z축 (파랑)
        DrawArrow(transform.position, transform.forward, m_VectorZColor);
    }

    private void DrawArrow(Vector3 pos, Vector3 direction, Color color)
    {
        Vector3 end = pos + direction.normalized * m_VectorLength;

        Handles.color = color;
        Handles.DrawAAPolyLine(m_VectorLineThickness, pos, end);

        // 화살표 머리
        Vector3 right = Quaternion.LookRotation(direction) *
                        Quaternion.Euler(0, 180 + m_VectorArrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) *
                       Quaternion.Euler(0, 180 - m_VectorArrowHeadAngle, 0) * Vector3.forward;

        Handles.DrawAAPolyLine(m_VectorLineThickness, end, end + right * m_VectorArrowHeadLength);
        Handles.DrawAAPolyLine(m_VectorLineThickness, end, end + left * m_VectorArrowHeadLength);
    }
}
#endif