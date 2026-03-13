using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddItem(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
        }
        else
        {
            inventory.Add(itemName, amount);
        }
        
        Debug.Log($"Inventory Updated: {itemName} = {inventory[itemName]}");
    }

    public bool HasItem(string itemName, int amountRequired)
    {
        if (inventory.ContainsKey(itemName))
        {
            return inventory[itemName] >= amountRequired;
        }
        return false;
    }

    public void RemoveItem(string itemName, int amountToRemove)
    {
        if (HasItem(itemName, amountToRemove))
        {
            inventory[itemName] -= amountToRemove;
            Debug.Log($"Spent {amountToRemove} {itemName}. Remaining: {inventory[itemName]}");
        }
    }
}