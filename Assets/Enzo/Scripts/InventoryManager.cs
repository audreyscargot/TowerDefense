using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // Fired after any change — UIManager subscribes to trigger pulse
    public static Action<string> OnItemChanged;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void AddItem(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName)) inventory[itemName] += amount;
        else inventory.Add(itemName, amount);

        Debug.Log("Inventory Updated " + itemName + ": " + inventory[itemName]);
        UIManager.Instance.UpdateText(itemName);
        OnItemChanged?.Invoke(itemName);
    }

    public bool HasItem(string itemName, int amountRequired)
    {
        if (inventory.ContainsKey(itemName)) return inventory[itemName] >= amountRequired;
        return false;
    }

    public void RemoveItem(string itemName, int amountToRemove)
    {
        if (HasItem(itemName, amountToRemove))
        {
            inventory[itemName] -= amountToRemove;
            Debug.Log("Spent " + amountToRemove + " " + itemName + ". Remaining: " + inventory[itemName]);
            UIManager.Instance.UpdateText(itemName);
            OnItemChanged?.Invoke(itemName);
        }
    }

    public string findInInventory(string itemName)
    {
        if (inventory.ContainsKey(itemName)) return inventory[itemName].ToString();
        return "0";
    }
}