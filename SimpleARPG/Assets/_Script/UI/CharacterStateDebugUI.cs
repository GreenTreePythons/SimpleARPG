
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterFSMStatesController))]
public class CharacterStateDebugGUI : MonoBehaviour
{
    [SerializeField] int m_FontSize = 16;
    [SerializeField] Color m_TextColor = Color.white;
    
    private CharacterFSMStatesController m_StateController;
    private CharacterAnimationController m_AnimationController;
    private float m_DeltaTime;

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
        
        float fps = 1.0f / Time.deltaTime;
        float ms = Time.deltaTime * 1000.0f;
        string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
        GUI.Label(new Rect(10, 160, 400, 40), $"FPS: {text}", style);
    }
}