using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Wood";
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(itemName, amount);
            Destroy(gameObject);
        }
    }
}