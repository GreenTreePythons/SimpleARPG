using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public interface IWeaponController
{
    void EnableWeaponHitBox();
    void DisableWeaponHitBox();
}

public class SwordController : MonoBehaviour
{
    [SerializeField] private float m_KnockbackForce = 6.0f;
    [SerializeField] private float m_DamagedDuration = 0.2f;
    [SerializeField] private Collider m_HitBoxCollider;

    private HashSet<Collider> m_HitTargets = new();
    private CharacterController m_CharacterController;

    private void Awake()
    {
        m_CharacterController = this.GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 피격 대상 중복 방지
        if (m_HitTargets.Contains(other)) return;

        if (!other.TryGetComponent<CharacterStateController>(out CharacterStateController stateController)) return;

        var targetCharCtrl = stateController.CharacterController;

        if (!m_CharacterController.IsEnemy(targetCharCtrl)) return;

        m_HitTargets.Add(other);
        stateController.OnDamaged(transform.position, m_KnockbackForce, m_DamagedDuration, m_CharacterController.GetStatValue(CharacterStat.Strong));
    }

    public void EnableHitBox()
    {

        m_HitBoxCollider.enabled = true;
        m_HitTargets.Clear();
    }

    public void DisableHitBox()
    {
        m_HitBoxCollider.enabled = false;
    }
}