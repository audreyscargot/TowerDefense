using UnityEngine;
using UnityEngine.AI;

public class BuildingSystemBarrier : MonoBehaviour
{
    [Header("Barrier Settings")]
    public float maxHealth = 50f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}