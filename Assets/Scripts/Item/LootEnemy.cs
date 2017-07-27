using InteractableObjects;
using UI;

namespace Item
{
    public class LootEnemy : InteractableObject
    {
        private LootInventory lootInventory;
        private UIGame uiGame;

        private void Start()
        {
            uiGame = UIGame.Instance;

            lootInventory = gameObject.GetComponent<LootInventory>();
        }

        public override void OnUse()
        {
            UIGame.Instance.ToggleInventoryWindow(lootInventory);
            inputMgr.LockInventory();
            uiGame.playerCam.ToggleCursorLock();
        }
    }
}