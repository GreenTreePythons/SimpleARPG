using UnityEngine;

public class SwordController : MonoBehaviour
{
    [Header("Source Collider")]
    [SerializeField] private CapsuleCollider m_SwordCollider;

    [Header("Spawn Rule")]
    [SerializeField] private float m_SpawnEveryMeters = 0.05f;
    [SerializeField] private float m_LifetimeSeconds = 0.25f;

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

    private void GetCapsuleWorld(CapsuleCollider col, out Vector3 centerWs, out Vector3 axisDirWs, out float radiusWs, out float halfLine)
    {
        Transform t = col.transform;

        Vector3 centerLs = col.center;
        float radius = col.radius;
        float height = Mathf.Max(col.height, radius * 2f);
        int dir = col.direction; 

        Vector3 lossy = t.lossyScale;
        float sx = Mathf.Abs(lossy.x);
        float sy = Mathf.Abs(lossy.y);
        float sz = Mathf.Abs(lossy.z);

        radiusWs = radius * (dir == 0 ? Mathf.Max(sy, sz) : (dir == 1 ? Mathf.Max(sx, sz) : Mathf.Max(sx, sy)));

        float axisScale = (dir == 0 ? sx : (dir == 1 ? sy : sz));
        float heightWs = height * axisScale;

        halfLine = Mathf.Max(0f, (heightWs * 0.5f) - radiusWs);

        centerWs = t.TransformPoint(centerLs);

        Vector3 axisLocal =
            (dir == 0) ? Vector3.right :
            (dir == 1) ? Vector3.up    : Vector3.forward;

        axisDirWs = (t.TransformDirection(axisLocal)).normalized;
    }

    private void SpawnCapsuleMarker(Vector3 from, Vector3 to, float radiusWs)
    {
        Vector3 mid = (from + to) * 0.5f;
        Vector3 dir = to - from;
        float length = Mathf.Max(0.0001f, dir.magnitude);

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "SwordPathCapsule";

        int layer = LayerMask.NameToLayer(m_DebugLayerName);
        if (layer >= 0) go.layer = layer;

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        go.transform.SetPositionAndRotation(mid, rot);

        const float baseR = 0.5f;
        const float baseH = 2f;
        go.transform.localScale = new Vector3(
            radiusWs / baseR,                
            (length + 2f * radiusWs) / baseH,
            radiusWs / baseR                 
        );

        var col = go.GetComponent<CapsuleCollider>();
        col.isTrigger = true; 

        var rend = go.GetComponent<Renderer>();
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;
        var mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        mat.color = m_MarkerColor;
        rend.sharedMaterial = mat;

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
