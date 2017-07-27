using System.Collections.Generic;
using Character;
using InteractableObjects;
using Item;
using Item.Shop;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryPanel : MonoBehaviour
    {
        public enum FILTER
        {
            All = 0,
            Weapons,
            Clothes,
            Usables,
            None
        }

        public enum TYPE_INVENTORY
        {
            Player,
            Shop,
            Sell,
            Loot,
            Count,
            None = -1
        }

        private Transform actionItemPanel;
        private Button currentButtonPreview;
        private List<Button> currentButtons;
        private ItemData currentItemPreview;
        private FILTER currFilter = FILTER.None;
        private TYPE_INVENTORY currType = TYPE_INVENTORY.None;
        private Transform descriptionPanel;

        private Transform filterPanel;

        private Inventory inventoryToShow;

        private bool isSellingItem;

        private Button itemButtonPrefab;
        private Transform itemDataPanel;
        private Transform itemsContainer;
        private Transform itemsPanel;

        private Transform menuItemPanel;

        private Inventory oldInventory;
        private Player player;
        private Equipment playerEquipment;
        private Transform playerInfoPanel;

        private Transform previewPanel;

        private PreviewRoom previewRoom;
        private Transform viewItemPanel;

        private void Awake()
        {
            menuItemPanel = transform.Find("MenuItemPanel");
            playerInfoPanel = transform.Find("PlayerInfoPanel");

            InitFilterPanel();

            itemsPanel = menuItemPanel.Find("ItemsPanel");
            itemsContainer = itemsPanel.Find("ItemsContainer");

            InitPreviewPanel();

            itemButtonPrefab = Resources.Load<Button>("Prefabs/ItemButton");

            currentButtons = new List<Button>();
            inventoryToShow = null;
            currentItemPreview = null;
            currentButtonPreview = null;
        }

        private void Start()
        {
            playerEquipment = GameManager.Instance.PlayerInstance.Equipment;
            player = GameManager.Instance.PlayerInstance.Player;

            InputManager.Instance.shopIsDown += ToggleBuySellFunction;
            previewRoom = FindObjectOfType<PreviewRoom>();
            HideInventory();
        }

        public void ShowInventory(Inventory inventory)
        {
            gameObject.SetActive(true);
            RefreshPlayerInfo();
            previewPanel.gameObject.SetActive(false);
            inventoryToShow = inventory;
            SetCurrentType();
        }

        public void HideInventory()
        {
            if (inventoryToShow is LootInventory)
            {
                Chest chest = inventoryToShow.GetComponent<Chest>();
                if (chest != null)
                    chest.OnUse();
            }

            gameObject.SetActive(false);
            inventoryToShow = null;
            currentItemPreview = null;
            currentButtonPreview = null;
            previewRoom.ResetPreviewPanel();
            itemsContainer.localPosition = new Vector3(0, -itemsPanel.localPosition.y, 0);
            CleanItemsButtons();
        }

        public void ResetState()
        {
            isSellingItem = false;
            currType = TYPE_INVENTORY.None;
        }

        private void RefreshPlayerInfo()
        {
            playerInfoPanel.Find("GoldText").GetComponent<Text>().text = player.Gold + " Gold";
            playerInfoPanel.Find("GoldText").GetComponent<Text>().color = Color.yellow;

            playerInfoPanel.Find("WeightText").GetComponent<Text>().text =
                player.CurWeight + " / " + player.MaxWeight + " Weight";

            float ratio = player.CurWeight / (float) player.MaxWeight * 100;

            if (ratio < 50)
                playerInfoPanel.Find("WeightText").GetComponent<Text>().color = Color.green;
            else if (ratio > 90)
                playerInfoPanel.Find("WeightText").GetComponent<Text>().color = Color.red;
            else
                playerInfoPanel.Find("WeightText").GetComponent<Text>().color = Color.white;
        }

        private void ShowPreviewPanel()
        {
            if (!previewPanel.gameObject.activeSelf)
                previewPanel.gameObject.SetActive(true);
        }

        private void InitFilterPanel()
        {
            filterPanel = menuItemPanel.Find("FilterPanel");

            filterPanel.Find("NoFilter").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowAll();
                itemsContainer.localPosition = new Vector3(0, -itemsPanel.localPosition.y, 0);
            });

            filterPanel.Find("WeaponFilter").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowWeapons();
                itemsContainer.localPosition = new Vector3(0, -itemsPanel.localPosition.y, 0);
            });

            filterPanel.Find("ClothesFilter").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowClothes();
                itemsContainer.localPosition = new Vector3(0, -itemsPanel.localPosition.y, 0);
            });

            filterPanel.Find("UsablesFilter").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowUsables();
                itemsContainer.localPosition = new Vector3(0, -itemsPanel.localPosition.y, 0);
            });
        }

        private void InitPreviewPanel()
        {
            previewPanel = menuItemPanel.Find("PreviewPanel");

            descriptionPanel = previewPanel.Find("DescriptionPanel");
            itemDataPanel = descriptionPanel.Find("ItemDataPanel");
            viewItemPanel = previewPanel.Find("ViewItemPanel");
            actionItemPanel = descriptionPanel.Find("ActionItemPanel");
        }

        private void SetCurrentType()
        {
            if (inventoryToShow is PlayerInventory)
                if (isSellingItem)
                    currType = TYPE_INVENTORY.Sell;
                else currType = TYPE_INVENTORY.Player;
            else if (inventoryToShow is ShopInventory)
                currType = TYPE_INVENTORY.Shop;
            else if (inventoryToShow is LootInventory)
                currType = TYPE_INVENTORY.Loot;
        }

        private void ShowAll()
        {
            currFilter = FILTER.All;
            CleanItemsButtons();
            CreateItemButtons();
        }

        private void ShowWeapons()
        {
            currFilter = FILTER.Weapons;
            CleanItemsButtons();
            CreateItemButtons(ItemData.TYPE.WEAPON);
        }

        private void ShowClothes()
        {
            currFilter = FILTER.Clothes;
            CleanItemsButtons();
            CreateItemButtons(ItemData.TYPE.ARMOR);
        }

        private void ShowUsables()
        {
            currFilter = FILTER.Usables;
            CleanItemsButtons();
            CreateItemButtons(ItemData.TYPE.POTION);
        }

        private void CreateItemButtons()
        {
            for (int i = 0; i < inventoryToShow.Items.Count; ++i)
                CreateButton(inventoryToShow.Items[i]);
        }

        private void CreateItemButtons(ItemData.TYPE filter)
        {
            for (int i = 0; i < inventoryToShow.Items.Count; ++i)
            {
                ItemData currentItem = inventoryToShow.Items[i];

                if (currentItem.Type == filter)
                    CreateButton(currentItem);
            }
        }

        private void CreateButton(ItemData item)
        {
            if (item == null)
                return;

            Button button = Instantiate(itemButtonPrefab);
            button.GetComponent<ItemButton>().itemData = item;

            button.transform.SetParent(itemsContainer);
            button.transform.Find("Text").GetComponent<Text>().text = item.name;

            button.onClick.AddListener(() =>
            {
                PreviewItem(item);
                currentButtonPreview = button;
                currentItemPreview = button.GetComponent<ItemButton>().itemData;
                ShowPreviewPanel();
                previewRoom.UpdatePreviewPanel(currentItemPreview);
            });

            currentButtons.Add(button);
            CompareEquipment(button);
        }

        private void CleanItemsButtons()
        {
            for (int i = 0; i < itemsContainer.childCount; ++i)
            {
                currentButtons.RemoveAll(button => { return true; });
                Destroy(itemsContainer.GetChild(i).gameObject);
            }
        }

        private void CompareEquipment(Button button)
        {
            ItemData item = button.GetComponent<ItemButton>().itemData;

            if (item.Type == ItemData.TYPE.ARMOR)
            {
                ArmorItemData armor = item as ArmorItemData;

                button.transform.Find("EquipedArrow").gameObject.SetActive(item && IsEquiped(armor));
                button.transform.Find("UpgradeArrow").gameObject.SetActive(item && IsBetterThanEquiped(armor));
            }
            else if (item.Type == ItemData.TYPE.WEAPON)
            {
                WeaponItemData weapon = item as WeaponItemData;

                button.transform.Find("EquipedArrow").gameObject.SetActive(item && IsEquiped(weapon));
                button.transform.Find("UpgradeArrow").gameObject.SetActive(item && IsBetterThanEquiped(weapon));
            }
        }

        private bool IsEquiped(WeaponItemData weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponItemData.WEAPON_TYPE.WEAPON:
                    return weapon == playerEquipment.RightWeapon;

                case WeaponItemData.WEAPON_TYPE.SHIELD:
                    return weapon == playerEquipment.LeftWeapon;
            }

            return false;
        }

        private bool IsEquiped(ArmorItemData armor)
        {
            switch (armor.slot)
            {
                case ArmorItemData.SLOT.CHEST:
                    return armor == playerEquipment.ChestEquipment;

                case ArmorItemData.SLOT.HEAD:
                    return armor == playerEquipment.HeadEquipment;

                case ArmorItemData.SLOT.LEGS:
                    return armor == playerEquipment.LegsEquipment;

                case ArmorItemData.SLOT.SHOOES:
                    return armor == playerEquipment.ShooesEquipment;
            }

            return false;
        }

        private bool IsBetterThanEquiped(WeaponItemData weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponItemData.WEAPON_TYPE.WEAPON:
                    return weapon.IsBetterThan(playerEquipment.RightWeapon);

                case WeaponItemData.WEAPON_TYPE.SHIELD:
                    return weapon.IsBetterThan(playerEquipment.LeftWeapon);
            }

            return false;
        }

        private bool IsBetterThanEquiped(ArmorItemData armor)
        {
            switch (armor.slot)
            {
                case ArmorItemData.SLOT.CHEST:
                    return armor.IsBetterThan(playerEquipment.ChestEquipment);

                case ArmorItemData.SLOT.HEAD:
                    return armor.IsBetterThan(playerEquipment.HeadEquipment);

                case ArmorItemData.SLOT.LEGS:
                    return armor.IsBetterThan(playerEquipment.LegsEquipment);

                case ArmorItemData.SLOT.SHOOES:
                    return armor.IsBetterThan(playerEquipment.ShooesEquipment);
            }

            return false;
        }

        private void PreviewItem(ItemData item)
        {
            if (item != currentItemPreview)
            {
                CleanPreviewItem();

                currentItemPreview = item;

                descriptionPanel.Find("NameText").GetComponent<Text>().text = item.name;


                itemDataPanel.Find("CharacteristicText").GetComponent<Text>().text = item.ToString();
                itemDataPanel.Find("ValueText").GetComponent<Text>().text = "Value : <b>" + item.value + "</b>";
                itemDataPanel.Find("WeightText").GetComponent<Text>().text = "Weight : <b>" + item.weight + "</b>";
                itemDataPanel.Find("LevelText").GetComponent<Text>().text =
                    "Level required : <b>" + item.levelRequired + "</b>";

                descriptionPanel.Find("DescriptionText").GetComponent<Text>().text = item.description;

                if (inventoryToShow.GetComponent<PlayerInventory>() != null)
                    PlayerInventoryFunction();
                else if (inventoryToShow.GetComponent<ShopInventory>() != null)
                    ShopInventoryFunction();
                else if (inventoryToShow.GetComponent<LootInventory>() != null)
                    LootInventoryFunction();
            }
        }

        private void LootInventoryFunction()
        {
            currType = TYPE_INVENTORY.Loot;

            LootInventory lootInventory = inventoryToShow as LootInventory;
            Button button = Instantiate(itemButtonPrefab);
            button.transform.SetParent(actionItemPanel);
            button.transform.Find("Text").GetComponent<Text>().text = "Recover";
            button.onClick.AddListener(() =>
            {
                if (lootInventory.AddItem(currentItemPreview))
                {
                    previewPanel.gameObject.SetActive(false);
                    RemoveButton(currentButtonPreview);
                    RefreshPlayerInfo();
                }
                else
                {
                    ChangeColor(button, Color.red);
                }
            });
        }

        private void RefreshInventory()
        {
            for (int i = 0; i < currentButtons.Count; ++i)
                CompareEquipment(currentButtons[i]);
        }

        public void ToggleBuySellFunction()
        {
            if (currType != TYPE_INVENTORY.Sell && currType != TYPE_INVENTORY.Shop)
                return;

            Inventory inventory = inventoryToShow;

            if (currType == TYPE_INVENTORY.Shop)
            {
                isSellingItem = true;
                oldInventory = inventoryToShow;
                inventory = GameManager.Instance.PlayerInstance.Inventory;
            }
            else if (currType == TYPE_INVENTORY.Sell)
            {
                isSellingItem = false;
                inventory = oldInventory;
            }
            HideInventory();
            ShowInventory(inventory);
        }

        private void SellInventoryFunction()
        {
            currType = TYPE_INVENTORY.Sell;

            PlayerInventory playerInventory = inventoryToShow as PlayerInventory;
            Button button = Instantiate(itemButtonPrefab);
            button.transform.SetParent(actionItemPanel);
            button.transform.Find("Text").GetComponent<Text>().text = "Sell";
            button.onClick.AddListener(() =>
            {
                player.Gold += currentItemPreview.value;
                playerInventory.RemoveItem(currentItemPreview);
                previewPanel.gameObject.SetActive(false);
                RemoveButton(currentButtonPreview);
                RefreshInventory();
                RefreshPlayerInfo();
            });
        }

        private void ShopInventoryFunction()
        {
            currType = TYPE_INVENTORY.Shop;

            ShopInventory shopInventory = inventoryToShow as ShopInventory;
            Button button = Instantiate(itemButtonPrefab);
            button.transform.SetParent(actionItemPanel);
            button.transform.Find("Text").GetComponent<Text>().text = "Buy";
            button.onClick.AddListener(() =>
            {
                if (shopInventory.BuyItem(currentItemPreview))
                {
                    previewPanel.gameObject.SetActive(false);
                    RemoveButton(currentButtonPreview);
                    RefreshInventory();
                    RefreshPlayerInfo();
                }
                else
                {
                    ChangeColor(button, Color.red);
                }
            });
        }

        private void CleanPreviewItem()
        {
            for (int i = 0; i < actionItemPanel.childCount; ++i)
                Destroy(actionItemPanel.GetChild(i).gameObject);
        }

        private void RemoveButton(Button button)
        {
            currentButtons.Remove(button);
            Destroy(button.gameObject);
        }

        private void PlayerInventoryFunction()
        {
            if (isSellingItem)
            {
                SellInventoryFunction();
                return;
            }

            currType = TYPE_INVENTORY.Player;

            PlayerInventory playerInventory = inventoryToShow as PlayerInventory;

            if (currentItemPreview.Type == ItemData.TYPE.ARMOR || currentItemPreview.Type == ItemData.TYPE.WEAPON)
            {
                Button button = Instantiate(itemButtonPrefab);
                button.transform.SetParent(actionItemPanel);
                button.transform.Find("Text").GetComponent<Text>().text = "Equip";
                button.onClick.AddListener(() =>
                {
                    if (playerInventory.EquipItem(currentItemPreview))
                    {
                        RefreshInventory();
                        RefreshPlayerInfo();
                    }
                    else
                    {
                        ChangeColor(button, Color.red);
                    }
                });
            }
            else if (currentItemPreview.Type == ItemData.TYPE.POTION ||
                     currentItemPreview.Type == ItemData.TYPE.PARCHMENT)
            {
                Button button = Instantiate(itemButtonPrefab);
                button.transform.SetParent(actionItemPanel);
                button.transform.Find("Text").GetComponent<Text>().text = "Use";
                button.onClick.AddListener(() =>
                {
                    playerInventory.UseItem(currentItemPreview);
                    RemoveButton(currentButtonPreview);
                    previewPanel.gameObject.SetActive(false);
                    RefreshPlayerInfo();
                });
            }

            if (!currentItemPreview.isQuestItem)
            {
                Button button1 = Instantiate(itemButtonPrefab);
                button1.transform.SetParent(actionItemPanel);
                button1.transform.Find("Text").GetComponent<Text>().text = "Drop";
                button1.onClick.AddListener(() =>
                {
                    playerInventory.DropItem(currentItemPreview);
                    RemoveButton(currentButtonPreview);
                    previewPanel.gameObject.SetActive(false);
                    RefreshPlayerInfo();
                });
            }
        }

        private void ChangeColor(Button button, Color color)
        {
            Color currentColor = button.colors.highlightedColor;
            ColorBlock block = button.colors;
            block.highlightedColor = color;
            button.colors = block;
        }
    }
}