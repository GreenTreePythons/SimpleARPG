using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SwordController : MonoBehaviour
{
    [Header("Blade Points (root -> tip)")]
    [Tooltip("칼날을 따라 배치한 포인트들. 2개 이상(루트/팁) 권장, 3~5개 추천")]
    public Transform[] bladePoints;

    [Header("Gizmo Settings")]
    public bool drawWhileIdle = true;              // 대기 중에도 현재 캡슐 표시
    public bool drawSweptCapsules = true;          // 이전→현재로 스윕된 궤적 표시
    public Color currentCapsuleColor = new Color(0.2f, 0.8f, 1f, 0.5f);
    public Color sweptCapsuleColor = new Color(1f, 0.6f, 0.2f, 0.35f);
    public float radius = 0.03f;

    [Header("Runtime Debug Colliders (Optional)")]
    public bool spawnDebugTriggerColliders = false; // 재생 중 궤적에 트리거 캡슐 생성(물리 무시 레이어 권장)
    public string debugLayerName = "Ignore Raycast"; // 생성될 오브젝트 레이어
    public float despawnSeconds = 0.2f;              // 생성 후 자동 파괴 시간

    // --- private ---
    private Vector3[] m_PrevPoints;
    private List<(Vector3 a, Vector3 b)> m_SweptSegments = new(); // 이전 프레임→현재 프레임 세그먼트
    private bool m_InitedPrev;
    private float m_TimeAccumulator;

    private void OnEnable()
    {
        EnsurePrevBuffer();
        CachePrevPoints();
    }

    private void LateUpdate()
    {
        if (bladePoints == null || bladePoints.Length < 2) return;

        // 이전 프레임→현재 프레임 세그먼트 축적(그림용)
        m_SweptSegments.Clear();
        for (int i = 0; i < bladePoints.Length - 1; i++)
        {
            Vector3 prevA = m_PrevPoints[i];
            Vector3 prevB = m_PrevPoints[i + 1];
            Vector3 currA = bladePoints[i].position;
            Vector3 currB = bladePoints[i + 1].position;

            // 현재 ‘세그먼트 중심’ 이동을 단순화해 표시
            // (시각화 목적: 두 세그먼트의 중점을 잇는 선을 캡슐처럼 본다)
            Vector3 from = (prevA + prevB) * 0.5f;
            Vector3 to   = (currA + currB) * 0.5f;

            m_SweptSegments.Add((from, to));

            if (spawnDebugTriggerColliders && Application.isPlaying)
                SpawnTempCapsuleTrigger(from, to, radius);
        }

        // 다음 프레임을 위해 현재 포인트 저장
        for (int i = 0; i < bladePoints.Length; i++)
            m_PrevPoints[i] = bladePoints[i].position;

        // 생존 시간 관리(런타임 트리거 캡슐은 개별 스크립트에서 파괴됨)
        m_TimeAccumulator += Time.deltaTime;
        if (m_TimeAccumulator > 3f) // 누적 관리가 필요 없다면 주기적으로 비워줘도 됨
        {
            m_TimeAccumulator = 0f;
        }
    }

    private void EnsurePrevBuffer()
    {
        if (bladePoints == null) return;
        if (m_PrevPoints == null || m_PrevPoints.Length != bladePoints.Length)
            m_PrevPoints = new Vector3[bladePoints.Length];
    }

    private void CachePrevPoints()
    {
        if (bladePoints == null) return;
        for (int i = 0; i < bladePoints.Length; i++)
            m_PrevPoints[i] = bladePoints[i] ? bladePoints[i].position : transform.position;
        m_InitedPrev = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (bladePoints == null || bladePoints.Length < 2) return;

        if (!m_InitedPrev) { EnsurePrevBuffer(); CachePrevPoints(); }

        // 1) 현재 프레임의 칼날 세그먼트 표시(정적 캡슐 느낌)
        Gizmos.color = currentCapsuleColor;
        for (int i = 0; i < bladePoints.Length - 1; i++)
        {
            if (!bladePoints[i] || !bladePoints[i + 1]) continue;
            DrawCapsuleGizmo(bladePoints[i].position, bladePoints[i + 1].position, radius, drawWhileIdle);
        }

        // 2) 스윕 궤적(이전→현재) 표시
        if (drawSweptCapsules && m_SweptSegments.Count > 0)
        {
            Gizmos.color = sweptCapsuleColor;
            for (int i = 0; i < m_SweptSegments.Count; i++)
            {
                var seg = m_SweptSegments[i];
                DrawCapsuleGizmo(seg.a, seg.b, radius, true);
            }
        }
    }

    private void DrawCapsuleGizmo(Vector3 a, Vector3 b, float r, bool draw)
    {
        if (!draw) return;

        // 간단한 캡슐 표현: 양 끝에 구, 가운데 원기둥(라인으로 근사)
        Gizmos.DrawSphere(a, r);
        Gizmos.DrawSphere(b, r);
        // 원기둥 대신 라인 여러 개로 근사
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
        // 디버그 전용: 물리에 영향 최소화(Trigger + 디버그 레이어)
        GameObject go = new GameObject("DebugCapsuleTrigger");
        int layer = LayerMask.NameToLayer(debugLayerName);
        if (layer >= 0) go.layer = layer;
        go.transform.position = (from + to) * 0.5f;

        // 길이/방향 세팅
        Vector3 dir = to - from;
        float length = dir.magnitude;
        Vector3 axis = (length > 0.0001f) ? dir.normalized : Vector3.forward;

        // 캡슐콜라이더 파라미터
        var col = go.AddComponent<CapsuleCollider>();
        col.isTrigger = true;
        col.direction = 2;            // Z축 기준
        col.radius = r;
        col.height = Mathf.Max(r * 2f, length + r * 2f);

        // 회전(Z축이 세그먼트 방향을 보도록)
        go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, axis);

        // 짧게 살았다 사라지게
        var selfDestruct = go.AddComponent<TempDebugDespawn>();
        selfDestruct.Initialize(despawnSeconds);
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
