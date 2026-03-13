using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("Building Settings")]
    public GameObject barrierPrefab;
    public string requiredResource = "Wood";
    public int barrierCost = 5;

    [Header("Placement Settings")]
    public LayerMask unbuildableLayer; 
    public float maxBuildDistance = 5f;

    [Header("Preview")]
    public GameObject previewIndicator; 
    private SpriteRenderer previewRenderer;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        
        if (previewIndicator != null)
        {
            previewRenderer = previewIndicator.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        HandlePreview();

        if (Input.GetMouseButtonDown(1))
        {
            TryBuildBarrier();
        }
    }

    private void HandlePreview()
    {
        if (previewIndicator == null) return;

        Vector2 mousePos = GetMouseGridPosition();
        previewIndicator.transform.position = mousePos;

        if (CanBuildHere(mousePos) && HasEnoughResources())
        {
            previewRenderer.color = new Color(0, 1, 0, 0.5f); 
        }
        else
        {
            previewRenderer.color = new Color(1, 0, 0, 0.5f); 
        }
    }

    private void TryBuildBarrier()
    {
        Vector2 buildPos = GetMouseGridPosition();

        if (!HasEnoughResources())
        {
            Debug.Log("Not enough resources! Need " + barrierCost + " " + requiredResource);
            return;
        }

        if (!CanBuildHere(buildPos))
        {
            Debug.Log("Cannot build here!");
            return;
        }

        InventoryManager.Instance.RemoveItem(requiredResource, barrierCost);
        Instantiate(barrierPrefab, buildPos, Quaternion.identity);
    }

    private Vector2 GetMouseGridPosition()
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        float snappedX = Mathf.Round(mouseWorldPos.x);
        float snappedY = Mathf.Round(mouseWorldPos.y);
        
        return new Vector2(snappedX, snappedY);
    }

    private bool HasEnoughResources()
    {
        return InventoryManager.Instance.HasItem(requiredResource, barrierCost);
    }

    private bool CanBuildHere(Vector2 position)
    {
        if (Vector2.Distance(transform.position, position) > maxBuildDistance)
            return false;

        Collider2D hit = Physics2D.OverlapCircle(position, 0.2f, unbuildableLayer);
        
        return hit == null; // Returns true if nothing is blocking it
    }
}
