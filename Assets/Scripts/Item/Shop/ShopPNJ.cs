using InteractableObjects;
using UI;

namespace Item.Shop
{
    public class ShopPNJ : InteractableObject
    {
        private ShopInventory shopInventory;
        private UIGame uiGame;

        private void Start()
        {
            uiGame = UIGame.Instance;

            shopInventory = gameObject.GetComponent<ShopInventory>();
        }

        public override void OnUse()
        {
            UIGame.Instance.ToggleInventoryWindow(shopInventory);
            inputMgr.LockInventory();
            uiGame.playerCam.ToggleCursorLock();
        }
    }
}