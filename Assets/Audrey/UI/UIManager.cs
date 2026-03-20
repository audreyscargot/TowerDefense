using Audrey.Player.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public UIDocument uiDocument;
    private Label foodLabel;
    private Label woodLabel;
    private Label stoneLabel;
    private Label energyLabel;
    private Label healthLabel;
    private Label baseLabel;

    private InventoryManager inventoryManager;
    public PlayerHealth playerHealth;
    private PlayerFood playerFood;
    public PlayerHealth baseHealth;

    private Dictionary<string, IEnumerator> pulseCoroutines = new Dictionary<string, IEnumerator>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        foodLabel   = uiDocument.rootVisualElement.Q<Label>("FoodText");
        woodLabel   = uiDocument.rootVisualElement.Q<Label>("WoodText");
        stoneLabel  = uiDocument.rootVisualElement.Q<Label>("StoneText");
        healthLabel = uiDocument.rootVisualElement.Q<Label>("HealthText");
        energyLabel = uiDocument.rootVisualElement.Q<Label>("EnergyText");
        baseLabel   = uiDocument.rootVisualElement.Q<Label>("BaseText");
    }

    private void OnEnable()  => InventoryManager.OnItemChanged += TriggerPulse;
    private void OnDisable() => InventoryManager.OnItemChanged -= TriggerPulse;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        playerHealth     = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerFood       = GameObject.Find("Player").GetComponent<PlayerFood>();

        UpdateText("Food");
        UpdateText("Wood");
        UpdateText("Stone");
        UpdateText("Health");
        UpdateText("Energy");
    }


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
            case "Player":
                healthLabel.text = playerHealth.currentHealth.ToString();
                Debug.Log("Player: " + healthLabel.text);
                break;
            case "Base":
                baseLabel.text = baseHealth.currentHealth.ToString();
                break;
        }
    }

    public void SetVariable(string name, PlayerHealth health)
    {
        switch (name)
        {
            case "Player": playerHealth = health; break;
            case "Base":   baseHealth   = health; break;
        }
    }


    private void TriggerPulse(string resourceName)
    {
        Label target = resourceName switch
        {
            "Food"  => foodLabel,
            "Wood"  => woodLabel,
            "Stone" => stoneLabel,
            _       => null
        };

        if (target == null) return;

        if (pulseCoroutines.TryGetValue(resourceName, out IEnumerator existing) && existing != null)
            StopCoroutine(existing);

        IEnumerator coroutine = PulseCoroutine(target);
        pulseCoroutines[resourceName] = coroutine;
        StartCoroutine(coroutine);
    }

    private IEnumerator PulseCoroutine(Label label)
    {
        float duration   = 0.2f;
        float startScale = 1.3f;
        float t          = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, 1f, t / duration);
            label.transform.scale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        label.transform.scale = Vector3.one;
    }
}
