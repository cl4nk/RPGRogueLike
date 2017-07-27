using System;
using UnityEngine;

[Serializable]
public abstract class ItemData : ScriptableObject
{
    public enum TYPE
    {
        WEAPON,
        ARMOR,
        POTION,
        PARCHMENT,
        QUEST_ITEM
    }

    public string description = "Item Description";
    public bool isQuestItem = true;
    public int levelRequired;

    public new string name = "Item name";
    public string path = "Items/";
    public GameObject prefab;

    protected TYPE type;
    public int value;
    public int weight;

    public TYPE Type
    {
        get { return type; }
    }
}