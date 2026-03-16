using System;
using UnityEngine;

public class ItemInteract : Interactable
{
    public string itemName = "Wood";
    public int amount = 1;
    public override void Action()
    {
        InventoryManager.Instance.AddItem(itemName, amount);
        Destroy(gameObject);
    }
}