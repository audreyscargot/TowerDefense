using Audrey.Player.Script;
using UnityEngine;

public class RessourcesHealth : PlayerHealth
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            //spawn ressources
            Destroy(gameObject);
        }
    }
}
