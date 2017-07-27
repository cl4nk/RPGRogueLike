using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public enum ACTION
    {
        None,
        Use,
        TooHeavy
    }

    private Text actionLabel;
    [SerializeField] private float durationHeavyMessage;

    private InputManager inputMgr;

    private Text keyLabel;
    private GameObject keyPanel;

    private ACTION state = ACTION.None;

    public ACTION State
    {
        get { return state; }
        set { StateChanged(value); }
    }

    private void Awake()
    {
        keyLabel = transform.Find("KeyLabel").GetComponent<Text>();
        actionLabel = transform.Find("ActionLabel").GetComponent<Text>();
        keyPanel = transform.Find("Panel").gameObject;

        inputMgr = InputManager.Instance;
    }

    private void FixedUpdate()
    {
        if (inputMgr.GetCountSubscribersUseIsDown() > 0 && state != ACTION.TooHeavy)
            State = ACTION.Use;
        else if (state != ACTION.TooHeavy)
            State = ACTION.None;
    }

    private void StateChanged(ACTION newState)
    {
        if (newState != state)
        {
            state = newState;

            if (state == ACTION.None)
                HideKeyAction();
            else
                ShowKeyAction();
        }
    }

    private void RefreshText()
    {
        if (state == ACTION.Use)
        {
            keyLabel.text = "E";
            actionLabel.text = "Use";
        }
        else if (state == ACTION.TooHeavy)
        {
            keyLabel.text = "Tab";
            actionLabel.text = "Too much to carry. Drop some items.";
            Invoke("ResetState", durationHeavyMessage);
        }
    }

    private void ShowKeyAction()
    {
        RefreshText();
        keyLabel.gameObject.SetActive(true);
        actionLabel.gameObject.SetActive(true);
        keyPanel.SetActive(true);
    }

    private void HideKeyAction()
    {
        keyLabel.gameObject.SetActive(false);
        actionLabel.gameObject.SetActive(false);
        keyPanel.SetActive(false);
    }

    private void ResetState()
    {
        State = ACTION.None;
    }
}