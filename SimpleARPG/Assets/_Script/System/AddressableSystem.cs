using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class AddressableSystem
{
    private static readonly AddressableSystem m_Instance = new();
    public static AddressableSystem Instance => m_Instance;

    private readonly Dictionary<string, AsyncOperationHandle> m_OperationHandles = new();

    private AddressableSystem()
    {
        Application.quitting += ReleaseAll;
    }

    public async Task<T> LoadAsync<T>(string address) where T : Object
    {
        if (m_OperationHandles.TryGetValue(address, out var cached) && cached.IsValid()) return cached.Convert<T>().Result;

        var handle = Addressables.LoadAssetAsync<T>(address);
        m_OperationHandles[address] = handle;
        var result = await handle.Task;
        return result;
    }

    public void Release(string address)
    {
        if (m_OperationHandles.TryGetValue(address, out var h) && h.IsValid())
        {
            Addressables.Release(h);
            m_OperationHandles.Remove(address);
        }
    }

    public void ReleaseAll()
    {
        foreach (var h in m_OperationHandles.Values)
        {
            if (h.IsValid()) Addressables.Release(h);
        }
        m_OperationHandles.Clear();
    }
}