using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Wood";
    public int amount = 1;

    // Use OnTriggerEnter2D to detect when the player walks over it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding is the Player
        if (collision.CompareTag("Player"))
        {
            // Here you will eventually call your Inventory Manager
            // e.g., InventoryManager.Instance.AddItem(itemName, amount);
            
            Debug.Log("Picked up " + amount + " " + itemName);
            Destroy(gameObject);
        }
    }
}