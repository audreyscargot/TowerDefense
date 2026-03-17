using UnityEngine;

public class FoodSpots : Interactable
{
    bool isHarvestable = false;
    public int daysToGrow;
    private int currentDay = -1;

    public Sprite[] plantDays;

    private SpriteRenderer currentSprite;
    
    public GameObject dropItemPrefab;
    public int dropCount;

    void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        GameManager.OnDayStarted += Grow;
    }
    void Grow()
    {
        if (currentDay < daysToGrow)
        {
            currentDay++;
            currentSprite.sprite = plantDays[currentDay-1];
            if (currentDay >= daysToGrow)
            {
                isHarvestable = true;
            }
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
