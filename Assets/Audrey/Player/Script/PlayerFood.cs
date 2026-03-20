using Audrey.Player.Script;
using UnityEngine;

public class PlayerFood : MonoBehaviour
{
    public static PlayerFood Instance{get; private set;}
    
    public bool canAction = true;
    
    [SerializeField] private int maxFood;
    public int currentFood;

    public AudioSource heal;

    public InventoryManager inventory;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        currentFood = maxFood;
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateText("Energy");
        }
        GameManager.OnDayStarted += ResetFood;
    }
    
    public void ChangeFood(int amount)
    {
        currentFood = Mathf.Clamp(currentFood + amount, 0, maxFood);
        canAction = currentFood > 0;
        UIManager.Instance.UpdateText("Food");
        UIManager.Instance.UpdateText("Energy");
    }

    //temp for one type of food
    public void ConsumeFood()
    {
        if (inventory.HasItem("Food",1))
        {
            if (currentFood < maxFood)
            {
                heal.Play();
                ChangeFood(1);
                inventory.RemoveItem("Food",1);
            }
        }
    }

    void ResetFood()
    {
        currentFood = maxFood;
    }
    
}
