using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private Animator m_Animator;
    private string m_CurrentStateName;

    private void Awake()
    {
         m_Animator = this.GetComponent<Animator>();
         m_CurrentStateName = string.Empty;
    }

    public void PlayNormalIdle()
    {
        m_CurrentStateName = "NormalIdle";
        m_Animator.CrossFade(m_CurrentStateName, 0.15f);
    }


    public void PlayBattleIdle()
    {
        m_CurrentStateName = "BattleIdle";
        m_Animator.CrossFade(m_CurrentStateName, 0.15f);
    }

    public void PlayWalking(Vector2 input)
    {
        if (m_CurrentStateName == $"Walk{Get8Direction(input)}") return;
        m_CurrentStateName = $"Walk{Get8Direction(input)}";
        m_Animator.CrossFade(m_CurrentStateName, 0.15f);
    }

    public void PlayRunning(Vector2 input)
    {
        m_CurrentStateName = $"Running{Get8Direction(input)}";
        m_Animator.CrossFade(m_CurrentStateName, 0.15f);
    }

    public void PlaySprinting(Vector2 input)
    {
        m_CurrentStateName = $"Sprint{Get8Direction(input)}";
        m_Animator.CrossFade(m_CurrentStateName, 0.15f);   
    }

    public void PlayAttack(string stateName)
    {
        m_CurrentStateName = stateName;
        m_Animator.CrossFade(stateName, 0.15f);
    }

    public void PlayHit()
    {
        m_CurrentStateName = "Hit";
        m_Animator.CrossFade(m_CurrentStateName, 0.1f);
    }

    public void PlayDeath()
    {
        m_CurrentStateName = "Death";
        m_Animator.CrossFade(m_CurrentStateName, 0.2f);
    }

    public void PlaySkill(string skillAnimName)
    {
        m_CurrentStateName = skillAnimName;
        m_Animator.CrossFade(m_CurrentStateName, 0.12f);
    }
    
    private string Get8Direction(Vector2 input)
    {
        // if (input.sqrMagnitude < 0.01f) return "Idle";

        float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (angle >= 337.5f || angle < 22.5f)  return "Forward";
        if (angle >= 22.5f && angle < 67.5f)   return "ForwardRight";
        if (angle >= 67.5f && angle < 112.5f)  return "Right";
        if (angle >= 112.5f && angle < 157.5f) return "BackwardRight";
        if (angle >= 157.5f && angle < 202.5f) return "Backward";
        if (angle >= 202.5f && angle < 247.5f) return "BackwardLeft";
        if (angle >= 247.5f && angle < 292.5f) return "Left";
        if (angle >= 292.5f && angle < 337.5f) return "ForwardLeft";
        return "Forward";
    }
}