using Character;
using InteractableObjects;
using Managers;
using UI;
using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(BoxCollider))]
    public class Item : InteractableObject
    {
        private GameObject itemPrefab;

        [SerializeField] private ItemData itemReference;

        public ItemData ItemReference
        {
            get { return itemReference; }
            set
            {
                itemReference = value;
                itemPrefab = itemReference.prefab;
            }
        }

        private void Start()
        {
            if (itemReference != null)
                itemPrefab = itemReference.prefab;

            GetComponent<SphereCollider>().isTrigger = true;
            if (itemReference.Type == ItemData.TYPE.PARCHMENT)
                GetComponent<SphereCollider>().radius = 600f;
            else
                GetComponent<SphereCollider>().radius = 2.5f;
        }

        public override void OnUse()
        {
            PlayerController player = GameManager.Instance.PlayerInstance;

            if (!player.Inventory.AddItem(itemReference))
            {
                UIGame.Instance.PlayerCantCarry();
                return;
            }

            InputManager.Instance.useIsDown -= OnUse;
            Destroy(gameObject);
        }
    }
}