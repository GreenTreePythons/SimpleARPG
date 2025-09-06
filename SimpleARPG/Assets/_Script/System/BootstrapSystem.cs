using UnityEngine;

public static class BootstrapSystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        var root = new GameObject("ManagersRoot");
        Object.DontDestroyOnLoad(root);

        CreateManager<GameManager>(root.transform, "GameManager");
    }

    private static void CreateManager<T>(Transform root, string name) where T : Component
    {
        if (Object.FindObjectOfType<T>(true) != null) return;

        var go = new GameObject(name);
        go.transform.SetParent(root);
        go.AddComponent<T>();
    }
}