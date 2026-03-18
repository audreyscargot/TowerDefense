using UnityEngine;
using UnityEngine.SceneManagement;
namespace Audrey.Player.Script
{
    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth;
        public float currentHealth;
        public GameObject GameManagerObj;
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
                Death();
            }
        }

        public virtual void Death()
        {
            //GameManager game over
            GameManager GM = GameManagerObj.GetComponent<GameManager>(); 
            
            SaveAndLoad SV = gameObject.GetComponent<SaveAndLoad>();
            SV.SetDays(GM.Days);
            SceneManager.LoadSceneAsync(1);
            Debug.Log("DEAD");
        }
    }
}
