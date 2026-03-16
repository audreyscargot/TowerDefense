using UnityEngine;

public class FoodSpots : MonoBehaviour
{
    bool isOccupied = false;
    private int daysToGrow;
    private int currentDay;
    //
    
    void Plant(int  days)
    {
        if (!isOccupied)
        {
            isOccupied = true;
            daysToGrow = days;
            if (currentDay >= daysToGrow)
            {
                createResources();
            }
        }
    }
    
    void createResources()
    {
        isOccupied = false;
    }
}
