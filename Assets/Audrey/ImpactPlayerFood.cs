
using UnityEngine;

public class ImpactPlayerFood : MonoBehaviour
{
    [SerializeField] private int foodChange; //positive if makes player gain food, negative if makes player lose food
    public PlayerFood playerFood;

    public void ImpactFood()
    {
        playerFood.changeFood(foodChange);
    }
}

