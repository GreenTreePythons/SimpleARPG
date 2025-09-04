using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : Selectable, IPointerClickHandler
{
    [SerializeField] private AudioClip m_ClickSound;
    
    public delegate void OnButtonClicked();
    
    private OnButtonClicked m_OnButtonClicked;

    public void AddButtonEvent(OnButtonClicked onButtonClicked)
    {
        m_OnButtonClicked += onButtonClicked;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        
        m_OnButtonClicked?.Invoke();
        
        if (m_ClickSound == null) return;
        // play sound 
    }
}

#if UNITY_EDITOR
namespace UnityEngine.UI
{
    using UnityEditor;
    
    [CustomEditor(typeof(UIButton))]
    [CanEditMultipleObjects]
    public class IngredientDrawerUID : Editor
    {
        private static bool s_EnableOldMenuNextTime = false;

        [UnityEditor.MenuItem("GameObject/UI/Button - TextMeshPro", true)]
        public static bool DisableOldButtonMenu()
        {
            if (!s_EnableOldMenuNextTime) return false;
            s_EnableOldMenuNextTime = false;
            return true;
        }
        
        [UnityEditor.MenuItem("GameObject/UI/UIBUtton - TextMeshPro", false, 2031)]
        public static void CreateCustomButton()
        {
            //temporary enable old button functionality
            s_EnableOldMenuNextTime = true;
            if (!UnityEditor.EditorApplication.ExecuteMenuItem("GameObject/UI/Button - TextMeshPro")) return;

            //rename properly according to convention
            var go = UnityEditor.Selection.activeGameObject;
            go.name = "Button";
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            txt.overrideColorTags = true;
            if (txt != null) txt.gameObject.name = "Txt";

            //replace old button to new button component
            var oldButton = go.GetComponent<Button>();
            DestroyImmediate(oldButton);
            var newButton = go.AddComponent<UIButton>();
            var navigation = newButton.navigation;
            navigation.mode = Navigation.Mode.None;
            newButton.navigation = navigation;
        }
        public override void OnInspectorGUI()
        {
            
        }
    }
}
#endif