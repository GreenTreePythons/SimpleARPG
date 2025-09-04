
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AttackComboDatas", menuName = "ScriptableObject/AttackComboDatas")]
public class AttackComboDatas : ScriptableObject
{
    [SerializeField] AttackComboData[] ComboDatas;
    
    private Dictionary<ComboType, AttackComboData> m_ComboDatas = new();
    
    private void OnEnable()
    {
        if (ComboDatas == null) return;
        foreach (var data in ComboDatas)
        {
            m_ComboDatas[data.GetComboType()] = data;
        }
    }
    
    public AttackComboData GetComboData(ComboType type)
    {
        if (m_ComboDatas.TryGetValue(type, out var data)) return data;
        Debug.LogWarning($"ComboData for {type} not found!");
        return null;
    }
}

[System.Serializable]
public class AttackComboData
{
    [SerializeField] ComboType ComboType;
    [SerializeField] AttackComboInfo[] AttackComboInfos;

    public AttackComboInfo GetAttackComboInfo(int step)
    {
        if (step >= 0 && step < AttackComboInfos.Length) return AttackComboInfos[step];
        Debug.LogWarning($"AttackComboData: step {step} out of range!");
        return null;
    }
    public AttackComboInfo[] GetAttackComboInfos() => AttackComboInfos;
    public int GetMaxComboCount() => AttackComboInfos.Length;
    public ComboType GetComboType() => ComboType;
}

[System.Serializable]
public class AttackComboInfo
{
    [SerializeField] public string AnimStateName;
    [SerializeField] public float ComboValidStartTime;
    [SerializeField] public float ComboValidTime;    
    [SerializeField] public float Damage;
}