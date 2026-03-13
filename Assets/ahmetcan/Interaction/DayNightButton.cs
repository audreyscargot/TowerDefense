using System;
using UnityEngine;

public class DayNightButton : Interactable
{
    public override void Action()
    {
        GameManager.PlayerMoved();
    }

    private bool spriteWasVisible = false;

    private void Update()
    {
        if (spawnedObject)
        {
            var sr = spawnedObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (GameManager.TimelineAnimating && spriteWasVisible)
                {
                    sr.enabled = false;
                    spriteWasVisible = false;
                    Debug.Log("maked invisible");
                }
                else
                {
                    if (GameManager.TimelineAnimating == false && !spriteWasVisible)
                    {
                        sr.enabled = true;
                        spriteWasVisible = true;
                        Debug.Log("Visible");
                    }
                }
            }
        }
    }
}