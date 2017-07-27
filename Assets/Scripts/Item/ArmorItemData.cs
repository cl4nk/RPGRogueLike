using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor", order = 1)]
    public class ArmorItemData : EquipableItemData
    {
        public enum SLOT
        {
            HEAD,
            CHEST,
            LEGS,
            SHOOES
        }

        public SLOT slot;

        private void OnEnable()
        {
            type = TYPE.ARMOR;
        }

        public bool IsBetterThan(ArmorItemData armorToCompare)
        {
            if (!armorToCompare)
                return false;

            if (constitution > armorToCompare.constitution)
                return true;

            return false;
        }
    }
}