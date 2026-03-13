using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    //Settings
    private float PlayerLightIntensity = 50;

    //References
    public GameObject Player;
    private PlayerLightHandler PlayerLightHandler;
    public GameObject InteractIconNormal;

    public GameObject InteractIconAnimated;

    //General Variables
    private float Days = 1; // counting days for score panel
    private float NightStep = 0; // after how much step the night gonna end
    private float DayStep = 0; // after how much step the day gonna end

    //function variables
    private float DayNightCounter = 0;
    private bool Day = true;

    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    public bool TimelineAnimating;
    bool TimelinePlayerLightPoint;
    public Volume ppv; // getting the volume

    private void Start()
    {
        PlayerLightHandler = Player.GetComponent<PlayerLightHandler>();
    }

    //DayNight Tween
    public void StartTween(float newStart, float newTarget, float newDuration)
    {
        TimelineStart = newStart;
        TimelineTarget = newTarget;
        TimelineDuration = newDuration;
        TimelineTime = 0;
        TimelineAnimating = true;
        TimelinePlayerLightPoint = true;
    }

    // player moved 1 time
    public void PlayerMoved()
    {
        if (!TimelineAnimating)
        {
            DayNightCounter += 1;
            if (Day && DayNightCounter >= DayStep)
            {
                Day = false;
                StartTween(0f, 1f, 5f);
                DayNightCounter = 0;
            }
            else if (!Day && DayNightCounter >= NightStep)
            {
                Days += 1;
                Day = true;
                StartTween(1f, 0f, 5f);
                DayNightCounter = 0;
                PlayerLightHandler.StartTweenPlayer(PlayerLightIntensity, 0f, 3f);
            }
        }
    }

    void Update()
    {
        // Day Night Timeline (count)
        if (!TimelineAnimating) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (ppv)
        {
            ppv.weight = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 0.6f && TimelinePlayerLightPoint && !Day)
            {
                TimelinePlayerLightPoint = false;
                // Activating/Deactivating player light
                PlayerLightHandler.StartTweenPlayer(0f, PlayerLightIntensity, 3f);
            }

            if (t >= 1f)
            {
                // stoping the timeline
                ppv.weight = TimelineTarget;
                TimelineAnimating = false;
            }
        }
    }
}