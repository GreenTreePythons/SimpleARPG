

public class CharacterAttackedState : CharacterBaseState
{
    public CharacterAttackedState(CharacterFSMStatesController controller, CharacterAnimationController animController) 
        : base(controller, animController) { }
    
    public override void OnEnter()
    {   
        base.OnEnter();
        m_AnimController.PlayHit();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}