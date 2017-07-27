using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    private static void Save(object o, string path)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + path);
        bf.Serialize(file, o);
        file.Close();
    }

    private static object Load(string path)
    {
        if (File.Exists(Application.persistentDataPath + path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + path, FileMode.Open);
            object o = bf.Deserialize(file);
            file.Close();
            return o;
        }
        return null;
    }

    public static void SaveGame(GameObject playerObj, int floor)
    {
        PlayerPrefs.SetInt("LastSavedReachedFloor", floor);
        SavePlayer(playerObj.GetComponent<Player>());
        SavePlayerInventory(playerObj.GetComponent<PlayerInventory>());
        SaveSpellsLauncher(playerObj.GetComponent<SpellsLauncher>());
        PlayerPrefs.Save();
    }

    private static void SavePlayer(Player p)
    {
        SavablePlayerData data = new SavablePlayerData(p);
        Save(data, "/player.gd");
    }

    private static void SaveSpellsLauncher(SpellsLauncher l)
    {
        SavableSpellShortcuts data = new SavableSpellShortcuts(l);
        Save(data, "/spells.gd");
    }

    public static void LoadSpellsLauncher(SpellsLauncher l)
    {
        if (l == null)
            return;
        SavableSpellShortcuts tmp = Load("/spells.gd") as SavableSpellShortcuts;
        if (tmp == null)
            return;

        tmp.FillSpellsLauncher(l);
    }

    public static void LoadPlayer(Player p)
    {
        if (p == null)
            return;
        SavablePlayerData tmp = Load("/player.gd") as SavablePlayerData;
        if (tmp == null)
            return;

        tmp.FillPlayer(p);
    }

    public static void LoadFloor()
    {
        PlayerPrefs.SetInt("LastReachedFloor", PlayerPrefs.GetInt("LastSavedReachedFloor", 1));
    }

    public static void RemoveCurrentFloor()
    {
        PlayerPrefs.DeleteKey("LastReachedFloor");
    }

    private static void SavePlayerInventory(PlayerInventory p)
    {
        SavablePlayerInventoryData tmp = new SavablePlayerInventoryData(p);
        Save(tmp, "/playerInventory.gd");
    }

    public static void LoadPlayerInventory(PlayerInventory p)
    {
        SavablePlayerInventoryData tmp = Load("/playerInventory.gd") as SavablePlayerInventoryData;
        if (tmp == null)
            return;
        tmp.FillPlayerInventory(p);
    }

    public static void RemoveSaves()
    {
        RemoveCurrentFloor();
        PlayerPrefs.DeleteKey("LastSavedReachedFloor");
        File.Delete(Application.persistentDataPath + "/player.gd");
        File.Delete(Application.persistentDataPath + "/playerInventory.gd");
    }

    [Serializable]
    private class SavablePlayerInventoryData
    {
        private int chestEquipmentIndex = -1;
        private int headEquipmentIndex = -1;
        private int leftWeaponIndex = -1;
        private int legsEquipmentIndex = -1;
        private readonly List<string> paths;
        private int rightWeaponIndex = -1;
        private int shooesEquipmentIndex = -1;

        public SavablePlayerInventoryData(PlayerInventory inventory)
        {
            paths = new List<string>();
            Equipment e = inventory.Equipment;
            int i = 0;
            foreach (ItemData data in inventory.Items)
            {
                paths.Add(data.path);
                SetEquipmentIndex(e, data, i++);
            }
        }

        public void FillPlayerInventory(PlayerInventory inventory)
        {
            inventory.Items.Clear();
            foreach (string path in paths)
                inventory.Items.Add(Resources.Load(path) as ItemData);
            Equipment e = inventory.Equipment;
            if (e == null)
                return;
            if (rightWeaponIndex >= 0)
                e.RightWeapon = inventory.Items[rightWeaponIndex] as WeaponItemData;
            if (leftWeaponIndex >= 0)
                e.LeftWeapon = inventory.Items[leftWeaponIndex] as WeaponItemData;
            if (headEquipmentIndex >= 0)
                e.HeadEquipment = inventory.Items[headEquipmentIndex] as ArmorItemData;
            if (chestEquipmentIndex >= 0)
                e.ChestEquipment = inventory.Items[chestEquipmentIndex] as ArmorItemData;
            if (legsEquipmentIndex >= 0)
                e.LegsEquipment = inventory.Items[legsEquipmentIndex] as ArmorItemData;
            if (shooesEquipmentIndex >= 0)
                e.ShooesEquipment = inventory.Items[shooesEquipmentIndex] as ArmorItemData;
        }

        private void SetEquipmentIndex(Equipment equipment, ItemData item, int index)
        {
            if (equipment.RightWeapon == item)
                rightWeaponIndex = index;
            else if (equipment.LeftWeapon == item)
                leftWeaponIndex = index;
            else if (equipment.HeadEquipment == item)
                headEquipmentIndex = index;
            else if (equipment.ChestEquipment == item)
                chestEquipmentIndex = index;
            else if (equipment.LegsEquipment == item)
                legsEquipmentIndex = index;
            else if (equipment.ShooesEquipment == item)
                shooesEquipmentIndex = index;
        }
    }

    [Serializable]
    private class SavableSpellShortcuts
    {
        private readonly string[] spellPaths;

        public SavableSpellShortcuts(SpellsLauncher launcher)
        {
            spellPaths = launcher.GetSpellPaths();
        }

        public void FillSpellsLauncher(SpellsLauncher launcher)
        {
            for (int i = 0; i < 9; i++)
                if (spellPaths[i] != "")
                    launcher.SetSpell(i, spellPaths[i]);
        }
    }

    [Serializable]
    private class SavablePlayerData
    {
        public readonly int characPointsOnLevelUp;
        public readonly int constitution;

        public readonly int curLife;
        public readonly int curMana;
        public readonly int dexterity;
        public readonly int experience;
        public readonly int experienceNextLevel;

        public readonly int gold;
        public readonly int intelligence;
        public readonly int level;
        public readonly int strength;

        public SavablePlayerData(Player p)
        {
            strength = p.Strength;
            dexterity = p.Dexterity;
            constitution = p.Constitution;
            intelligence = p.Intelligence;

            curLife = p.CurLife;
            curMana = p.CurMana;
            characPointsOnLevelUp = p.CharacPointsOnLevelUp;
            experience = p.Experience;
            experienceNextLevel = p.ExperienceNextLevel;
            level = p.Level;
            gold = p.Gold;
        }

        public void FillPlayer(Player p)
        {
            p.Strength = strength;
            p.Dexterity = dexterity;
            p.Constitution = constitution;
            p.Intelligence = intelligence;
            p.RecalculateCharacterStats();
            p.CurLife = curLife;
            p.CurMana = curMana;
            p.CharacPointsOnLevelUp = characPointsOnLevelUp;
            p.Experience = experience;
            p.ExperienceNextLevel = experienceNextLevel;
            p.Level = level;

            p.Gold = gold;
        }
    }
}