using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public InputActionReference SwitchInput;
    public InputActionReference scrollInput;

    public GameObject CamObj;
    private CinemachineCamera cam;

    [Header("Zoom Settings")]
    public float scrollSensitivity = 0.05f;
    public float minZoom = 2f;
    public float zoomLerpSpeed = 8f;

    [Header("Minimap Settings")]
    public float markerSize = 18f;

    [Header("Minimap Marker Sprites")]
    public Sprite baseMarkerSprite;
    public Sprite spawnerMarkerSprite;
    public Sprite playerMarkerSprite;

    private float targetZoom;
    private float maxZoom;
    private bool minimapOpen = false;

    private GameObject minimapCanvas;
    private GameObject minimapPanel;
    private Camera mapCam;
    private RenderTexture mapRenderTexture;
    private RawImage mapRawImage;
    private RectTransform mapImageRect;
    private Dictionary<string, RectTransform> markers = new Dictionary<string, RectTransform>();

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

        if (MapGenerator.Instance != null)
            maxZoom = MapGenerator.Instance.mapSize / 2f;
        else
            maxZoom = 25f;

        targetZoom = cam.Lens.OrthographicSize;

        CreateMapCamera();
        CreateMinimapUI();

        float mapPx = Mathf.Min(1080 * 0.76f, 1920 * 0.76f); 
        mapImageRect.sizeDelta = new Vector2(mapPx, mapPx);

        minimapPanel.SetActive(false);
    }

    private void CreateMapCamera()
    {
        int size = Mathf.Min(Screen.width, Screen.height);
        mapRenderTexture = new RenderTexture(size, size, 16);

        GameObject camObj = new GameObject("MapOverviewCamera");
        mapCam = camObj.AddComponent<Camera>();
        mapCam.orthographic = true;
        mapCam.orthographicSize = MapGenerator.Instance != null
            ? MapGenerator.Instance.mapSize / 2f : 25f;
        mapCam.aspect = 1f;
        mapCam.transform.position = new Vector3(
            MapGenerator.Instance != null ? MapGenerator.Instance.MapCenterWorld.x : 0f,
            MapGenerator.Instance != null ? MapGenerator.Instance.MapCenterWorld.y : 0f,
            -10f
        );
        mapCam.targetTexture = mapRenderTexture;
        mapCam.cullingMask = ~0;
        mapCam.enabled = false;
    }

    private void CreateMinimapUI()
    {
        minimapCanvas = new GameObject("MinimapCanvas");
        Canvas canvas = minimapCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = minimapCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        minimapCanvas.AddComponent<GraphicRaycaster>();

        minimapPanel = new GameObject("MinimapPanel");
        minimapPanel.transform.SetParent(minimapCanvas.transform, false);
        Image bg = minimapPanel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 1f);
        RectTransform bgRect = minimapPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;

        // Title — top band, anchor to top of panel
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(minimapPanel.transform, false);
        Text title = titleObj.AddComponent<Text>();
        title.text = "MAP";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 60;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.88f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = titleRect.offsetMax = Vector2.zero;

        // Map image — centered in panel, size set in Start() via sizeDelta
        // No AspectRatioFitter — it was overriding anchor bounds and covering title/hint
        GameObject mapImageObj = new GameObject("MapImage");
        mapImageObj.transform.SetParent(minimapPanel.transform, false);
        mapRawImage = mapImageObj.AddComponent<RawImage>();
        mapRawImage.texture = mapRenderTexture;
        mapImageRect = mapImageObj.GetComponent<RectTransform>();
        mapImageRect.anchorMin = new Vector2(0.5f, 0.5f);
        mapImageRect.anchorMax = new Vector2(0.5f, 0.5f);
        mapImageRect.pivot = new Vector2(0.5f, 0.5f);
        mapImageRect.anchoredPosition = Vector2.zero;

        // Hint — bottom band, anchor to bottom of panel
        GameObject hintObj = new GameObject("Hint");
        hintObj.transform.SetParent(minimapPanel.transform, false);
        Text hint = hintObj.AddComponent<Text>();
        hint.text = "[press map key to close]";
        hint.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hint.fontSize = 40;
        hint.color = new Color(1f, 1f, 1f, 0.4f);
        hint.alignment = TextAnchor.MiddleCenter;
        RectTransform hintRect = hintObj.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0.12f);
        hintRect.offsetMin = hintRect.offsetMax = Vector2.zero;
    }

    private void RefreshMinimapMarkers()
    {
        if (MapGenerator.Instance == null) return;

        Vector2 mapCenter = MapGenerator.Instance.MapCenterWorld;
        float halfMap = MapGenerator.Instance.mapSize / 2f;

        GameObject baseObj = GameObject.Find("Base");
        if (baseObj != null)
            UpdateOrCreateMarker("Base", baseObj.transform.position, mapCenter, halfMap, baseMarkerSprite, markerSize * 3f);

        GameObject player = GameObject.Find("Player");
        if (player != null)
            UpdateOrCreateMarker("Player", player.transform.position, mapCenter, halfMap, playerMarkerSprite, markerSize * 2f);

        if (EnemySpawner.Instance != null)
        {
            List<Vector3> positions = EnemySpawner.Instance.GetSpawnPositions();
            for (int i = 0; i < positions.Count; i++)
                UpdateOrCreateMarker($"Spawner_{i}", positions[i], mapCenter, halfMap, spawnerMarkerSprite, markerSize * 2f);
        }
    }

    private void UpdateOrCreateMarker(string id, Vector3 worldPos, Vector2 mapCenter, float halfMap, Sprite sprite, float size)
    {
        float nx = Mathf.InverseLerp(-halfMap, halfMap, worldPos.x - mapCenter.x);
        float ny = Mathf.InverseLerp(-halfMap, halfMap, worldPos.y - mapCenter.y);

        Transform markerParent = minimapPanel.transform.Find("MapImage");

        if (markers.TryGetValue(id, out RectTransform rt) && rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(nx, ny);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            GameObject markerObj = new GameObject("Marker_" + id);
            markerObj.transform.SetParent(markerParent, false);

            Image img = markerObj.AddComponent<Image>();
            img.color = Color.white;

            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }

            rt = markerObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(size, size);
            rt.anchorMin = rt.anchorMax = new Vector2(nx, ny);
            rt.anchoredPosition = Vector2.zero;

            markers[id] = rt;
        }
    }

    private void ToggleMinimap()
    {
        minimapOpen = !minimapOpen;

        if (minimapOpen)
        {
            mapCam.enabled = true;
            mapCam.Render();
            mapCam.enabled = false;

            Time.timeScale = 0f;
            minimapPanel.SetActive(true);
            RefreshMinimapMarkers();
        }
        else
        {
            Time.timeScale = 1f;
            minimapPanel.SetActive(false);
            markers.Clear();
            Transform mapImage = minimapPanel.transform.Find("MapImage");
            if (mapImage != null)
                foreach (Transform child in mapImage)
                    Destroy(child.gameObject);
        }
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
        ToggleMinimap();
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        if (minimapOpen) return;
        float scroll = context.ReadValue<float>();
        targetZoom -= scroll * scrollSensitivity;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    void Update()
    {
        if (!minimapOpen && cam != null)
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(
                cam.Lens.OrthographicSize,
                targetZoom,
                Time.unscaledDeltaTime * zoomLerpSpeed
            );
        }

        if (!TimelineAnimating) return;
        TimelineTime += Time.unscaledDeltaTime;
        float t = TimelineTime / TimelineDuration;
        if (cam)
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(TimelineStart, TimelineTarget, t);
            if (t >= 1f)
            {
                cam.Lens.OrthographicSize = TimelineTarget;
                if (!minimapOpen) targetZoom = TimelineTarget;
                TimelineAnimating = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (mapRenderTexture != null) mapRenderTexture.Release();
    }
}
