using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public InputActionReference SwitchInput;
    public InputActionReference scrollInput;
    private bool Switched = false;

    public GameObject CamObj;
    private CinemachineCamera cam;

    [Header("Zoom Settings")]
    public float scrollSensitivity = 1f;
    public float minZoom = 2f;
    public float zoomLerpSpeed = 8f;

    private float targetZoom;
    private float maxZoom;

    //timeline
    float TimelineStart;
    float TimelineTarget;
    float TimelineDuration;
    float TimelineTime;
    public bool TimelineAnimating;

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

        // Max zoom = half the map size so you never see outside the map
        if (MapGenerator.Instance != null)
            maxZoom = MapGenerator.Instance.mapSize / 2f;
        else
            maxZoom = 25f;

        targetZoom = cam.Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        SwitchInput.action.performed += OnPressed;
        SwitchInput.action.Enable();

        if (scrollInput != null)
        {
            scrollInput.action.Enable();
            scrollInput.action.performed += OnScroll;
        }
    }

    private void OnDisable()
    {
        SwitchInput.action.performed -= OnPressed;
        SwitchInput.action.Disable();

        if (scrollInput != null)
        {
            scrollInput.action.performed -= OnScroll;
            scrollInput.action.Disable();
        }
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (TimelineAnimating) return;
        if (Switched) StartTween(cam.Lens.OrthographicSize, minZoom, 1f);
        else StartTween(cam.Lens.OrthographicSize, 5f, 1f);
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        if (TimelineAnimating) return;

        float scroll = context.ReadValue<float>();
        targetZoom -= scroll * scrollSensitivity;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }


    void Update()
    {
        // Smooth scroll zoom
        if (!TimelineAnimating && cam != null)
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(
                cam.Lens.OrthographicSize,
                targetZoom,
                Time.deltaTime * zoomLerpSpeed
            );
        }

        // Timeline tween (button zoom)
        if (!TimelineAnimating) return;
        TimelineTime += Time.deltaTime;
        float t = TimelineTime / TimelineDuration;
        if (cam)
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 1f)
            {
                cam.Lens.OrthographicSize = TimelineTarget;
                targetZoom = TimelineTarget; // sync scroll target to tween result
                TimelineAnimating = false;
                Switched = !Switched;
            }
        }
    }
}
