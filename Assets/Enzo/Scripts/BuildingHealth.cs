using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            DestroyBuilding();
    }

    private void DestroyBuilding()
    {
        NavMesh.bake();
        Destroy(gameObject);
    }
}