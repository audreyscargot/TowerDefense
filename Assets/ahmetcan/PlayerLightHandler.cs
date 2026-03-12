using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using Component = System.ComponentModel.Component;

public class PlayerLightHandler : MonoBehaviour
{
    //Settings
    private float LightIntensity = 50;
    //References
    private Light2D Spotlight;
    
    //timeline
    private static float TimelineStart = 0f;
    private static float TimelineTarget = 50f;
    private static float TimelineDuration = 2f;
    private static float TimelineTime = 0f;
    private static bool TimelineAnimatin = false;

    private void Start()
    {
        Spotlight = GetComponent<Light2D>();
    }
    //Tween
    public void StartTween(float newStart,float newTarget, float newDuration)
    {
        TimelineStart = newStart;
        TimelineTarget = newTarget;
        TimelineDuration = newDuration;
        TimelineTime = 0;
        TimelineAnimatin = true;
    }
    // Activate/Deactivate Light
   public void ActivateLight() {StartTween(0f,LightIntensity,2f);}
   public void DeactivateLight() {StartTween(LightIntensity,0,2f);}
    
    // Event Tick
    void Update()
    {
        HandleTimelineAnimation();
    }

    private void HandleTimelineAnimation()
    {
        // Day Night Timeline (count)
        if (!TimelineAnimatin) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (Spotlight != null)
        {
            Spotlight.intensity = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 1f)
            {
                Spotlight.intensity = TimelineTarget;
                TimelineAnimatin = false;
            }
        }
    }
}