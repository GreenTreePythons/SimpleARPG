using System;
using UnityEngine;

public class CharacterBodyCollider : MonoBehaviour
{
    private CharacterAnimationController m_Animator;
    private CharacterFSMStatesController m_FSM;
    
    private void Awake()
    {
        m_Animator = GetComponentInParent<CharacterAnimationController>();
        m_FSM = GetComponentInParent<CharacterFSMStatesController>();
    }

    public void TakeHit(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log($"{this.name} took {damage} damage!");
        
        // TODO: 체력 감소, 경직, 이펙트, 사운드 등
        m_FSM.OnTakeHit(damage, hitPoint, hitNormal);
    }
}