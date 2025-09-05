using UnityEngine;

/// 검의 CapsuleCollider를 기준으로 이동 경로에 '보이는' 캡슐 트리거를 간단히 생성
/// - 프리미티브 캡슐(메쉬+Trigger) 사용 → 눈으로 바로 확인
/// - 콜라이더의 월드 반경/높이/축을 반영
public class SimpleSwordPathColliders_UsingCollider : MonoBehaviour
{
    [Header("Source Collider")]
    [SerializeField] private CapsuleCollider m_SwordCollider;   // ★ 여기를 할당 (검에 붙은 CapsuleCollider)

    [Header("Spawn Rule")]
    [SerializeField] private float m_SpawnEveryMeters = 0.05f;  // 이 거리만큼 움직일 때마다 1개 생성
    [SerializeField] private float m_LifetimeSeconds = 0.25f;   // 자동 파괴 시간

    [Header("Visuals")]
    [SerializeField] private Color m_MarkerColor = new Color(0, 1, 1, 0.35f);
    [SerializeField] private string m_DebugLayerName = "Ignore Raycast";

    // prev-center
    private Vector3 m_PrevCenterWs;
    private bool m_HasPrev;

    private void OnEnable()
    {
        m_HasPrev = false;
    }

    private void LateUpdate()
    {
        if (m_SwordCollider == null) return;

        // 현재 CapsuleCollider의 월드 중심/반경/축방향 구하기
        GetCapsuleWorld(m_SwordCollider, out Vector3 centerWs, out Vector3 axisDirWs, out float radiusWs, out float halfLine);

        if (!m_HasPrev)
        {
            m_PrevCenterWs = centerWs;
            m_HasPrev = true;
            return;
        }

        Vector3 delta = centerWs - m_PrevCenterWs;
        float dist = delta.magnitude;
        if (dist < m_SpawnEveryMeters) return;

        int steps = Mathf.Max(1, Mathf.FloorToInt(dist / m_SpawnEveryMeters));
        Vector3 stepDir = delta.normalized;

        Vector3 from = m_PrevCenterWs;
        for (int i = 0; i < steps; i++)
        {
            Vector3 to = (i == steps - 1) ? centerWs : from + stepDir * m_SpawnEveryMeters;
            SpawnCapsuleMarker(from, to, radiusWs);
            from = to;
        }

        m_PrevCenterWs = centerWs;
    }

    // == Helpers ==

    /// CapsuleCollider의 월드 속성 계산
    private void GetCapsuleWorld(CapsuleCollider col, out Vector3 centerWs, out Vector3 axisDirWs, out float radiusWs, out float halfLine)
    {
        Transform t = col.transform;

        // 콜라이더 로컬 기준값
        Vector3 centerLs = col.center;
        float radius = col.radius;
        float height = Mathf.Max(col.height, radius * 2f);
        int dir = col.direction; // 0=X, 1=Y, 2=Z (로컬축)

        // 월드 변환 및 스케일 반영
        Vector3 lossy = t.lossyScale;
        float sx = Mathf.Abs(lossy.x);
        float sy = Mathf.Abs(lossy.y);
        float sz = Mathf.Abs(lossy.z);

        // 반경은 콜라이더 축을 제외한 두 축 중 최대 스케일을 적용 (Unity의 캡슐 스케일 규칙 근사)
        radiusWs = radius * (dir == 0 ? Mathf.Max(sy, sz) : (dir == 1 ? Mathf.Max(sx, sz) : Mathf.Max(sx, sy)));

        // 높이는 해당 축 스케일을 적용
        float axisScale = (dir == 0 ? sx : (dir == 1 ? sy : sz));
        float heightWs = height * axisScale;

        // 선분 절반 길이(헤미스피어 제외 실제 ‘원기둥’ 라인 길이 절반)
        halfLine = Mathf.Max(0f, (heightWs * 0.5f) - radiusWs);

        // 월드 중심
        centerWs = t.TransformPoint(centerLs);

        // 월드 축 방향
        Vector3 axisLocal =
            (dir == 0) ? Vector3.right :
            (dir == 1) ? Vector3.up    : Vector3.forward;

        axisDirWs = (t.TransformDirection(axisLocal)).normalized;
    }

    /// from→to 구간에 '보이는' 캡슐 프리미티브 생성(Trigger)
    private void SpawnCapsuleMarker(Vector3 from, Vector3 to, float radiusWs)
    {
        Vector3 mid = (from + to) * 0.5f;
        Vector3 dir = to - from;
        float length = Mathf.Max(0.0001f, dir.magnitude);

        // 프리미티브 캡슐(메쉬+콜라이더)
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "SwordPathCapsule";

        int layer = LayerMask.NameToLayer(m_DebugLayerName);
        if (layer >= 0) go.layer = layer;

        // 회전: 프리미티브 캡슐은 'Y축 길이' 기준 → Y를 dir로 맞춤
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        go.transform.SetPositionAndRotation(mid, rot);

        // 스케일 맞추기 (프리미티브 기본: 반지름 0.5, 높이 2)
        const float baseR = 0.5f;
        const float baseH = 2f;
        go.transform.localScale = new Vector3(
            radiusWs / baseR,                // X
            (length + 2f * radiusWs) / baseH,// Y (전체 높이)
            radiusWs / baseR                 // Z
        );

        // 콜라이더 Trigger화
        var col = go.GetComponent<CapsuleCollider>();
        col.isTrigger = true; // direction=Y(1) 기본 유지

        // 머티리얼(투명)
        var rend = go.GetComponent<Renderer>();
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;
        var mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        mat.color = m_MarkerColor;
        rend.sharedMaterial = mat;

        // N초 후 파괴
        go.AddComponent<AutoDespawn>().Init(m_LifetimeSeconds);
    }
}

public class AutoDespawn : MonoBehaviour
{
    private float m_Life;

    public void Init(float seconds) { m_Life = Mathf.Max(0.01f, seconds); }

    private void Update()
    {
        m_Life -= Time.deltaTime;
        if (m_Life <= 0f) Destroy(gameObject);
    }
}
