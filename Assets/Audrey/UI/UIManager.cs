using Audrey.Player.Script;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance{get; private set;}
    
    public UIDocument uiDocument;
    private Label foodLabel;
    private Label woodLabel;
    private Label stoneLabel;
    private Label energyLabel;
    private Label healthLabel;
    
    private InventoryManager inventoryManager;
    private PlayerHealth playerHealth;
    private PlayerFood playerFood;

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
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerFood = GameObject.Find("Player").GetComponent<PlayerFood>();
        Debug.Log(playerHealth ? playerHealth : "Null");
        Debug.Log(playerFood ? playerFood : "Null");
        
        //Label init
        foodLabel = uiDocument.rootVisualElement.Q<Label>("FoodText");
        UpdateText("Food");
        woodLabel = uiDocument.rootVisualElement.Q<Label>("WoodText");
        UpdateText("Wood");
        stoneLabel = uiDocument.rootVisualElement.Q<Label>("StoneText");
        UpdateText("Stone");
        healthLabel = uiDocument.rootVisualElement.Q<Label>("HealthText");
        UpdateText("Health");
        energyLabel = uiDocument.rootVisualElement.Q<Label>("EnergyText");
        UpdateText("Energy");
    }

    
    //Resources resource label update
    public void UpdateText(string resourceName)
    {
        switch (resourceName)
        {
            case "Food":
                foodLabel.text = inventoryManager.findInInventory("Food");
                break;
            case "Wood":
                woodLabel.text = inventoryManager.findInInventory("Wood");
                break;
            case "Stone":
                stoneLabel.text = inventoryManager.findInInventory("Stone");
                break;
            case "Energy":
                energyLabel.text = playerFood.currentFood.ToString();
                break;
            case "Health":
                healthLabel.text = playerHealth.currentHealth.ToString();
                break;
        }
    }
}
