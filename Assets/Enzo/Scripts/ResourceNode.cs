using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    public float maxHealth = 3;
    public GameObject dropItemPrefab;
    public int minDropAmount = 1;
    public int maxDropAmount = 3;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color hitFlashColor = Color.red;
    private Color originalColor;

    [HideInInspector] public int prefabIndex = 0;
    [HideInInspector] public Vector3 spawnPosition;

    private float currentHealth;

    public AudioSource hit;

    private void Start()
    {
        currentHealth = maxHealth;
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damageAmount)
    {
        if (!PlayerFood.Instance.canAction) return; //if not enough Food, player can't hit
        
        currentHealth -= damageAmount;
        hit.Play();

        if (spriteRenderer != null)
            StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
            Harvest();
    }

    private void Harvest()
    {
        if (dropItemPrefab != null)
        {
            int dropCount = Random.Range(minDropAmount, maxDropAmount + 1);
            for (int i = 0; i < dropCount; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(dropItemPrefab, transform.position + randomOffset, Quaternion.identity);
            }
        }

        PlayerFood.Instance.ChangeFood(-1);
        DestroyNode();
    }

    public void DestroyNode()
    {
        if (MapGenerator.Instance != null)
            MapGenerator.Instance.OnResourceDestroyed(this);
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }
}