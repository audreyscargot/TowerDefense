using UnityEngine;

public class FoodSpots : Interactable
{
    bool isHarvestable = true;
    private int daysToGrow;
    private int currentDay = 0;

    public Sprite[] plantDays;

    public SpriteRenderer currentSprite;
    
    public GameObject dropItemPrefab;
    public int dropCount;
    
    void Grow()
    {
        currentDay++;
        currentSprite.sprite = plantDays[currentDay];
        if (currentDay >= daysToGrow)
        {
            isHarvestable = true;
        }
    }

    public override void Action()
    {
        if (isHarvestable)
        {
            if (dropItemPrefab != null)
            {
                for (int i = 0; i < dropCount; i++)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f,0.5f), 0);
                    Instantiate(dropItemPrefab, transform.position + randomOffset, Quaternion.identity);
                }
            }
            Destroy(gameObject);
        }
    }
}
