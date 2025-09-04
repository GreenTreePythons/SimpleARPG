using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISceneLoadHandler
{   
    public static GameManager Instance { get; private set; }

    public InputManager InputManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public SceneManager SceneManager { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputManager = new GameObject("InputManager").AddComponent<InputManager>();
        InputManager.transform.SetParent(this.transform);
        
        SceneManager = new GameObject("SceneManager").AddComponent<SceneManager>();
        SceneManager.transform.SetParent(this.transform);
        SceneManager.RegisterSceneLoadHandler(this);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CameraManager != null) CameraManager = null;
        CameraManager = FindObjectOfType<CameraManager>();
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("Game is quitting. Performing cleanup...");

        if (AddressableManager.Instance != null)
        {
            AddressableManager.Instance.ReleaseAll();
        }
        // if (NetworkManager != null)
        // {
        //     NetworkManager.Disconnect(); // 네트워크 연결 종료
        // }
        // if (SaveDataManager != null)
        // {
        //     SaveDataManager.SaveGameData(); // 게임 데이터 저장
        // }
        // 기타 필요한 종료 로직...
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