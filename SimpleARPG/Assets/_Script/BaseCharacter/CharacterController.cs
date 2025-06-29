using UnityEngine;

public enum CharacterStat
{
    HP, Strong, Defence
}
public abstract class CharacterController : MonoBehaviour, IWeaponController
{
    [Header("Base Stats")]
    [SerializeField] protected int m_MaxHp = 100;
    [SerializeField] protected int m_Strong = 10;
    [SerializeField] protected int m_Defence = 5;

    [SerializeField] SwordController m_SwordController;

    protected int m_CurrentHp;

    protected virtual void Awake()
    {
        m_CurrentHp = m_MaxHp;
    }

    public virtual void EnableWeaponHitBox()
    {
        m_SwordController.EnableHitBox();
    }

    public virtual void DisableWeaponHitBox()
    {
        m_SwordController.DisableHitBox();
    }

    public virtual void ApplyDamage(int damage)
    {
        m_CurrentHp = Mathf.Max(0, m_CurrentHp - damage);
        Debug.Log($"{gameObject.name} {damage} 피격! 남은 체력: {m_CurrentHp}");
    }

    public int GetStatValue(CharacterStat characterStat) => characterStat switch
    {
        CharacterStat.HP => m_MaxHp,
        CharacterStat.Strong => m_Strong,
        CharacterStat.Defence => m_Defence,
        _ => 0
    };

    public int GetCurrentHp() => m_CurrentHp;

    public bool IsDead() => m_CurrentHp <= 0;
    
    public abstract bool IsEnemy(CharacterController other);

    // 필요하다면 공용 데이터(HP, 스탯 등)도 여기에
    // 공용 피격 처리, 공용 Move/Attack 등도 가능
}