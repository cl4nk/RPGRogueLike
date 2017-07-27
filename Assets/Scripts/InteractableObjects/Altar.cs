using Item;
using Managers;
using UnityEngine;

namespace InteractableObjects
{
    public class Altar : InteractableObject
    {
        public override void OnUse()
        {
            PlayerInventory playerInventory = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerInventory>();
            ItemData item = GoalManager.Instance.HasHeTheQuestItem(playerInventory);

            if (item)
            {
                playerInventory.RemoveItem(item);

                GameObject itemToDrop;
                itemToDrop = Instantiate(item.prefab);
                itemToDrop.transform.position = transform.GetChild(0).position;

                itemToDrop.AddComponent<Item.Item>();
                itemToDrop.GetComponent<Item.Item>().ItemReference = item;

                LevelManager.Instance.GameIsOver("Win");
            }
        }
    }
}