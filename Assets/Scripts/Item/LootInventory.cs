using Managers;

namespace Item
{
    public class LootInventory : Inventory
    {
        private PlayerInventory playerInventory;

        protected override void Start()
        {
            base.Start();

            playerInventory = GameManager.Instance.PlayerInstance.GetComponent<PlayerInventory>();
        }

        public bool AddItem(ItemData item)
        {
            if (CanTake(item))
            {
                playerInventory.AddItem(item);
                Remove(item);

                return true;
            }
            return false;
        }

        private void Remove(ItemData item)
        {
            Items.Remove(item);
        }
    }
}