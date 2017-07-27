using UnityEngine;

[CreateAssetMenu(fileName = "New Parchment", menuName = "Items/Parchment", order = 1)]
public class ParchmentItemData : UsableItemData
{
    private void OnEnable()
    {
        type = TYPE.PARCHMENT;
    }

    public override void Use(Player player)
    {
        LevelManager levelMgr = LevelManager.Instance;

        if (levelMgr.Place != LevelManager.PLACES.CampFire)
        {
            player.GetComponent<PlayerInventory>().RemoveItem(this);
            levelMgr.Place = LevelManager.PLACES.CampFire;
        }
    }
}