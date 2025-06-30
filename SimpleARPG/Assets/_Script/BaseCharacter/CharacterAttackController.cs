using UnityEngine;
using System.Collections.Generic;

public enum AttackType
{
    None,
    Light,
    Heavy
}

public class CharacterAttackController : MonoBehaviour
{
    private struct AttackInput
    {
        public AttackType type;
        public int frame;
    }

    [SerializeField] private Animator m_Animator;
    [SerializeField] private CharacterStateController m_StateController;
    [SerializeField] private CharacterController m_CharacterController;

    private Queue<AttackInput> m_AttackInputQueue = new();
    private bool m_IsAttacking = false;
    private AttackType m_CurrentAttackType = AttackType.None;
    private int m_CurrentComboIndex = 0;
    private readonly Dictionary<AttackType, int> m_MaxComboCount = new()
    {
        { AttackType.Light, 4 },
        { AttackType.Heavy, 3 }
    };

    public int GetMaxComboCount(AttackType attackType) => m_MaxComboCount[attackType];

    void Awake()
    {
        if (!m_Animator) m_Animator = GetComponent<Animator>();
        if (!m_StateController) m_StateController = GetComponent<CharacterStateController>();
        if (!m_CharacterController) m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!m_IsAttacking) return;

        var stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (m_AttackInputQueue.Count > 0)
        {
            var input = m_AttackInputQueue.Dequeue();

            if (input.type == m_CurrentAttackType && m_CurrentComboIndex < m_MaxComboCount[input.type] + 1)
            {
                m_CurrentComboIndex++;
            }
            else
            {
                m_CurrentAttackType = input.type;
                m_CurrentComboIndex = 1;
            }

            m_Animator.SetTrigger("Attack");
            m_Animator.SetInteger("AttackType", (int)m_CurrentAttackType);
            m_Animator.SetInteger("AttackIndex", m_CurrentComboIndex);
        }

        if (IsAttackAnimationEnd(stateInfo))
        {
            EndAttack();
        }
    }

    public void EnqueueAttackInput(AttackType type)
    {
        m_AttackInputQueue.Enqueue(new AttackInput { type = type, frame = Time.frameCount });

        if (!m_IsAttacking)
            StartAttack(type);
    }

    private void StartAttack(AttackType type)
    {
        m_IsAttacking = true;
        m_CurrentAttackType = type;
        m_CurrentComboIndex = 0;
        m_Animator.SetTrigger("Attack");
        m_Animator.SetInteger("AttackType", (int)type);
        m_Animator.SetInteger("AttackIndex", 1);
        m_StateController.ChangeState(new LightAttackState(m_StateController, 1));
    }

    private void EndAttack()
    {
        m_IsAttacking = false;
        m_CurrentAttackType = AttackType.None;
        m_CurrentComboIndex = 0;
        m_AttackInputQueue.Clear();
        m_Animator.ResetTrigger("Attack");
        m_Animator.SetInteger("AttackType", (int)m_CurrentAttackType);
        m_Animator.SetInteger("AttackIndex", 0);
        m_StateController.ChangeState(m_StateController.IdleState);
    }

    private bool IsAttackAnimationEnd(AnimatorStateInfo stateInfo) =>  stateInfo.IsTag("Attack") && stateInfo.normalizedTime > 0.95f;

    // 히트박스 활성/비활성화 등은 AnimationEvent로 m_CharacterController 호출
    public void EnableWeaponHitBox()
    {
        m_CharacterController.EnableWeaponHitBox();
    }
    public void DisableWeaponHitBox()
    {
        m_CharacterController.DisableWeaponHitBox();
    }
}
