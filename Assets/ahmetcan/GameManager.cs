using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    //General Variables
    private float Days = 1; // counting days for score panel
    private float NightStep = 10; // after how much step the night gonna end
    private float DayStep = 10; // after how much step the day gonna end
    
    //function variables
    private float DayNightCounter = 0;
    private bool Day = true;
    
    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    bool TimelineAnimating;
    
    public Volume ppv;  // getting the volume

    //DayNight Tween
    public void StartTween(float newStart,float newTarget, float newDuration)
    {
        TimelineStart = newStart;
        TimelineTarget = newTarget;
        TimelineDuration = newDuration;
        TimelineTime = 0;
        TimelineAnimating = true;
    }

    // player moved 1 time
    void PlayerMoved()
    {
        if (!TimelineAnimating) {
        DayNightCounter += 1;
        if (Day &&  DayNightCounter >= DayStep) {
            Day = false;
            StartTween(0f,1f,5f);
        } else if (!Day &&  DayNightCounter >= NightStep) {
            Days += 1;
            Day = true;
            StartTween(1f,0f,5f);
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
            if (t >= 1f)
            {
                ppv.weight = TimelineTarget;
                TimelineAnimating = false;
            }
        }
    }
}
