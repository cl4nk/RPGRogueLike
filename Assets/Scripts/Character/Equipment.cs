using Item;
using UnityEngine;

namespace Character
{
    public class Equipment : MonoBehaviour
    {
        public delegate void EquipmentChange(WeaponItemData weapon);

        private Character charac;

        [SerializeField] private ArmorItemData chestEquipment;

        [SerializeField] private ArmorItemData headEquipment;

        [SerializeField] private WeaponItemData leftWeapon;

        [SerializeField] private ArmorItemData legsEquipment;

        [SerializeField] private WeaponItemData rightWeapon;

        [SerializeField] private ArmorItemData shooesEquipment;

        public WeaponItemData RightWeapon
        {
            get { return rightWeapon; }
            set
            {
                RecalculateBonuses(rightWeapon, value);
                rightWeapon = value;
            }
        }

        public WeaponItemData LeftWeapon
        {
            get { return leftWeapon; }
            set
            {
                RecalculateBonuses(leftWeapon, value);
                leftWeapon = value;
            }
        }

        public ArmorItemData HeadEquipment
        {
            get { return headEquipment; }
            set
            {
                RecalculateBonuses(headEquipment, value);
                headEquipment = value;
            }
        }

        public ArmorItemData ChestEquipment
        {
            get { return chestEquipment; }
            set
            {
                RecalculateBonuses(chestEquipment, value);
                chestEquipment = value;
            }
        }

        public ArmorItemData LegsEquipment
        {
            get { return legsEquipment; }
            set
            {
                RecalculateBonuses(legsEquipment, value);
                legsEquipment = value;
            }
        }

        public ArmorItemData ShooesEquipment
        {
            get { return shooesEquipment; }
            set
            {
                RecalculateBonuses(shooesEquipment, value);
                shooesEquipment = value;
            }
        }

        public event EquipmentChange onEquipmentChange;

        public void RaiseOnEquipmentChange(WeaponItemData weapon)
        {
            if (onEquipmentChange != null) onEquipmentChange(weapon);
        }

        public void EquipItem(ItemData item)
        {
            if (item.Type == ItemData.TYPE.ARMOR)
                EquipArmor(item as ArmorItemData);
            else if (item.Type == ItemData.TYPE.WEAPON)
                EquipWeapon(item as WeaponItemData);
        }

        public void UnequipItem(ItemData item)
        {
            if (item.Type == ItemData.TYPE.ARMOR)
                UnequipArmor(item as ArmorItemData);
            else if (item.Type == ItemData.TYPE.WEAPON)
                UnequipWeapon(item as WeaponItemData);
        }

        private void UnequipArmor(ArmorItemData armor)
        {
            switch (armor.slot)
            {
                case ArmorItemData.SLOT.CHEST:
                    if (armor == chestEquipment)
                        ChestEquipment = null;
                    break;

                case ArmorItemData.SLOT.HEAD:
                    if (armor == headEquipment)
                        HeadEquipment = null;
                    break;

                case ArmorItemData.SLOT.LEGS:
                    if (armor == legsEquipment)
                        LegsEquipment = null;
                    break;

                case ArmorItemData.SLOT.SHOOES:
                    if (armor == shooesEquipment)
                        ShooesEquipment = null;
                    break;
            }
        }

        private void UnequipWeapon(WeaponItemData weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponItemData.WEAPON_TYPE.SHIELD:
                    if (weapon == LeftWeapon)
                        LeftWeapon = null;
                    break;

                case WeaponItemData.WEAPON_TYPE.WEAPON:
                    if (weapon == RightWeapon)
                        RightWeapon = null;
                    break;
            }
            RaiseOnEquipmentChange(weapon);
        }

        private void EquipArmor(ArmorItemData armor)
        {
            switch (armor.slot)
            {
                case ArmorItemData.SLOT.CHEST:
                    if (armor == chestEquipment)
                        ChestEquipment = null;
                    else
                        ChestEquipment = armor;
                    break;

                case ArmorItemData.SLOT.HEAD:
                    if (armor == headEquipment)
                        HeadEquipment = null;
                    else
                        HeadEquipment = armor;
                    break;

                case ArmorItemData.SLOT.LEGS:
                    if (armor == legsEquipment)
                        LegsEquipment = null;
                    else
                        LegsEquipment = armor;
                    break;

                case ArmorItemData.SLOT.SHOOES:
                    if (armor == shooesEquipment)
                        ShooesEquipment = null;
                    else
                        ShooesEquipment = armor;
                    break;
            }
        }

        private void EquipWeapon(WeaponItemData weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponItemData.WEAPON_TYPE.SHIELD:
                    if (weapon == LeftWeapon)
                        LeftWeapon = null;
                    else
                        LeftWeapon = weapon;
                    break;

                case WeaponItemData.WEAPON_TYPE.WEAPON:
                    if (weapon == RightWeapon)
                        RightWeapon = null;
                    else
                        RightWeapon = weapon;
                    break;
            }

            RaiseOnEquipmentChange(weapon);
        }

        private void Awake()
        {
            charac = gameObject.GetComponent<Character>();
        }

        private void Start()
        {
            RecalculateBonuses(null, rightWeapon);
            RecalculateBonuses(null, leftWeapon);
            RecalculateBonuses(null, headEquipment);
            RecalculateBonuses(null, chestEquipment);
            RecalculateBonuses(null, legsEquipment);
            RecalculateBonuses(null, shooesEquipment);
        }

        private void RecalculateBonuses(EquipableItemData prevItem, EquipableItemData newItem)
        {
            if (prevItem)
            {
                charac.Strength -= prevItem.strenght;
                charac.Dexterity -= prevItem.dexterity;
                charac.Constitution -= prevItem.constitution;
                charac.Intelligence -= prevItem.intelligence;
            }

            if (newItem)
            {
                charac.Strength += newItem.strenght;
                charac.Dexterity += newItem.dexterity;
                charac.Constitution += newItem.constitution;
                charac.Intelligence += newItem.intelligence;
            }

            charac.RecalculateCharacterStats();
        }
    }
}