using UnityEngine;

public class PlayerFood : MonoBehaviour
{
    private bool canAction = true;
    
    [SerializeField] private int maxFood;
    public int currentFood;

    public InventoryManager inventory;
    
    void Start()
    {
        currentFood = maxFood;
    }
    
    public void ChangeFood(int amount)
    {
        currentFood = Mathf.Clamp(currentFood + amount, 0, maxFood);
        canAction = currentFood > 0;
        Debug.Log("current food: " + currentFood);
        UIManager.Instance.UpdateText("Food");
    }

    //temp for one type of food
    public void ConsumeFood()
    {
        if (inventory.HasItem("Food",1))
        {
            if (currentFood < maxFood)
            {
                ChangeFood(1);
                inventory.RemoveItem("Food",1);
            }
        }
    }
    
}
