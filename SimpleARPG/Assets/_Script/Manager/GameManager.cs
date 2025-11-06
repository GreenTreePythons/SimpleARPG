using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISceneLoadHandler
{   
    public static GameManager Instance { get; private set; }

    public InputManager InputManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public SceneSystem SceneSystem { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputManager = new GameObject("InputManager").AddComponent<InputManager>();
        InputManager.transform.SetParent(this.transform);
        
        SceneSystem.Instance.RegisterSceneLoadHandler(this);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CameraManager != null) CameraManager = null;
        CameraManager = FindObjectOfType<CameraManager>();
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("Game is quitting. Performing cleanup...");
        
        AddressableSystem.Instance?.ReleaseAll();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}