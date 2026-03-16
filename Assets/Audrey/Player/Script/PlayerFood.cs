using UnityEngine;

public class PlayerFood : MonoBehaviour
{
    private bool canAction = true;
    
    [SerializeField] private int maxFood;
    public int currentFood;
    
    void Start()
    {
        currentFood = maxFood;
    }
    
    public void changeFood(int amount)
    {
        currentFood = Mathf.Clamp(currentFood + amount, 0, maxFood);
        canAction = currentFood > 0;
    }
}
