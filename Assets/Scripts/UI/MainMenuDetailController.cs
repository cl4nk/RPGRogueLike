using UnityEngine;
using UnityEngine.UI;

public class MainMenuDetailController : MonoBehaviour
{
    private GameObject cancelBtn;
    private Text continueLabel;

    private NavigableMenu navMenu;

    private Text newGameLabel;
    private Text quitLabel;

    private State state;

    private GameObject validateBtn;

    // Use this for initialization
    private void Awake()
    {
        validateBtn = transform.Find("AcceptBtn").gameObject;
        cancelBtn = transform.Find("CancelBtn").gameObject;
        newGameLabel = transform.Find("NewGameLabel").GetComponent<Text>();
        continueLabel = transform.Find("ContinueLabel").GetComponent<Text>();
        quitLabel = transform.Find("QuitLabel").GetComponent<Text>();

        navMenu = GetComponent<NavigableMenu>();
        if (navMenu)
            navMenu.OnUpdateData += index =>
            {
                HideLabels();
                switch (index)
                {
                    case 0:
                        OnContinueClick();
                        break;
                    case 1:
                        OnNewGameClick();
                        break;
                    case 2:
                        OnQuitClick();
                        break;
                    default:
                        break;
                }
            };

        state = State.None;
    }

    private void HideLabels()
    {
        if (newGameLabel)
            newGameLabel.enabled = false;
        if (continueLabel)
            continueLabel.enabled = false;
        if (quitLabel)
            quitLabel.enabled = false;
    }

    private void HideChoiceButtons()
    {
        if (validateBtn)
            validateBtn.SetActive(false);
        if (cancelBtn)
            cancelBtn.SetActive(false);
    }

    private void ShowChoiceButtons()
    {
        if (validateBtn)
            validateBtn.SetActive(true);
        if (cancelBtn)
            cancelBtn.SetActive(true);
    }


    private void OnContinueClick()
    {
        HideLabels();
        if (continueLabel)
            continueLabel.enabled = true;
        state = State.Continue;
        ShowChoiceButtons();
    }

    private void OnNewGameClick()
    {
        HideLabels();
        if (newGameLabel)
            newGameLabel.enabled = true;
        state = State.NewGame;
        ShowChoiceButtons();
    }

    private void OnQuitClick()
    {
        HideLabels();
        if (quitLabel)
            quitLabel.enabled = true;
        state = State.Quit;
        ShowChoiceButtons();
    }

    public void OnValidateButton()
    {
        switch (state)
        {
            case State.None:
                break;
            case State.Continue:
                if (PlayerPrefs.HasKey("LastSavedReachedFloor"))
                    GameManager.Instance.LoadLastSave();
                else
                    GameManager.Instance.LoadFirstLevel();
                break;
            case State.NewGame:
                SaveManager.RemoveSaves();
                GameManager.Instance.LoadFirstLevel();
                break;
            case State.Quit:
                GameManager.Instance.Quit();
                break;
            default:
                break;
        }
    }

    public void OnCancelButton()
    {
        state = State.None;
        GetComponentInParent<HierarchyMenu>().OnClickBack();
    }

    private enum State
    {
        None,
        Continue,
        NewGame,
        Quit
    }
}