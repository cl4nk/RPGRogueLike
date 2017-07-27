using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum StateGame
    {
        Running,
        Paused
    }

    private static TimeManager instance;

    private StateGame state = StateGame.Running;

    public static TimeManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<TimeManager>();

            return instance;
        }
    }

    public StateGame State
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                state = value;
                StateChanged();
            }
        }
    }

    private void StateChanged()
    {
        if (state == StateGame.Running)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }
}