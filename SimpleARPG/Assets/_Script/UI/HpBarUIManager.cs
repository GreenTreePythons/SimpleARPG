using System.Collections.Generic;
using UnityEngine;

public class HPBarUIManager : MonoBehaviour
{
    public static HPBarUIManager Instance { get; private set; }

    [SerializeField] private HPBarUI m_HpBarPrefab;

    private Dictionary<CharacterController, HPBarUI> m_HpBarMap = new();
    Canvas m_Canvas;

    void Awake()
    {
        Instance = this;
        m_Canvas = this.GetComponent<Canvas>();
    }

    public void Register(CharacterController target)
    {
        if (m_HpBarMap.ContainsKey(target)) return;

        var hpBar = Instantiate(m_HpBarPrefab, m_Canvas.transform);
        m_HpBarMap[target] = hpBar;
        hpBar.SetHP(target.GetCurrentHp(), target.GetStatValue(CharacterStat.HP));

        if (target is PlayerController)
            hpBar.SetPivot(1f);  // 오른쪽
        else
            hpBar.SetPivot(0f);  // 왼쪽
    }

    public void Unregister(CharacterController target)
    {
        if (!m_HpBarMap.TryGetValue(target, out var bar)) return;
        Destroy(bar.gameObject);
        m_HpBarMap.Remove(target);
    }

    public void UpdateHP(CharacterController target)
    {
        if (!m_HpBarMap.TryGetValue(target, out var bar)) return;
        bar.SetHP(target.GetCurrentHp(), target.GetStatValue(CharacterStat.HP));
    }

    void LateUpdate()
    {
        foreach (var pair in m_HpBarMap)
        {
            var target = pair.Key;
            var hpBar = pair.Value;
            // 월드좌표 -> 스크린좌표 변환
            Vector3 worldPos = target.transform.position + Vector3.up * 2.2f; // 캐릭터 머리 위, 필요시 조정
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            hpBar.SetScreenPosition(screenPos);
            // 화면 바깥이면 끄기 등 추가 가능
        }
    }
}
