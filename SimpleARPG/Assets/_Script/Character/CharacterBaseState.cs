// State Base
public abstract class CharacterBaseState
{
    protected CharacterFSMStatesController m_StateController;
    protected CharacterAnimationController m_AnimController;

    protected CharacterBaseState(CharacterFSMStatesController controller, CharacterAnimationController animController)
    {
        this.m_StateController = controller;
        this.m_AnimController = animController;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update() { }
}
