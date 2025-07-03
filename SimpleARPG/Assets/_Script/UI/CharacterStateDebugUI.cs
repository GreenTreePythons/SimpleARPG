
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterFSMStatesController))]
public class CharacterStateDebugGUI : MonoBehaviour
{
    [SerializeField] int m_FontSize = 16;
    [SerializeField] Color m_TextColor = Color.white;
    
    CharacterFSMStatesController m_StateController;
    CharacterAnimationController m_AnimationController;

    private void Awake()
    {
        m_StateController = GetComponent<CharacterFSMStatesController>();
        m_AnimationController = GetComponent<CharacterAnimationController>();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = m_FontSize,
            normal = { textColor = m_TextColor }
        };
        GUI.Label(new Rect(10, 10, 400, 40), $"Current State: {m_StateController.CurrentStateType}", style);
        GUI.Label(new Rect(10, 40, 400, 40), $"Current ComboStep: {m_StateController.CurrentComboStep}", style);
        GUI.Label(new Rect(10, 70, 400, 40), $"Current ComboTimer: {m_StateController.ComboTimer}", style);
        GUI.Label(new Rect(10, 100, 400, 40), $"NextComboQueued: {m_StateController.NextComboQueued}", style);
        GUI.Label(new Rect(10, 130, 400, 40), $"LockOnTarget: {GameManager.Instance.InputManager.IsLockOnTarget}", style);
    }
}