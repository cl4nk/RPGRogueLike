using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour
{
    public string nameSpell;

    private Image spellSprite;

    private void Awake()
    {
        spellSprite = transform.Find("SpellSprite").GetComponent<Image>();
    }

    public void ChangeSpell(Spell spell)
    {
        if (spell)
        {
            nameSpell = spell.name;
            spellSprite.sprite = spell.sprite;
            spellSprite.gameObject.SetActive(true);
        }
        else
        {
            nameSpell = "";
            spellSprite.sprite = null;
            spellSprite.gameObject.SetActive(false);
        }
    }
}