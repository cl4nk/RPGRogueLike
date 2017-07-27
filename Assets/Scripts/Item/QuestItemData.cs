using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Item", menuName = "Items/QuestItem", order = 1)]
public class QuestItemData : ItemData
{
    private void OnEnable()
    {
        type = TYPE.QUEST_ITEM;
    }
}