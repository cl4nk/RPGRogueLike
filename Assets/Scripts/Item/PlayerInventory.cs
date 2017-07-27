using System;
using Character;
using UnityEngine;

namespace Item
{
    [Serializable]
    public class PlayerInventory : Inventory
    {
        private Equipment equipment;

        public Equipment Equipment
        {
            get { return equipment; }
            set { equipment = value; }
        }

        private void Awake()
        {
            equipment = GetComponent<Equipment>();
            CalculWeight();
        }

        public bool EquipItem(ItemData item)
        {
            if (player.Level >= item.levelRequired)
            {
                equipment.EquipItem(item);
                return true;
            }

            return false;
        }

        public void DropItem(ItemData item)
        {
            Items.Remove(item);

            equipment.UnequipItem(item);

            GameObject itemToDrop;
            itemToDrop = Instantiate(item.prefab);
            itemToDrop.transform.position = transform.position + transform.forward * 3;

            itemToDrop.AddComponent<Item>();
            itemToDrop.GetComponent<Item>().ItemReference = item;

            CalculWeight();
        }

        public bool AddItem(ItemData item)
        {
            if (CanTake(item))
            {
                Items.Add(item);
                CalculWeight();

                return true;
            }

            return false;
        }

        public void UseItem(ItemData item)
        {
            UsableItemData usableItem = item as UsableItemData;

            if (usableItem)
            {
                usableItem.Use(GetComponent<Player>());
                CalculWeight();
            }
        }

        public void RemoveItem(ItemData item)
        {
            Items.Remove(item);
            CalculWeight();
        }

        private void CalculWeight()
        {
            int weight = 0;
            for (int i = 0; i < Items.Count; ++i)
                weight += Items[i].weight;

            GetComponent<Player>().CurWeight = weight;
        }
    }
}