using UnityEngine;

public enum CharacterState
{
    Idle, Move, LightAttack, HeavyAttack, Parry, Block, Damaged, Dead
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

    public void EnterState() { m_Controller.CharacterAnimator.SetBool("IsMoving", false); }
    public void HandleInput()
    {
        if (m_Controller.MoveInput.magnitude > 0)
            m_Controller.ChangeState(m_Controller.MovingState);
        else if (m_Controller.ParryPressed)
            m_Controller.ChangeState(m_Controller.ParryState);
        else if (m_Controller.BlockPressed)
            m_Controller.ChangeState(m_Controller.BlockState);
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Idle;
    public void ExitState() { }
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
        if (m_Controller.ParryPressed)
            m_Controller.ChangeState(m_Controller.ParryState);
        else if (m_Controller.BlockPressed)
            m_Controller.ChangeState(m_Controller.BlockState);
        else if (m_Controller.MoveInput.magnitude == 0)
            m_Controller.ChangeState(m_Controller.IdleState);
    }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Move;
    public void ExitState() { }
}

public class LightAttackState : ICharacterState
{
    private CharacterStateController m_Controller;
    public LightAttackState(CharacterStateController controller, int comboIndex = 1) { m_Controller = controller; }
    public void EnterState() { m_Controller.CharacterController.EnableWeaponHitBox(); }
    public void HandleInput() {  }
    public void UpdateState()
    {
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        if (animState.normalizedTime > 0.95f)
            m_Controller.ChangeState(m_Controller.IdleState);
    }
    public void ExitState() { m_Controller.CharacterController.DisableWeaponHitBox(); }
    public CharacterState GetState() => CharacterState.LightAttack;
}

public class HeavyAttackState : ICharacterState
{
    private CharacterStateController m_Controller;
    public HeavyAttackState(CharacterStateController controller, int comboIndex = 1) { m_Controller = controller; }
    public void EnterState() { m_Controller.CharacterController.EnableWeaponHitBox(); }
    public void HandleInput() {  }
    public void UpdateState()
    {
        var animState = m_Controller.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        if (animState.normalizedTime > 0.95f)
            m_Controller.ChangeState(m_Controller.IdleState);
    }
    public void ExitState() { m_Controller.CharacterController.DisableWeaponHitBox(); }
    public CharacterState GetState() => CharacterState.HeavyAttack;
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

public class DeadState : ICharacterState
{
    private CharacterStateController m_Controller;

    public DeadState(CharacterStateController c)
    {
        m_Controller = c;
    }

    public void EnterState()
    {
        Debug.Log($"{m_Controller.gameObject.name} 사망!");
        m_Controller.CharacterAnimator.SetTrigger("Dead");
        
        // 콜라이더 비활성화 등 추가
        if (m_Controller.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        // 무기, AI 등 끄기
        if (m_Controller.TryGetComponent<SwordController>(out var sword))
            sword.enabled = false;

        HPBarUIManager.Instance?.Unregister(m_Controller.CharacterController);
    }
    public void HandleInput() { }
    public void UpdateState() { }
    public CharacterState GetState() => CharacterState.Dead;
    public void ExitState() { }
}