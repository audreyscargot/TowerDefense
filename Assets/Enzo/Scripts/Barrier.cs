using UnityEngine;

public class BuildingSystemBarrier : MonoBehaviour, IRotatableBuilding
{
    [Header("Barrier Settings")]
    public float maxHealth = 50f;
    private float currentHealth;
    private Color originalColor; 

    [Header("Rotation Sprites")]
    public Sprite horizontalSprite;
    public Sprite verticalSprite;

    [Header("Collider Sizes")]
    public Vector2 horizontalColliderSize = new Vector2(1f, 0.5f);
    public Vector2 verticalColliderSize = new Vector2(0.5f, 1f);

    private SpriteRenderer sr;
    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth; 
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color; 
    }

    public void ApplyRotationSprite(float rotationAngle)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (box == null) box = GetComponent<BoxCollider2D>();

        float angle = Mathf.Abs(rotationAngle % 180f);
        bool isVertical = Mathf.Abs(angle - 90f) < 1f;

        sr.sprite = isVertical ? verticalSprite : horizontalSprite;

        if (box != null)
            box.size = isVertical ? verticalColliderSize : horizontalColliderSize;

        transform.rotation = Quaternion.identity;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        sr.color = Color.red;
        Invoke(nameof(ResetColor), 0.1f);

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    private void ResetColor()
    {
        if (sr != null) sr.color = originalColor;
    }
}