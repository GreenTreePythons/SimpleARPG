using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; }

    public InputManager InputManager { get; private set; }
    public CameraManager CameraManager { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputManager = new GameObject("InputManager").AddComponent<InputManager>();
        InputManager.transform.SetParent(this.transform);

        CameraManager = FindObjectOfType<CameraManager>();;
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