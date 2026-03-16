using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public InputActionReference SwitchInput;
    private bool Switched = false;

    public GameObject CamObj;
    private Camera cam;

    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    public bool TimelineAnimating;

    public float NormalZoom = 5f;
    public float BigZoom = 10f;
    //Tween
    public void StartTween(float newStart, float newTarget, float newDuration)
    {
        TimelineStart = newStart;
        TimelineTarget = newTarget;
        TimelineDuration = newDuration;
        TimelineTime = 0;
        TimelineAnimating = true;
    }

    private void Start()
    {
        cam = CamObj.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        SwitchInput.action.performed += OnPressed;
        SwitchInput.action.Enable();
    }

    private void OnDisable()
    {
        SwitchInput.action.performed -= OnPressed;
        SwitchInput.action.Disable();
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (TimelineAnimating) return;
        if (Switched) StartTween(BigZoom, NormalZoom, 1f);
        else StartTween(NormalZoom, BigZoom, 1f);
    }

    void Update()
    {
        //Timeline (count)
        if (!TimelineAnimating) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (cam)
        {
            cam.orthographicSize = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 1f)
            {
                // stoping the timeline
                cam.orthographicSize = TimelineTarget;
                TimelineAnimating = false;
                if (Switched) Switched = false;
                else Switched = true;
            }
        }
    }
}