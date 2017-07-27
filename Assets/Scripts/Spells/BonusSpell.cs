using UnityEngine;

[CreateAssetMenu(fileName = "BonusSpell", menuName = "SpellList/BonusSpells", order = 1)]
public class BonusSpell : Spell
{
    public enum BONUS_TYPE
    {
        Health,
        Mana,
        Strength,
        Constitution,
        AtkSpeed
    }

    public BONUS_TYPE bonusType;

    public int bonusValue;
    public float duration;
    public bool isPermanent;
}