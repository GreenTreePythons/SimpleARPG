using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneManager : MonoBehaviour
{
    private ISceneLoadHandler m_SceneLoadHandler;
    
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleSceneLoaded;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RegisterSceneLoadHandler(ISceneLoadHandler handler)
    {
        m_SceneLoadHandler = handler;
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (AddressableManager.Instance != null)
        {
            AddressableManager.Instance.ReleaseAll();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
    }

    private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (m_SceneLoadHandler != null)
        {
            m_SceneLoadHandler.OnSceneLoaded(scene, mode);
        }
    }
}

public interface ISceneLoadHandler
{
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode);
}