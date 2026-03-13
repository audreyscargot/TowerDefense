using UnityEngine;
namespace Audrey.Player.Script
{
    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth;
        public float currentHealth;
    
        void Start()
        {
            currentHealth = maxHealth;
        }

        public virtual void ChangeHealth(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            Debug.Log(currentHealth);
            if (currentHealth <= 0)
            {
                //GameManager game over
                Debug.Log("DEAD");
            }
        }
    }
}
