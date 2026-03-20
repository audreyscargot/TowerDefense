using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NightSplashUI : MonoBehaviour
{
    [Header("Settings")]
    public float displayDuration = 2f;
    public float fontSize = 90f;
    public Color splashColor = new Color(1f, 0.25f, 0.25f, 1f);

    private GameObject splashCanvas;
    private TextMeshProUGUI splashText;

    private void OnEnable()  => GameManager.OnNightStarted += HandleNightStarted;
    private void OnDisable() => GameManager.OnNightStarted -= HandleNightStarted;

    private void Start()
    {
        splashCanvas = new GameObject("NightSplashCanvas");
        Canvas c = splashCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 100;
        splashCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GameObject textObj = new GameObject("SplashText");
        textObj.transform.SetParent(splashCanvas.transform, false);

        splashText = textObj.AddComponent<TextMeshProUGUI>();
        splashText.alignment = TextAlignmentOptions.Center;
        splashText.fontSize = fontSize;
        splashText.fontStyle = FontStyles.Bold;
        splashText.color = splashColor;

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.7f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        splashCanvas.SetActive(false);
    }

    private void HandleNightStarted()
    {
        int day = Mathf.RoundToInt(GameManager.Instance.Days);
        splashText.text = $"NIGHT {day}";
        StopAllCoroutines();
        StartCoroutine(ShowSplash());
    }

    private IEnumerator ShowSplash()
    {
        splashCanvas.SetActive(true);

        float fadeIn = 0.3f;
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            splashText.color = new Color(splashColor.r, splashColor.g, splashColor.b, t / fadeIn);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        float fadeOut = 0.5f;
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            splashText.color = new Color(splashColor.r, splashColor.g, splashColor.b, 1f - (t / fadeOut));
            yield return null;
        }

        splashCanvas.SetActive(false);
    }
}
