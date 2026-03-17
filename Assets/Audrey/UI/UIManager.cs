using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance{get; private set;}
    
    public UIDocument uiDocument;
    private Label foodLabel;
    private Label woodLabel;
    private Label stoneLabel;
    
    private InventoryManager inventoryManager;

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
        Debug.Log(Instance ? Instance : "Null");
        
        //Label init
        foodLabel = uiDocument.rootVisualElement.Q<Label>("FoodText");
        Debug.Log("the food Label is " + foodLabel);
        UpdateText("Food");
        woodLabel = uiDocument.rootVisualElement.Q<Label>("WoodText");
        Debug.Log("the wood Label is " + woodLabel);
        UpdateText("Wood");
        stoneLabel = uiDocument.rootVisualElement.Q<Label>("StoneText");
        UpdateText("Stone");
    }

    
    //Resources label update
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
            default:
                Debug.Log("default");
                break;
        }
    }
}
