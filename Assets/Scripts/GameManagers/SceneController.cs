using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    Title,
    Operation,
    Playing,
    GameClear,
    GameOver,
}

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public SceneState SceneStateV { get; private set; }

    public string
        titleSceneName,
        operationSceneName,
        playingSceneName,
        GameClearSceneName,
        GameOverSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TitleScene();

        if (Input.GetKeyDown(KeyCode.Return)) OperationScene();
    }

    public void TitleScene()
    {
        SceneStateV = SceneState.Title;
        SceneManager.LoadScene(titleSceneName);
    }

    public void OperationScene()
    {
        SceneStateV = SceneState.Operation;
        SceneManager.LoadScene(operationSceneName);
    }

    public void PlayingScene()
    {
        SceneStateV = SceneState.Playing;
        SceneManager.LoadScene(playingSceneName);
    }

    public void GameClearScene()
    {
        SceneStateV = SceneState.GameClear;
        SceneManager.LoadScene(GameClearSceneName);
    }

    public void GameOverScene()
    {
        SceneStateV = SceneState.GameOver;
        SceneManager.LoadScene(GameOverSceneName);
    }
}
