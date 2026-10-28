using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public static GameManager Instance { get; private set;}
    public GameState CurrentState { get; private set;}

    GameState initialState;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
                return;
        }

        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Gameplay);
    }

    public void ChangeState(GameState newstate)
    {
        if (CurrentState == newstate) return;

        ExitState(CurrentState);
        CurrentState = newstate;
        EnterState(CurrentState);
    }


   public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
        }
    }

    public void ExitState(GameState state)
    {

    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        ChangeState(GameState.Gameplay);
        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Pause) return;

        ChangeState(GameState.Gameplay);

    }

    private void PauseGame()
    {
        if (CurrentState != GameState.Gameplay) return;
        
        ChangeState(GameState.Gameplay);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        Instance = null;
        Time.timeScale = 1f;
    }
}
