using UnityEngine;

public enum CharacterState
{
    Idle, Move, Attack1, Attack2, Attack3, Parry, Block, Damaged
}

public interface ICharacterState
{
    void EnterState();
    void HandleInput();
    void UpdateState();
    void ExitState();
    CharacterState GetState();
}

public interface IStateTimer
{
    public float CurrentStateTime { get; }
}

public class IdleState : ICharacterState
{
    private CharacterStateController m_Controller;
    public IdleState(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log("Enter Idle State");
        m_Controller.CharacterAnimator.SetBool("IsMoving", false);
    }
    public void HandleInput()
    {
        if (m_Controller.MoveInput.magnitude > 0)
            m_Controller.ChangeState(m_Controller.MovingState);
        else if (m_Controller.AttackPressed)
            m_Controller.ChangeState(m_Controller.Attack1State);
        else if (m_Controller.ParryPressed)
            m_Controller.ChangeState(m_Controller.ParryState);
        else if (m_Controller.BlockPressed)
            m_Controller.ChangeState(m_Controller.BlockState);
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Idle;

    public void ExitState()
    {
        
    }
}

public class MovingState : ICharacterState
{
    private CharacterStateController m_Controller;
    public MovingState(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log("Enter Moving State");
        m_Controller.CharacterAnimator.SetBool("IsMoving", true);
    }
    public void HandleInput()
    {
        if (m_Controller.AttackPressed)
            m_Controller.ChangeState(m_Controller.Attack1State);
        else if (m_Controller.ParryPressed)
            m_Controller.ChangeState(m_Controller.ParryState);
        else if (m_Controller.BlockPressed)
            m_Controller.ChangeState(m_Controller.BlockState);
        else if (m_Controller.MoveInput.magnitude == 0)
            m_Controller.ChangeState(m_Controller.IdleState);
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Move;

    public void ExitState()
    {

    }
}

public class Attack1State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;
    private bool m_NextComboQueued;

    public float CurrentStateTime { get; private set; }

    public Attack1State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log($"Enter Attack1State");
        CurrentStateTime = 0;
        m_NextComboQueued = false;
        m_Controller.CharacterAnimator.SetInteger("AttackComboCount", 1);
        m_Controller.CharacterAnimator.SetTrigger("Attack");
    }

    public void HandleInput()
    {
        if (m_Controller.MoveInput.magnitude > 0)
        {
            m_Controller.ChangeState(m_Controller.MovingState);
            return;
        }

        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        if (m_Controller.AttackPressed && animState.normalizedTime > 0.2f)
        {
            m_NextComboQueued = true;
        }
    }

    public void UpdateState()
    {
        CurrentStateTime += Time.deltaTime;
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);

        if (animState.normalizedTime > 0.7f)
        {
            if (m_NextComboQueued)
                m_Controller.ChangeState(m_Controller.Attack2State);
        }

        if (CurrentStateTime > 5.0f || animState.normalizedTime >= 0.95f)
            m_Controller.ChangeState(m_Controller.IdleState);
    }

    public CharacterState GetState() => CharacterState.Attack1;

    public void ExitState()
    {
        
    }
}

public class Attack2State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;
    private bool m_NextComboQueued;

    public float CurrentStateTime { get; private set; }
    public Attack2State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log($"Enter Attack2State");
        CurrentStateTime = 0f;
        m_NextComboQueued = false;
        m_Controller.CharacterAnimator.SetInteger("AttackComboCount", 2);
        m_Controller.CharacterAnimator.SetTrigger("Attack");
    }
    
    public void HandleInput()
    {
        if (m_Controller.MoveInput.magnitude > 0)
        {
            m_Controller.ChangeState(m_Controller.MovingState);
            return;
        }

        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        if (m_Controller.AttackPressed && animState.normalizedTime > 0.2f)
        {
            m_NextComboQueued = true;
        }
    }

    public void UpdateState()
    {
        CurrentStateTime += Time.deltaTime;
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);

        if (animState.normalizedTime > 0.7f)
        {
            if (m_NextComboQueued)
                m_Controller.ChangeState(m_Controller.Attack3State);
        }

        if (CurrentStateTime > 5.0f || animState.normalizedTime >= 0.95f)
            m_Controller.ChangeState(m_Controller.IdleState);

    }

    public CharacterState GetState() => CharacterState.Attack2;

    public void ExitState()
    {
        
    }
}

public class Attack3State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;

    public float CurrentStateTime { get; private set; }
    public Attack3State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log($"Enter Attack3State");
        CurrentStateTime = 0f;
        m_Controller.CharacterAnimator.SetInteger("AttackComboCount", 3);
        m_Controller.CharacterAnimator.SetTrigger("Attack");
    }
    
    public void HandleInput()
    {
        if (m_Controller.MoveInput.magnitude > 0)
        {
            m_Controller.ChangeState(m_Controller.MovingState);
            return;
        }
    }

    public void UpdateState()
    {
        CurrentStateTime += Time.deltaTime;
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);

        if (CurrentStateTime > 5.0f || animState.normalizedTime >= 0.95f)
            m_Controller.ChangeState(m_Controller.IdleState);
    }

    public CharacterState GetState() => CharacterState.Attack3;

    public void ExitState()
    {
        
    }
}

public class ParryState : ICharacterState
{
    private CharacterStateController m_Controller;
    public ParryState(CharacterStateController c) { m_Controller = c; }
    public void EnterState()
    {
        m_Controller.CharacterAnimator.SetTrigger("Parry");
    }
    public void HandleInput() { }
    public void UpdateState()
    {
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName("Parry") && animState.normalizedTime >= 1.0f)
            m_Controller.ChangeState(m_Controller.IdleState);
    }
    public CharacterState GetState() => CharacterState.Parry;

    public void ExitState()
    {
        
    }
}

public class BlockState : ICharacterState
{
    private CharacterStateController m_Controller;
    public BlockState(CharacterStateController c) { m_Controller = c; }
    public void EnterState()
    {
        Debug.Log("Enter Block State");
        m_Controller.CharacterAnimator.SetTrigger("Block");
    }
    
    public void HandleInput()
    {

    }

    public void UpdateState()
    {
        if (!m_Controller.BlockPressed)
            m_Controller.ChangeState(m_Controller.IdleState);
    }

    public CharacterState GetState() => CharacterState.Block;

    public void ExitState()
    {

    }
}

public class DamagedState : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;
    private Vector3 m_KnockbackDir;
    private float m_KnockbackForce;
    private float m_Duration;
    public float CurrentStateTime { get; private set; }

    public DamagedState(CharacterStateController c)
    {
        m_Controller = c;
    }

    public void SetDamageInfo(Vector3 knockbackDir, float knockbackForce, float duration)
    {
        m_KnockbackDir = knockbackDir.normalized;
        m_KnockbackForce = knockbackForce;
        m_Duration = duration;
    }

    public void EnterState()
    {
        CurrentStateTime = 0f;
        m_Controller.CharacterAnimator.SetTrigger("Damaged");
    }

    public void HandleInput()
    {
        // 피격 상태에서는 입력 무시
    }

    public void UpdateState()
    {
        CurrentStateTime += Time.deltaTime;
        // 넉백 이동
        m_Controller.transform.position += m_KnockbackDir * m_KnockbackForce * Time.deltaTime;

        if (CurrentStateTime >= m_Duration)
        {
            m_Controller.ChangeState(m_Controller.IdleState); // 또는 전투 Idle로 복귀
        }
    }

    public CharacterState GetState() => CharacterState.Damaged; // enum에 Damaged 없으면 -1로 처리

    public void ExitState() { }
}