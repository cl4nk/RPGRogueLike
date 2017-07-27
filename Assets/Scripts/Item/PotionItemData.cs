using Character;
using Managers;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potion", order = 1)]
    public class PotionItemData : UsableItemData
    {
        public int constitution;
        public int dexterity;
        public int intelligence;

        public int life;
        public int mana;
        public int strenght;
        public float time;

        private void OnEnable()
        {
            type = TYPE.POTION;
        }

        public override string ToString()
        {
            return "Strenght : " + strenght + "\nDexterity : " + dexterity + "\nConstitution : " + constitution +
                   "\nIntelligence : " + intelligence + "\nLife : " + life + "\nMana : " + mana + "\nTime : " + time +
                   " s";
        }

        public override void Use(Player player)
        {
            if (mana == 0 && life == 0)
                AlterationManager.Instance.AddAlteration(strenght, constitution, intelligence, dexterity, time, player);
            else
                RegenLifeMana(player);

            player.GetComponent<PlayerInventory>().RemoveItem(this);
        }

        private void RegenLifeMana(Player player)
        {
            player.CurLife += life;
            player.CurMana += mana;
        }
    }
}