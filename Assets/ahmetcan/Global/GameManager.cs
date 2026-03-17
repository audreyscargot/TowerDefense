using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    // Events
    public static Action OnNightStarted;
    public static Action OnDayStarted;

    //Settings
    public float PlayerLightIntensity = 50;

    //References
    public GameObject Player;
    private PlayerLightHandler PlayerLightHandler;
  

    //General Variables
    public float Days = 1;
    private float DayStep = 0; 

    //function variables
    private float DayNightCounter = 0;
    public bool isDay { get; private set; } = true; // Spawner needs to know this

    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    public bool TimelineAnimating;
    bool TimelinePlayerLightPoint;
    public Volume ppv;

    private void Start()
    {
        PlayerLightHandler = Player.GetComponent<PlayerLightHandler>();
        
        StartCoroutine(StartDayOneDelayed());
    }

    private System.Collections.IEnumerator StartDayOneDelayed()
    {
        yield return null; 
        OnDayStarted?.Invoke();
    }


    public void StartTween(float newStart, float newTarget, float newDuration)
    {
        TimelineStart = newStart;
        TimelineTarget = newTarget;
        TimelineDuration = newDuration;
        TimelineTime = 0;
        TimelineAnimating = true;
        TimelinePlayerLightPoint = true;
    }

    // Called by DayNightButton to start the night
    public void PlayerMoved()
    {
        if (!TimelineAnimating && isDay)
        {
            DayNightCounter += 1;
            if (DayNightCounter >= DayStep)
            {
                isDay = false;
                StartTween(0f, 1f, 5f);
                DayNightCounter = 0;
                
                OnNightStarted?.Invoke(); // Tell spawner to start
                Debug.Log("Day " + DayNightCounter);
            }
        }
    }

    //Called by the Spawner when all enemies die
    public void EndNight()
    {
        if (!isDay && !TimelineAnimating)
        {
            Days += 1;
            isDay = true;
            StartTween(1f, 0f, 5f);
            PlayerLightHandler.StartTweenPlayer(PlayerLightIntensity, 0f, 3f);
            
            OnDayStarted?.Invoke(); // Tell spawners to show themselves for the new day
        }
    }

    void Update()
    {
        if (!TimelineAnimating) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (ppv)
        {
            ppv.weight = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 0.6f && TimelinePlayerLightPoint && !isDay)
            {
                TimelinePlayerLightPoint = false;
                PlayerLightHandler.StartTweenPlayer(0f, PlayerLightIntensity, 3f);
            }

            if (t >= 1f)
            {
                ppv.weight = TimelineTarget;
                TimelineAnimating = false;
            }
        }
    }
}
