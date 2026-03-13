using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public InputActionReference ChangeCV;
    private bool Switched = false;

    public GameObject CamObj;
    private Camera cam;

    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    public bool TimelineAnimating;

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
        ChangeCV.action.performed += OnPressed;
        ChangeCV.action.Enable();
    }

    private void OnDisable()
    {
        ChangeCV.action.performed -= OnPressed;
        ChangeCV.action.Disable();
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (Switched) StartTween(5f, 10f, 1f);
        else StartTween(5f, 10f, 1f);
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
            }
        }
    }
}