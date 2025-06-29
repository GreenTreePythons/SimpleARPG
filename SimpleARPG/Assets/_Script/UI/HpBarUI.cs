using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image m_FillImage;
    [SerializeField] TextMeshProUGUI m_HpText;

    public void SetHP(int curHp, int maxHp)
    {
        m_FillImage.fillAmount = Mathf.Clamp01((float)curHp / maxHp);
        m_HpText.text = $"{curHp} / {maxHp}";
    }

    public void SetPivot(float x)
    {
        var rect = GetComponent<RectTransform>();
        rect.pivot = new Vector2(x, rect.pivot.y);
    }

    public void SetScreenPosition(Vector3 screenPos)
    {
        transform.position = screenPos;
    }
}
