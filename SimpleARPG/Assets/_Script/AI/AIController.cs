using UnityEngine;
using System.Collections;

public class AIController : CharacterController
{
    [Header("AI Settings")]
    [SerializeField] private float m_AttackRange = 1.5f;
    [SerializeField] private float m_AttackCooldown = 1.2f;
    [SerializeField] private float m_SearchRange = 8.0f;

    private CharacterStateController m_CharacterStateController;
    private CharacterAttackController m_CharacterAttackController;
    private Animator m_Animator;

    private CharacterController m_CurrentTarget = null;
    private float m_NextAttackTime = 0f;
    private float m_CurrentMoveSpeed = 0f;
    private bool m_IsInBattle = false;
    private bool m_IsAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterStateController = GetComponent<CharacterStateController>();
        m_CharacterAttackController = GetComponent<CharacterAttackController>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsDead()) return;

        UpdateTarget();
        UpdateBattleState();
        UpdateMoveAndAttack();
    }

    private void UpdateTarget()
    {
        m_CurrentTarget = m_EnemyDetector.GetNearestEnemy();
    }

    private void UpdateBattleState()
    {
        bool wasBattle = m_IsInBattle;
        m_IsInBattle = (m_CurrentTarget != null && (m_CurrentTarget.transform.position - transform.position).magnitude <= m_SearchRange);
        if (wasBattle != m_IsInBattle)
        {
            m_Animator.SetBool("IsBattleMode", m_IsInBattle);
        }
        m_CurrentMoveSpeed = m_IsInBattle ? GetStatValue(CharacterStat.BattleMoveSpeed) : GetStatValue(CharacterStat.NormalMoveSpeed);
    }

    private void UpdateMoveAndAttack()
    {
        if (m_CurrentTarget == null) return;

        Vector3 toTarget = m_CurrentTarget.transform.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        // 이동
        if (distance > m_AttackRange)
        {
            Vector3 moveDir = toTarget.normalized;
            transform.position += moveDir * m_CurrentMoveSpeed * Time.deltaTime;

            // 자연스럽게 타겟 방향으로 회전
            if (toTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
            m_IsAttacking = false;
        }
        else
        {
            // 공격
            if (!m_IsAttacking && Time.time >= m_NextAttackTime)
            {
                AttackType atkType = (Random.value > 0.5f) ? AttackType.Light : AttackType.Heavy;
                int targetCombo = Random.Range(1, m_CharacterAttackController.GetMaxComboCount(atkType) + 1);

                for (int i = 0; i < targetCombo; ++i)
                    m_CharacterAttackController.EnqueueAttackInput(atkType);

                m_IsAttacking = true;
                m_NextAttackTime = Time.time + m_AttackCooldown;
            }
        }
    }

    public override bool IsEnemy(CharacterController other) => other is PlayerController;
}
