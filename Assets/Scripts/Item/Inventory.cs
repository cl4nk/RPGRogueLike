using System.Collections.Generic;
using UnityEngine;

public abstract class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> items;

    protected Player player;

    public List<ItemData> Items
    {
        get { return items; }
        protected set { items = value; }
    }

    protected virtual void Start()
    {
        player = GameManager.Instance.PlayerInstance.Player;

        for (int i = 0; i < items.Count; ++i)
            items[i] = Instantiate(items[i]);
    }

    protected bool CanTake(ItemData item)
    {
        if (player.CurWeight + item.weight <= player.MaxWeight)
            return true;

        return false;
    }
}