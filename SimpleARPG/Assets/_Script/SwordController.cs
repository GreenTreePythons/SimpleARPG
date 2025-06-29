using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] private float m_KnockbackForce = 6.0f;
    [SerializeField] private float m_DamagedDuration = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI") || other.CompareTag("Player"))
        {
            CharacterStateController enemy = other.GetComponent<CharacterStateController>();
            if (enemy == null) return;

            // 내 위치(칼)에서 적까지 넉백
            enemy.OnDamaged(transform.position, m_KnockbackForce, m_DamagedDuration);
        }
    }
}