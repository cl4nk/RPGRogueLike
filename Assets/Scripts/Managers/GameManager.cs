using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnStateDelegate();

    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Pause
    }

    private static GameManager instance;

    private GameState state = GameState.MainMenu;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    public PlayerController PlayerInstance { get; set; }

    public event OnStateDelegate OnMainMenu;
    public event OnStateDelegate OnLoading;
    public event OnStateDelegate OnPlaying;
    public event OnStateDelegate OnPause;

    private void Start()
    {
        if (OnMainMenu != null)
            OnMainMenu();
        LevelManager.Instance.OnLoadingFinish += () =>
        {
            if (OnPlaying != null)
                OnPlaying();
        };
    }

    public void LoadFirstLevel()
    {
        Destroy(GameObject.Find("Main Camera"));
        Destroy(GameObject.Find("EventSystem"));
        LevelManager.Instance.AsyncLoadLevel(LevelManager.FIRST_LEVEL_ID);
        LevelManager.Instance.Init();
    }

    public void LoadMainMenu()
    {
        LevelManager.Instance.LoadMainMenu();
    }

    public void LoadLastSave()
    {
        Destroy(GameObject.Find("EventSystem"));
        LevelManager.Instance.Place = LevelManager.PLACES.CampFire;
        LevelManager.Instance.InitSavedGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveManager.RemoveCurrentFloor();
    }
}