using UnityEngine;
using UnityEngine.SceneManagement;
namespace Audrey.Player.Script
{
    public class PlayerHealth : MonoBehaviour
    {
        private new string name;
        
        public float maxHealth;
        public float currentHealth;
        public GameObject GameManagerObj;

        public AudioSource hit;
        public AudioSource heal;
        
        void Start()
        {
            name = gameObject.name;
            GameManagerObj = GameObject.Find("GameManager");
            currentHealth = maxHealth;
            if (UIManager.Instance != null)
            {
                PlayerHealth temp = GameObject.Find(name).GetComponent<PlayerHealth>();
                if (temp != null)
                {
                    Debug.LogWarning(name);
                    UIManager.Instance.SetVariable(name, temp);
                    UpdateText(name);
                }
            }
            GameManager.OnDayStarted += Heal;
            
            
        }

        public virtual void ChangeHealth(float amount)
        {
            if (amount < 0)
            {
                hit.Play();
            }
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UpdateText(name);
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
            SceneManager.LoadSceneAsync(2);
            Debug.Log("DEAD");
        }

        void Heal()
        {
            heal.Play();
            currentHealth = maxHealth;
            UpdateText(name);
        }

        void UpdateText(string name)
        {
            UIManager.Instance.UpdateText(name);
        }
    }
}
