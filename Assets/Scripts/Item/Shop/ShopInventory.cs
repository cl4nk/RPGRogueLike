using System;
using Managers;

namespace Item.Shop
{
    [Serializable]
    public class ShopInventory : Inventory
    {
        private PlayerInventory playerInventory;

        protected override void Start()
        {
            base.Start();

            playerInventory = GameManager.Instance.PlayerInstance.GetComponent<PlayerInventory>();
        }

        public bool BuyItem(ItemData item)
        {
            if (player.Gold >= item.value && playerInventory.AddItem(item))
            {
                player.Gold -= item.value;
                RemoveFromShop(item);
                return true;
            }

            return false;
        }

        private void RemoveFromShop(ItemData item)
        {
            Items.Remove(item);
        }
    }
}