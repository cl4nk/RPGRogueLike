using Item;
using UnityEngine;

namespace Managers
{
    public class GoalManager : MonoBehaviour
    {
        private static GoalManager instance;

        public static GoalManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GoalManager>();
                return instance;
            }
        }

        public ItemData HasHeTheQuestItem(PlayerInventory inventory)
        {
            foreach (ItemData item in inventory.Items)
                if (item.isQuestItem)
                    return item;

            return null;
        }
    }
}