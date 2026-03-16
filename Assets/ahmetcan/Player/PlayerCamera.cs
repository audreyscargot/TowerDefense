using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public InputActionReference SwitchInput;
    private bool Switched = false;

    public GameObject CamObj;
    private CinemachineCamera cam;

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
        cam = CamObj.GetComponent<CinemachineCamera>();
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
        if (Switched) StartTween(2f, 2f, 1f);
        else StartTween(2f, 5f, 1f);
    }

    void Update()
    {
        //Timeline (count)
        if (!TimelineAnimating) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (cam)
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 1f)
            {
                // stoping the timeline
                cam.Lens.OrthographicSize = TimelineTarget;
                TimelineAnimating = false;
                if (Switched) Switched = false;
                else Switched = true;
            }
        }
    }
}