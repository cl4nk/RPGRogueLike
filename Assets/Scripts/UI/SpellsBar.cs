using UnityEngine;
using UnityEngine.UI;

public class SpellsBar : MonoBehaviour
{
    private Image selecSlot;
    private SpellSlot[] spellsSlots;

    private void Awake()
    {
        spellsSlots = GetComponentsInChildren<SpellSlot>();
        selecSlot = transform.Find("SelecSlot").GetComponent<Image>();
    }

    public void SelectSpell(int index)
    {
        selecSlot.transform.position = spellsSlots[index].transform.position;
    }

    public void ChangeSpellSlot(Spell spell, int newIndex)
    {
        int prevIndex = GetIndexSpell(spell);

        if (prevIndex != -1)
            spellsSlots[prevIndex].ChangeSpell(null);

        if (newIndex != -1)
            spellsSlots[newIndex].ChangeSpell(spell);
    }

    private int GetIndexSpell(Spell spell)
    {
        for (int index = 0; index < spellsSlots.Length; index++)
            if (spellsSlots[index].nameSpell == spell.name)
                return index;

        return -1;
    }
}