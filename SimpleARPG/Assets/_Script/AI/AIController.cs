using UnityEngine;

public class AIController : CharacterController
{
    [Header("AI Settings")]
    [SerializeField] private Transform m_PlayerTransform;
    [SerializeField] private float m_BattleMoveSpeed = 2.0f;
    [SerializeField] private float m_NormalMoveSpeed = 3.5f;
    [SerializeField] private float m_AttackRange = 1.5f;
    [SerializeField] private float m_AttackCooldown = 1.2f;
    [SerializeField] private float m_SearchRange = 8.0f;
    [SerializeField] private float m_RandomMoveRadius = 5.0f;
    [SerializeField] private float m_IdleAfterMoveDuration = 2.0f;

    private CharacterStateController m_CharacterStateController;
    private CharacterAttackController m_CharacterAttackController;
    private Animator m_Animator;

    private Vector2 m_MoveInput = Vector2.zero;
    private float m_NextAttackTime = 0f;
    private float m_CurrentMoveSpeed = 0f;

    private Vector3 m_CurrentTargetPos;
    private bool m_IsChasingPlayer = false;
    private bool m_IsIdleAfterRandomMove = false;
    private float m_IdleEndTime = 0f;
    private bool m_IsInBattle = false;
    private bool m_IsAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterStateController = GetComponent<CharacterStateController>();
        m_CharacterAttackController = GetComponent<CharacterAttackController>();
        m_Animator = GetComponent<Animator>();
        if (m_PlayerTransform == null && GameObject.FindWithTag("Player") != null)
            m_PlayerTransform = GameObject.FindWithTag("Player").transform;
        m_CurrentTargetPos = transform.position;
    }

    void Update()
    {
        if (m_PlayerTransform == null) return;
        if (IsDead()) return;

        UpdateBattleState();
        UpdateMoveTarget();
        UpdateAttackLogic();
        UpdateMoveInput();
        ApplyMoveAndRotation();
        m_CharacterStateController.SetInput(m_MoveInput, false, false, false);
    }

    private void UpdateBattleState()
    {
        float playerDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);
        bool wasBattle = m_IsInBattle;
        m_IsInBattle = playerDistance <= m_SearchRange;
        if (wasBattle != m_IsInBattle)
        {
            m_Animator.SetBool("IsBattleMode", m_IsInBattle);
        }
        m_CurrentMoveSpeed = m_IsInBattle ? m_BattleMoveSpeed : m_NormalMoveSpeed;
    }

    private void UpdateMoveTarget()
    {
        if (m_IsInBattle)
        {
            m_CurrentTargetPos = m_PlayerTransform.position;
            m_IsChasingPlayer = true;
            m_IsIdleAfterRandomMove = false;
        }
        else
        {
            m_IsChasingPlayer = false;
            if (m_IsIdleAfterRandomMove)
            {
                if (Time.time >= m_IdleEndTime)
                {
                    m_CurrentTargetPos = GetRandomPosition(transform.position, m_RandomMoveRadius);
                    m_IsIdleAfterRandomMove = false;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, m_CurrentTargetPos) < 0.5f)
                {
                    m_IsIdleAfterRandomMove = true;
                    m_IdleEndTime = Time.time + m_IdleAfterMoveDuration;
                    m_MoveInput = Vector2.zero;
                }
            }
        }
    }

    private void UpdateAttackLogic()
    {
        float playerDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        if (m_IsChasingPlayer && playerDistance <= m_AttackRange)
        {
            if (!m_IsAttacking && Time.time >= m_NextAttackTime)
            {
                // Light, Heavy 중 랜덤, 콤보 횟수도 랜덤하게 입력
                AttackType atkType = (Random.value > 0.5f) ? AttackType.Light : AttackType.Heavy;
                int targetCombo = Random.Range(1, m_CharacterAttackController.GetMaxComboCount(atkType) + 1);

                for (int i = 0; i < targetCombo; ++i)
                    m_CharacterAttackController.EnqueueAttackInput(atkType);

                m_IsAttacking = true;
                m_NextAttackTime = Time.time + m_AttackCooldown;
            }
        }
        else
        {
            m_IsAttacking = false;
        }
    }

    private void UpdateMoveInput()
    {
        Vector3 toTarget = m_CurrentTargetPos - transform.position;
        toTarget.y = 0;
        float playerDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        if (m_IsChasingPlayer && playerDistance <= m_AttackRange)
        {
            m_MoveInput = Vector2.zero;
        }
        else
        {
            if (m_IsIdleAfterRandomMove)
            {
                m_MoveInput = Vector2.zero;
            }
            else
            {
                Vector3 localDir = transform.InverseTransformDirection(toTarget.normalized);
                m_MoveInput = new Vector2(localDir.x, localDir.z);
            }
        }
    }

    private void ApplyMoveAndRotation()
    {
        Vector3 toTarget = m_CurrentTargetPos - transform.position;
        toTarget.y = 0;
        float playerDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        if ((!m_IsChasingPlayer || (m_IsChasingPlayer && playerDistance > m_AttackRange)) && !m_IsIdleAfterRandomMove)
        {
            Vector3 moveDir = toTarget.normalized;
            moveDir.y = 0;
            transform.position += moveDir * m_CurrentMoveSpeed * Time.deltaTime;
        }

        if (toTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private Vector3 GetRandomPosition(Vector3 center, float radius)
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        float distance = Random.Range(radius * 0.5f, radius);
        Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        pos.y = center.y;
        return pos;
    }

    public override bool IsEnemy(CharacterController other) => other is PlayerController;
}
