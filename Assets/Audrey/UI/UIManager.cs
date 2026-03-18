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
            return;
        }

        Instance = this;

        foodLabel   = uiDocument.rootVisualElement.Q<Label>("FoodText");
        woodLabel   = uiDocument.rootVisualElement.Q<Label>("WoodText");
        stoneLabel  = uiDocument.rootVisualElement.Q<Label>("StoneText");
        healthLabel = uiDocument.rootVisualElement.Q<Label>("HealthText");
        energyLabel = uiDocument.rootVisualElement.Q<Label>("EnergyText");
    }

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerFood = GameObject.Find("Player").GetComponent<PlayerFood>();

        UpdateText("Food");
        UpdateText("Wood");
        UpdateText("Stone");
        UpdateText("Health");
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
                if (energyLabel != null && playerFood != null)
                    energyLabel.text = playerFood.currentFood.ToString();
                break;
            case "Health":
                healthLabel.text = playerHealth.currentHealth.ToString();
                break;
        }
    }
}
