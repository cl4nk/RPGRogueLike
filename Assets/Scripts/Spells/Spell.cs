using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "SpellList", order = 1)]
public abstract class Spell : ScriptableObject
{
    public int costMana;
    public string description = "Spell description";
    public new string name = "Spell Name";
    public string path = "Spells/";
    public int shortcut = -1;
    public Sprite sprite;
}