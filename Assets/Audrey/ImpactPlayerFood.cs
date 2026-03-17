
using UnityEngine;

public class ImpactPlayerFood : MonoBehaviour
{
    [SerializeField] private int foodChange; 
    public PlayerFood playerFood;

    public void ImpactFood()
    {
        playerFood.ChangeFood(foodChange);
    }
}

