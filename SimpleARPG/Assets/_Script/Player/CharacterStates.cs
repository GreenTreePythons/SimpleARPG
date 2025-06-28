using UnityEngine;

public enum CharacterState
{
    Idle,
    Move,
    Attack1,
    Attack2,
    Attack3,
    Parry,
    Block
}

public interface ICharacterState
{
    void EnterState();
    void HandleInput();
    void UpdateState();
    CharacterState GetState();
}

public class IdleState : ICharacterState
{
    private CharacterActionController m_Character;

    public IdleState(CharacterActionController character) { m_Character = character; }

    public void EnterState() { }
    public void HandleInput()
    {
        if (m_Character.MoveInput.magnitude > 0) m_Character.ChangeState(new MovingState(m_Character));
        if (m_Character.AttackPressed) m_Character.ChangeState(new Attack1State(m_Character));
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Idle;
}

public class MovingState : ICharacterState
{
    private CharacterActionController m_Character;

    public MovingState(CharacterActionController character) { m_Character = character; }

    public void EnterState() { }
    public void HandleInput()
    {
        if (m_Character.AttackPressed) m_Character.ChangeState(new Attack1State(m_Character));
        if (m_Character.ParryPressed) m_Character.ChangeState(new ParryingState(m_Character));
        if (m_Character.BlockPressed) m_Character.ChangeState(new BlockingState(m_Character));
        if (m_Character.MoveInput.magnitude == 0) m_Character.ChangeState(new IdleState(m_Character));
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Move;
}

public class Attack1State : ICharacterState
{
    private CharacterActionController m_Character;
    private float m_AttackTimer = 0f;
    private const float m_ComboTimeLimit = 5.5f;

    public Attack1State(CharacterActionController character)
    {
        m_Character = character;
    }

    public void EnterState()
    {
        m_AttackTimer = 0.0f;
    }

    public void HandleInput()
    {
        m_AttackTimer += Time.deltaTime;

        if (m_AttackTimer > m_ComboTimeLimit) m_Character.ChangeState(new IdleState(m_Character));

        if (m_Character.AttackPressed)
        {
            m_Character.ChangeState(new Attack2State(m_Character));
        }
    }

    public void UpdateState() { }

    public CharacterState GetState() => CharacterState.Attack1;
}

public class Attack2State : ICharacterState
{
    private CharacterActionController m_Character;
    private float m_AttackTimer = 0f;
    private const float m_ComboTimeLimit = 0.5f;

    public Attack2State(CharacterActionController character)
    {
        m_Character = character;
    }

    public void EnterState()
    {
        m_AttackTimer = 0.0f;
    }

    public void HandleInput()
    {
        m_AttackTimer += Time.deltaTime;
        if (m_AttackTimer > m_ComboTimeLimit) m_Character.ChangeState(new IdleState(m_Character));
        if (m_Character.AttackPressed) m_Character.ChangeState(new Attack3State(m_Character));
    }

    public void UpdateState()
    { 

    }

    public CharacterState GetState() => CharacterState.Attack2;
}

public class Attack3State : ICharacterState
{
    private CharacterActionController m_Character;

    public Attack3State(CharacterActionController character)
    {
        m_Character = character;
    }

    public void EnterState()
    {
        
    }

    public void HandleInput()
    { 

    }

    public void UpdateState()
    {
        m_Character.ChangeState(new IdleState(m_Character));
    }

    public CharacterState GetState() => CharacterState.Attack3;
}

public class ParryingState : ICharacterState
{
    private CharacterActionController m_Character;

    public ParryingState(CharacterActionController character) { m_Character = character; }

    public void EnterState() { }
    public void HandleInput() { }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Parry;
}

public class BlockingState : ICharacterState
{
    private CharacterActionController m_Character;

    public BlockingState(CharacterActionController character) { m_Character = character; }

    public void EnterState() { }
    public void HandleInput() { }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Block;
}
