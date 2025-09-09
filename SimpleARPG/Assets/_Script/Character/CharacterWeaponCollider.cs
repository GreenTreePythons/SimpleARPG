using UnityEngine;

public class CharacterWeaponCollider : MonoBehaviour
{   
    [SerializeField] private float m_Damage = 10f;
    [SerializeField] private GameObject m_Owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == m_Owner) return;

        CharacterBodyCollider bodyCollider = other.GetComponent<CharacterBodyCollider>();
        if (bodyCollider != null)
        {
            Vector3 point = other.ClosestPoint(transform.position);
            Vector3 normal = (other.transform.position - transform.position).normalized;
            bodyCollider.TakeHit(m_Damage, point, normal);
        }
    }
}