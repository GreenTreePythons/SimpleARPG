using UnityEngine;

public enum CharacterStat
{
    HP, Strong, Defence, BattleMoveSpeed, NormalMoveSpeed
}

public abstract class CharacterController : MonoBehaviour, IWeaponController
{
    [Header("Base Stats")]
    [SerializeField] protected float m_MaxHp = 100;
    [SerializeField] protected float m_Strong = 10;
    [SerializeField] protected float m_Defence = 5;
    [SerializeField] protected float m_BattleMoveSpeed = 2.0f;
    [SerializeField] protected float m_NormalMoveSpeed = 7.0f;

    // [SerializeField] SwordController m_SwordController;
    [SerializeField] HPBarUI m_HPBar;
    [SerializeField] CharacterEnemyDetector m_Detector;

    protected float m_CurrentHp;
    protected CharacterEnemyDetector m_EnemyDetector;

    protected virtual void Awake()
    {
        m_CurrentHp = m_MaxHp;
        HPBarUIManager.Instance?.Register(this);
        m_EnemyDetector = m_Detector;
    }

    public virtual void EnableWeaponHitBox()
    {
        // m_SwordController.EnableHitBox();
    }

    public virtual void DisableWeaponHitBox()
    {
        // m_SwordController.DisableHitBox();
    }

    public virtual void ApplyDamage(float damage)
    {
        m_CurrentHp = Mathf.Max(0, m_CurrentHp - damage);
        Debug.Log($"{gameObject.name} {damage} 피격! 남은 체력: {m_CurrentHp}");

        HPBarUIManager.Instance?.UpdateHP(this);
    }

    public float GetStatValue(CharacterStat characterStat) => characterStat switch
    {
        CharacterStat.HP => m_MaxHp,
        CharacterStat.Strong => m_Strong,
        CharacterStat.Defence => m_Defence,
        CharacterStat.BattleMoveSpeed => m_BattleMoveSpeed,
        CharacterStat.NormalMoveSpeed => m_NormalMoveSpeed,
        _ => 0.0f
    };

    public float GetCurrentHp() => m_CurrentHp;

    public bool IsDead() => m_CurrentHp <= 0;

    public abstract bool IsEnemy(CharacterController other);
}