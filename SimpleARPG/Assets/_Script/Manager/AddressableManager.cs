using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance { get; private set; }

    // 로드된 에셋 핸들을 저장하는 딕셔너리
    private Dictionary<string, AsyncOperationHandle> loadedHandles = new();

    private void Awake()
    {
        // 인스턴스가 이미 존재하면 새로 생성된 오브젝트를 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // 씬이 변경되어도 유지
        }
    }

    /// <summary>
    /// 문자열 주소(Address)를 사용하여 비동기적으로 에셋을 로드합니다.
    /// </summary>
    /// <typeparam name="T">로드할 에셋의 타입</typeparam>
    /// <param name="address">데이터 테이블에 저장된 에셋의 주소</param>
    /// <returns>비동기 작업 핸들</returns>
    public AsyncOperationHandle<T> LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        if (loadedHandles.ContainsKey(address) && loadedHandles[address].IsValid())
        {
            return loadedHandles[address].Convert<T>();
        }
        
        var handle = Addressables.LoadAssetAsync<T>(address);
        loadedHandles[address] = handle;
        return handle;
    }
    
    /// <summary>
    /// 특정 주소의 에셋을 메모리에서 해제합니다.
    /// </summary>
    /// <param name="address">해제할 에셋의 주소</param>
    public void Release(string address)
    {
        if (loadedHandles.ContainsKey(address) && loadedHandles[address].IsValid())
        {
            Addressables.Release(loadedHandles[address]);
            loadedHandles.Remove(address);
        }
    }

    /// <summary>
    /// 로드된 모든 에셋을 메모리에서 해제합니다.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var handle in loadedHandles.Values)
        {
            if (!handle.IsValid()) continue;
            Addressables.Release(handle);
        }
        loadedHandles.Clear();
    }
}