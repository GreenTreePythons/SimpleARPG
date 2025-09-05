using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SwordController : MonoBehaviour
{
    [Header("Blade Points (root -> tip)")]
    public Transform[] BladePoints;

    [Header("Gizmo Settings")]
    public bool DrawWhileIdle = true;              
    public bool DrawSweptCapsules = true;          
    public Color CurrentCapsuleColor = new Color(0.2f, 0.8f, 1f, 0.5f);
    public Color SweptCapsuleColor = new Color(1f, 0.6f, 0.2f, 0.35f);
    public float Radius = 0.03f;

    [Header("Runtime Debug Colliders (Optional)")]
    public bool SpawnDebugTriggerColliders = false; 
    public string DebugLayerName = "Ignore Raycast"; 
    public float DespawnSeconds = 0.2f;              

    private Vector3[] m_PrevPoints;
    private List<(Vector3 a, Vector3 b)> m_SweptSegments = new(); 
    private bool m_InitedPrev;
    private float m_TimeAccumulator;

    private void OnEnable()
    {
        EnsurePrevBuffer();
        CachePrevPoints();
    }

    private void LateUpdate()
    {
        if (BladePoints == null || BladePoints.Length < 2) return;

        m_SweptSegments.Clear();
        for (int i = 0; i < BladePoints.Length - 1; i++)
        {
            Vector3 prevA = m_PrevPoints[i];
            Vector3 prevB = m_PrevPoints[i + 1];
            Vector3 currA = BladePoints[i].position;
            Vector3 currB = BladePoints[i + 1].position;

            Vector3 from = (prevA + prevB) * 0.5f;
            Vector3 to   = (currA + currB) * 0.5f;

            m_SweptSegments.Add((from, to));

            if (SpawnDebugTriggerColliders && Application.isPlaying)
            {
                SpawnTempCapsuleTrigger(from, to, Radius);
            }
        }

        for (int i = 0; i < BladePoints.Length; i++)
            m_PrevPoints[i] = BladePoints[i].position;

        m_TimeAccumulator += Time.deltaTime;
        if (m_TimeAccumulator > 3f)
        {
            m_TimeAccumulator = 0f;
        }
    }

    private void EnsurePrevBuffer()
    {
        if (BladePoints == null) return;
        if (m_PrevPoints == null || m_PrevPoints.Length != BladePoints.Length)
            m_PrevPoints = new Vector3[BladePoints.Length];
    }

    private void CachePrevPoints()
    {
        if (BladePoints == null) return;
        for (int i = 0; i < BladePoints.Length; i++)
            m_PrevPoints[i] = BladePoints[i] ? BladePoints[i].position : transform.position;
        m_InitedPrev = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (BladePoints == null || BladePoints.Length < 2) return;

        if (!m_InitedPrev) { EnsurePrevBuffer(); CachePrevPoints(); }

        Gizmos.color = CurrentCapsuleColor;
        for (int i = 0; i < BladePoints.Length - 1; i++)
        {
            if (!BladePoints[i] || !BladePoints[i + 1]) continue;
            DrawCapsuleGizmo(BladePoints[i].position, BladePoints[i + 1].position, Radius, DrawWhileIdle);
        }

        if (DrawSweptCapsules && m_SweptSegments.Count > 0)
        {
            Gizmos.color = SweptCapsuleColor;
            for (int i = 0; i < m_SweptSegments.Count; i++)
            {
                var seg = m_SweptSegments[i];
                DrawCapsuleGizmo(seg.a, seg.b, Radius, true);
            }
        }
    }

    private void DrawCapsuleGizmo(Vector3 a, Vector3 b, float r, bool draw)
    {
        if (!draw) return;

        Gizmos.DrawSphere(a, r);
        Gizmos.DrawSphere(b, r);
        
        Gizmos.DrawLine(a + Vector3.right * r, b + Vector3.right * r);
        Gizmos.DrawLine(a - Vector3.right * r, b - Vector3.right * r);
        Gizmos.DrawLine(a + Vector3.up * r,    b + Vector3.up * r);
        Gizmos.DrawLine(a - Vector3.up * r,    b - Vector3.up * r);
        Gizmos.DrawLine(a + Vector3.forward * r, b + Vector3.forward * r);
        Gizmos.DrawLine(a - Vector3.forward * r, b - Vector3.forward * r);
    }
#endif

    private void SpawnTempCapsuleTrigger(Vector3 from, Vector3 to, float r)
    {
        GameObject go = new GameObject("DebugCapsuleTrigger");
        int layer = LayerMask.NameToLayer(DebugLayerName);
        if (layer >= 0) go.layer = layer;
        go.transform.position = (from + to) * 0.5f;

        Vector3 dir = to - from;
        float length = dir.magnitude;
        Vector3 axis = (length > 0.0001f) ? dir.normalized : Vector3.forward;

        var col = go.AddComponent<CapsuleCollider>();
        col.isTrigger = true;
        col.direction = 2;            
        col.radius = r;
        col.height = Mathf.Max(r * 2f, length + r * 2f);

        go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, axis);

        var selfDestruct = go.AddComponent<TempDebugDespawn>();
        selfDestruct.Initialize(DespawnSeconds);
    }
}

public class TempDebugDespawn : MonoBehaviour
{
    private float m_Life;
    private float m_Elapsed;

    public void Initialize(float seconds)
    {
        m_Life = Mathf.Max(0.01f, seconds);
        m_Elapsed = 0f;
    }

    private void Update()
    {
        m_Elapsed += Time.deltaTime;
        if (m_Elapsed >= m_Life)
        {
            Destroy(gameObject);
        }
    }
}
