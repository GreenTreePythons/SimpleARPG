using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterWeaponIKController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool m_DebugManualMode = false;     // 수동 조정 모드
    [SerializeField, Range(0f,1f)] private float m_DebugSheath = 1f;
    [SerializeField, Range(0f,1f)] private float m_DebugHand   = 0f;
#endif
    
    [Header("Rig & Constraint")]
    [SerializeField] private MultiParentConstraint m_WeaponParent;
    [SerializeField] private MultiParentConstraint m_WeaponSheathParent;
    [SerializeField] private Rig m_EquipRig;                      

    [Header("Blend")]
    [SerializeField] private float m_BlendSpeed = 12f;            

    [Header("Optional")]
    [SerializeField] private GameObject[] m_EnableWhileEquipped;
    [SerializeField] private GameObject[] m_DisableWhileEquipped;

    private float m_TargetSheath = 1f;
    private float m_TargetHand = 0f;
    private float m_Sheath = 1f;
    private float m_Hand = 0f;
    private float m_TargetIk = 0f;

    private void Awake()
    {
        if (m_WeaponParent != null) m_WeaponParent.weight = 1f;
        if(m_WeaponSheathParent != null) m_WeaponSheathParent.weight = 1f;
        
        ApplySourceWeights(m_WeaponParent, 1f, 0f);
        
        ApplyToggleByEquipped(false);
        if (m_EquipRig != null) m_EquipRig.weight = 0f;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (m_DebugManualMode)
        {
            ApplySourceWeights(m_WeaponParent, m_DebugSheath, m_DebugHand);
            return;
        }
#endif
        
        // 소스 가중치 보간
        m_Sheath = Mathf.MoveTowards(m_Sheath, m_TargetSheath, Time.deltaTime * m_BlendSpeed);
        m_Hand   = Mathf.MoveTowards(m_Hand,   m_TargetHand,   Time.deltaTime * m_BlendSpeed);
        
        ApplySourceWeights(m_WeaponParent, m_Sheath, m_Hand);

        // 손 IK 보간
        if (m_EquipRig != null && Mathf.Abs(m_EquipRig.weight - m_TargetIk) > 0.0001f)
            m_EquipRig.weight = Mathf.MoveTowards(m_EquipRig.weight, m_TargetIk, Time.deltaTime * m_BlendSpeed);

        // 토글(전환이 거의 끝났을 때)
        if (Mathf.Abs(m_Sheath - m_TargetSheath) < 0.001f && Mathf.Abs(m_EquipRig.weight - m_TargetIk) < 0.001f)
            ApplyToggleByEquipped(m_TargetHand >= 0.99f);
    }

    public void OnEquipSwitch()   { m_TargetSheath = 0f; m_TargetHand = 1f; m_TargetIk = 1f; }
    
    public void OnUnequipSwitch() { m_TargetSheath = 1f; m_TargetHand = 0f; m_TargetIk = 0f; }

    public void ForceImmediateState(bool equipped)
    {
        m_TargetSheath = equipped ? 0f : 1f;
        m_TargetHand   = equipped ? 1f : 0f;
        m_Sheath = m_TargetSheath; m_Hand = m_TargetHand;
        ApplySourceWeights(m_WeaponParent, m_Sheath, m_Hand);
        
        if (m_EquipRig != null) m_EquipRig.weight = equipped ? 1f : 0f;
        ApplyToggleByEquipped(equipped);
    }

    private void ApplySourceWeights(MultiParentConstraint mpc, float from, float to)
    {
        if(mpc == null) return;
        var data = mpc.data;
        var srcs = data.sourceObjects;
        srcs.SetWeight(0, from);
        srcs.SetWeight(1, to);
        data.sourceObjects = srcs;
        m_WeaponParent.data = data;
    }

    private void ApplyToggleByEquipped(bool equipped)
    {
        if (m_EnableWhileEquipped != null)
            for (int i = 0; i < m_EnableWhileEquipped.Length; i++)
                if (m_EnableWhileEquipped[i] != null) m_EnableWhileEquipped[i].SetActive(equipped);

        if (m_DisableWhileEquipped != null)
            for (int i = 0; i < m_DisableWhileEquipped.Length; i++)
                if (m_DisableWhileEquipped[i] != null) m_DisableWhileEquipped[i].SetActive(!equipped);
    }
}