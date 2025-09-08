using UnityEngine;

public class CharacterAttackingState : CharacterBaseState
{
    private AttackComboInfo[] m_ComboInfos;
    private int m_ComboStep;
    private float m_CurrentComboElapsedTime;
    private AttackComboInfo m_CurrentComboInfo;
    private bool m_IsNextComboReserved;
    private ComboType m_CurrentComboType;
    private ComboType m_ReservedNextComboType;
    
    public CharacterAttackingState(CharacterFSMStatesController controller, CharacterAnimationController animController)
        : base(controller, animController) { }

    public override void OnEnter()
    {
        base.OnEnter();
        m_ComboStep = 0;
        ClearComboState();
        m_CurrentComboType = GameManager.Instance.InputManager.LatestComboTypeInput;
        m_ComboInfos = m_StateController.GetComboDatas(m_CurrentComboType);
        m_CurrentComboInfo = m_ComboInfos[m_ComboStep];
    }

    public override void Update()
    {
        base.Update();
        m_CurrentComboElapsedTime += Time.deltaTime;
        m_StateController.ComboTimer = m_CurrentComboElapsedTime;

        var currentComboValidTime = m_CurrentComboElapsedTime >= m_CurrentComboInfo.ComboValidStartTime &&
                                    m_CurrentComboElapsedTime <= m_CurrentComboInfo.ComboValidTime;
        var canTransitionToNextCombo = m_ComboStep == 0 || m_CurrentComboElapsedTime >= m_CurrentComboInfo.ComboValidTime;
        
        // reserve next combo
        if (currentComboValidTime && !m_IsNextComboReserved )
        {
            var input = GameManager.Instance.InputManager.LatestComboTypeInput;
            if (input != ComboType.None)
            {
                m_ReservedNextComboType = input;
                m_IsNextComboReserved = true;
                m_StateController.NextComboQueued = true;
                if (m_CurrentComboType != m_ReservedNextComboType)
                {
                    m_ComboStep = 0;
                    canTransitionToNextCombo = false;
                }
            }
        }
        
        // play next combo
        if (m_IsNextComboReserved && canTransitionToNextCombo)
        {
            if (m_ComboStep < m_ComboInfos.Length)
            {
                m_CurrentComboInfo = m_ComboInfos[m_ComboStep];
                m_AnimController.PlayAttacking(m_CurrentComboInfo.AnimStateName);
                
                m_ComboStep++;
                ClearComboState();
                return;
            }
        }
        
        // exit combo
        if (m_CurrentComboElapsedTime > m_CurrentComboInfo.ComboValidTime)
        {
            m_ComboStep = 0;
            ClearComboState();
            m_StateController.ChangeState(CharacterStateType.Idle);
        }
    }

    private void ClearComboState()
    {
        m_CurrentComboElapsedTime = 0f;
        m_IsNextComboReserved = false;
        m_ReservedNextComboType = ComboType.None;
            
        m_StateController.NextComboQueued = false;
        m_StateController.ComboTimer = 0.0f;
        m_StateController.CurrentComboStep = m_ComboStep;
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}