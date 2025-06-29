using UnityEngine;

public enum CharacterState
{
    Idle, Move, Attack1, Attack2, Attack3, Parry, Block
}

public interface ICharacterState
{
    void EnterState();
    void HandleInput();
    void UpdateState();
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
}

public class Attack1State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;
    private bool m_NextComboQueued;

    public float CurrentStateTime { get; private set; }

    public Attack1State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log("Enter Attack1 State");
        CurrentStateTime = 0;
        m_NextComboQueued = false;
        m_Controller.CharacterAnimator.SetTrigger("Attack1");
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
}

public class Attack2State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;
    private bool m_NextComboQueued;

    public float CurrentStateTime { get; private set; }
    public Attack2State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log("Enter Attack2 State");
        CurrentStateTime = 0f;
        m_NextComboQueued = false;
        m_Controller.CharacterAnimator.SetTrigger("Attack2");
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
}

public class Attack3State : ICharacterState, IStateTimer
{
    private CharacterStateController m_Controller;

    public float CurrentStateTime { get; private set; }
    public Attack3State(CharacterStateController c) { m_Controller = c; }

    public void EnterState()
    {
        Debug.Log("Enter Attack3 State");
        CurrentStateTime = 0f;
        m_Controller.CharacterAnimator.SetTrigger("Attack3");
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
}
