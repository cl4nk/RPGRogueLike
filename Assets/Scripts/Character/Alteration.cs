using System.Collections;
using UnityEngine;

public class Alteration : MonoBehaviour
{
    public enum POTIONSTATES
    {
        Neutral = 0,
        InUsed,
        Used,
        nbStates
    }

    private int constitution;
    private int dexterity;
    private int intelligence;
    private POTIONSTATES potionState = POTIONSTATES.Neutral;

    private int strenght;

    private Character target;

    public POTIONSTATES PotionState
    {
        get { return potionState; }
    }

    public float Time { get; private set; }

    public void Init(int _strenght, int _constitution, int _intelligence, int _dexterity, float _time,
        Character character)
    {
        strenght = _strenght;
        constitution = _constitution;
        intelligence = _intelligence;
        dexterity = _dexterity;
        Time = _time;

        target = character;
    }

    public IEnumerator TimeAlteraction()
    {
        Upgrade();
        yield return new WaitForSeconds(Time);
        Downgrade();
    }

    private void Upgrade()
    {
        if (target)
        {
            potionState = POTIONSTATES.InUsed;
            target.Strength += strenght;
            target.Constitution += constitution;
            target.Intelligence += intelligence;
            target.Dexterity += dexterity;
        }
        else
        {
            AlterationManager.Instance.RemoveAlteration(this);
        }
    }

    private void Downgrade()
    {
        if (target)
        {
            potionState = POTIONSTATES.Used;
            target.Strength -= strenght;
            target.Constitution -= constitution;
            target.Intelligence -= intelligence;
            target.Dexterity -= dexterity;
        }

        AlterationManager.Instance.RemoveAlteration(this);
    }
}