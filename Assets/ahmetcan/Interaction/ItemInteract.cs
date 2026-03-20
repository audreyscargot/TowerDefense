using System;
using UnityEngine;

public class ItemInteract : Interactable

{
    public string itemName = "Wood";
    public int amount = 1;

    public AudioClip grab;
    public override void Action()
    {
        InventoryManager.Instance.AddItem(itemName, amount);
        AudioSource.PlayClipAtPoint(grab, transform.position, 1.5f);
        Destroy(gameObject);
    }
}