using UnityEngine;

public class BuildingHealth : MonoBehaviour
{

    public AudioSource hit;
    
    [Header("Stats")]
    public float maxHealth = 50f;

    [Header("Health Bar")]
    public Vector3 healthBarOffset = new Vector3(0, 0.7f, 0);
    public float healthBarWidth = 1f;
    public float healthBarHeight = 0.15f;
    public float healthBarHideDelay = 2f;

    private float currentHealth;
    private GameObject healthBarGO;
    private UnityEngine.UI.Image healthBarFill;
    private Coroutine hideCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        CreateHealthBar();
        healthBarGO.SetActive(false);
    }

    private void CreateHealthBar()
    {
        Canvas worldCanvas = new GameObject("HealthBarCanvas").AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.transform.SetParent(transform, false);
        worldCanvas.transform.localPosition = healthBarOffset;
        worldCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarWidth, healthBarHeight);
        worldCanvas.sortingOrder = 10;

        healthBarGO = new GameObject("HealthBar");
        healthBarGO.transform.SetParent(worldCanvas.transform, false);
        UnityEngine.UI.Image bg = healthBarGO.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        healthBarGO.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarWidth, healthBarHeight);

        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(healthBarGO.transform, false);
        healthBarFill = fillGO.AddComponent<UnityEngine.UI.Image>();
        healthBarFill.color = Color.green;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float ratio = currentHealth / maxHealth;
        healthBarFill.GetComponent<RectTransform>().anchorMax = new Vector2(ratio, 1f);
        healthBarFill.color = Color.Lerp(Color.red, Color.green, ratio);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        healthBarGO.SetActive(true);
        UpdateHealthBar();
        
        if(hit) hit.Play(); //Audio when hit

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        if (currentHealth <= 0f)
        {
            AStarGrid.Instance?.SetNodeWalkable(transform.position, true);
            Destroy(gameObject);
        }
        else
            hideCoroutine = StartCoroutine(HideBarAfterDelay());
    }

    private System.Collections.IEnumerator HideBarAfterDelay()
    {
        yield return new WaitForSeconds(healthBarHideDelay);
        if (healthBarGO != null) healthBarGO.SetActive(false);
    }
    
}
