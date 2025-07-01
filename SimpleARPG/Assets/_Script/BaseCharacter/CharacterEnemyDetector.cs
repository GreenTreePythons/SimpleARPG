using System.Collections.Generic;
using UnityEngine;

public class CharacterEnemyDetector : MonoBehaviour
{
    [SerializeField] private CharacterController m_Owner;

    private HashSet<CharacterController> m_DetectedEnemies = new();

    public HashSet<CharacterController> GetDetectedEnemis() => m_DetectedEnemies;

    private void Awake()
    {
        if (m_Owner == null) m_Owner = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var characterController = other.GetComponent<CharacterController>();

        if (characterController == null) return;
        if (characterController.IsDead()) return;
        if (!m_Owner.IsEnemy(characterController)) return;

        m_DetectedEnemies.Add(characterController);
    }

    private void OnTriggerExit(Collider other)
    {
        var characterController = other.GetComponent<CharacterController>();
        if (characterController == null) return;

        m_DetectedEnemies.Remove(characterController);
    }

    public CharacterController GetNearestEnemy()
    {
        float nearestDistance = float.MaxValue;
        CharacterController nearestEnemy = null;
        foreach (var enemy in m_DetectedEnemies)
        {
            float distance = (enemy.transform.position - transform.position).magnitude;
            if (nearestDistance > distance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}
