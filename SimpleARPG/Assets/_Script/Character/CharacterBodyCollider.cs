using UnityEngine;

public class CharacterBodyCollider : MonoBehaviour
{
    [SerializeField] private GameObject m_Owner;

    public void TakeHit(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log($"{m_Owner.name} took {damage} damage!");
        
        // TODO: 체력 감소, 경직, 이펙트, 사운드 등
    }
}