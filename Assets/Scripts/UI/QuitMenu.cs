using UnityEngine;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    public enum QUIT_MENU_STATE
    {
        OnDesktopMainMenuChoice,
        OnQuit,
        OnSave
    }

    private InputManager inputMgr;
    private GameObject noButton;
    private Text quitLabel;

    private GameObject returnOnChoice;

    private bool returnOnMainMenu;
    private Text saveLabel;
    private QUIT_MENU_STATE? state;

    private UIGame uiGame;
    private GameObject yesButton;

    public QUIT_MENU_STATE? State
    {
        get { return state; }
        set { StateChanged(value); }
    }

    private void Awake()
    {
        uiGame = transform.parent.GetComponent<UIGame>();
        inputMgr = InputManager.Instance;

        returnOnChoice = transform.Find("ReturnOnChoice").gameObject;
        quitLabel = transform.Find("QuitLabel").GetComponent<Text>();
        saveLabel = transform.Find("SaveLabel").GetComponent<Text>();
        yesButton = transform.Find("YesButton").gameObject;
        noButton = transform.Find("NoButton").gameObject;

        State = QUIT_MENU_STATE.OnDesktopMainMenuChoice;
    }

    public void MainMenuPressed()
    {
        returnOnMainMenu = true;
        State = QUIT_MENU_STATE.OnQuit;
    }

    public void DesktopPressed()
    {
        returnOnMainMenu = false;
        State = QUIT_MENU_STATE.OnQuit;
    }

    public void YesPressed()
    {
        if (state == QUIT_MENU_STATE.OnQuit)
        {
            quitLabel.gameObject.SetActive(false);
            saveLabel.gameObject.SetActive(true);

            State = QUIT_MENU_STATE.OnSave;
        }
        else
        {
            SaveManager.SaveGame(GameManager.Instance.PlayerInstance.gameObject,
                PlayerPrefs.GetInt("LastReachedFloor"));
            if (returnOnMainMenu)
            {
                GameManager.Instance.LoadMainMenu();
                TimeManager.Instance.State = TimeManager.StateGame.Running;
            }
            else
            {
                GameManager.Instance.Quit();
            }
        }
    }

    public void NoPressed()
    {
        if (state == QUIT_MENU_STATE.OnQuit)
        {
            inputMgr.UnlockKey();

            uiGame.playerCam.ToggleCursorLock();
            uiGame.ToggleQuitMenu();
            uiGame.ToggleCrosshair();
            uiGame.TogglePlayerInfo();

            TimeManager.Instance.State = TimeManager.StateGame.Running;
        }
        else
        {
            if (returnOnMainMenu)
            {
                PlayerPrefs.DeleteKey("LastReachedFloor");
                GameManager.Instance.LoadMainMenu();
            }
            else
            {
                GameManager.Instance.Quit();
            }
        }
    }

    private void StateChanged(QUIT_MENU_STATE? newState)
    {
        if (newState != state || state == null)
        {
            state = newState;

            if (state == QUIT_MENU_STATE.OnDesktopMainMenuChoice)
            {
                quitLabel.gameObject.SetActive(false);
                saveLabel.gameObject.SetActive(false);
                yesButton.SetActive(false);
                noButton.SetActive(false);

                returnOnChoice.SetActive(true);
            }
            else if (state == QUIT_MENU_STATE.OnQuit)
            {
                returnOnChoice.SetActive(false);
                saveLabel.gameObject.SetActive(false);

                quitLabel.gameObject.SetActive(true);
                yesButton.SetActive(true);
                noButton.SetActive(true);
            }
            else
            {
                returnOnChoice.SetActive(false);
                quitLabel.gameObject.SetActive(false);

                saveLabel.gameObject.SetActive(true);
                yesButton.SetActive(true);
                noButton.SetActive(true);
            }
        }
    }
}