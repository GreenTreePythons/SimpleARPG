using UnityEngine;
using UnityEngine.UI;

public class TitlePage : MonoBehaviour
{
    [SerializeField] UIButton StartButton;
    [SerializeField] UIButton QuitButton;

    void Awake()
    {
        StartButton.AddButtonEvent(OnClickStart);
        QuitButton.AddButtonEvent(OnClickQuit);
    }

    void OnClickStart()
    {
        SceneSystem.Instance.LoadScene("MainScene");
    }

    void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}