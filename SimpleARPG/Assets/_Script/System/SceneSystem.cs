using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public sealed class SceneSystem
{
    private static readonly SceneSystem m_Instance = new();
    private ISceneLoadHandler m_SceneLoadHandler;
    
    public static SceneSystem Instance => m_Instance;

    private SceneSystem()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    public void RegisterSceneLoadHandler(ISceneLoadHandler handler)
    {
        m_SceneLoadHandler = handler;
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AddressableSystem.Instance?.ReleaseAll();
        SceneManager.LoadScene(sceneName, mode);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_SceneLoadHandler?.OnSceneLoaded(scene, mode);
    }
}

public interface ISceneLoadHandler
{
    void OnSceneLoaded(Scene scene, LoadSceneMode mode);
}