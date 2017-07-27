using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon", order = 1)]
public class WeaponItemData : EquipableItemData
{
    public enum WEAPON_TYPE
    {
        WEAPON,
        SHIELD
    }

    public bool twoHand;

    public WEAPON_TYPE weaponType;

    private void OnEnable()
    {
        type = TYPE.WEAPON;
    }

    public bool IsBetterThan(WeaponItemData weaponToCompare)
    {
        if (weaponToCompare != null && strenght > weaponToCompare.strenght)
            return true;
        return false;
    }
}