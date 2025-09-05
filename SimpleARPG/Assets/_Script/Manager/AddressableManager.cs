using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance { get; private set; }

    private Dictionary<string, AsyncOperationHandle> m_LoadedHandles = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public AsyncOperationHandle<T> LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        if (m_LoadedHandles.ContainsKey(address) && m_LoadedHandles[address].IsValid())
        {
            return m_LoadedHandles[address].Convert<T>();
        }
        
        var handle = Addressables.LoadAssetAsync<T>(address);
        m_LoadedHandles[address] = handle;
        return handle;
    }
    
    public void Release(string address)
    {
        if (m_LoadedHandles.ContainsKey(address) && m_LoadedHandles[address].IsValid())
        {
            Addressables.Release(m_LoadedHandles[address]);
            m_LoadedHandles.Remove(address);
        }
    }

    public void ReleaseAll()
    {
        foreach (var handle in m_LoadedHandles.Values)
        {
            if (!handle.IsValid()) continue;
            Addressables.Release(handle);
        }
        m_LoadedHandles.Clear();
    }
}