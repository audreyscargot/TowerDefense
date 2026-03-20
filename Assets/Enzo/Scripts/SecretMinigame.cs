using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SecretMinigame : MonoBehaviour
{
    // ── Config ───────────────────────────────────────────────
    private const string SECRET_WORD    = "GardenTower";
    private const float  GAME_DURATION  = 20f;
    private const int    REWARD_WOOD    = 15;
    private const int    REWARD_STONE   = 15;
    private const float  SPAWN_INTERVAL = 0.8f;
    private const float  ENEMY_SPEED    = 60f;
    private const int    MAX_LIVES      = 5;

    // ── State ────────────────────────────────────────────────
    private string       typedBuffer = "";
    private bool         gameRunning = false;

    // ── UI refs ──────────────────────────────────────────────
    private GameObject      rootCanvas;
    private RectTransform   playArea;
    private TextMeshProUGUI timerLabel;
    private TextMeshProUGUI livesLabel;
    private TextMeshProUGUI scoreLabel;
    private TextMeshProUGUI titleLabel;
    private int             score;
    private int             lives;
    private float           timeLeft;

    private List<GameObject> enemies = new List<GameObject>();

    // ── Input ────────────────────────────────────────────────
    private void OnEnable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c)
    {
        if (gameRunning) return;

        char expected = SECRET_WORD.ToLower()[typedBuffer.Length];

        if (char.ToLower(c) == expected)
        {
            typedBuffer += char.ToLower(c);
            if (typedBuffer == SECRET_WORD.ToLower())
            {
                typedBuffer = "";
                StartCoroutine(LaunchGame());
            }
        }
        else
        {
            typedBuffer = char.ToLower(c) == SECRET_WORD.ToLower()[0]
                ? char.ToLower(c).ToString()
                : "";
        }
    }

    // ── Update ───────────────────────────────────────────────
    private void Update()
    {
        if (!gameRunning) return;

        timeLeft -= Time.unscaledDeltaTime;
        timerLabel.text = $"{Mathf.CeilToInt(timeLeft)}s";

        if (timeLeft <= 0f) { EndGame(true); return; }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] == null) { enemies.RemoveAt(i); continue; }
                RectTransform rt = enemies[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos))
                {
                    Destroy(enemies[i]);
                    enemies.RemoveAt(i);
                    score++;
                    scoreLabel.text = $"Score: {score}";
                    break;
                }
            }
        }

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null) { enemies.RemoveAt(i); continue; }

            RectTransform rt = enemies[i].GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.MoveTowards(
                rt.anchoredPosition,
                Vector2.zero,
                ENEMY_SPEED * Time.unscaledDeltaTime
            );

            if (rt.anchoredPosition.magnitude < 18f)
            {
                Destroy(enemies[i]);
                enemies.RemoveAt(i);
                lives--;
                livesLabel.text = BuildLives();
                if (lives <= 0) { EndGame(false); return; }
            }
        }
    }

    // ── Game lifecycle ────────────────────────────────────────
    private IEnumerator LaunchGame()
    {
        Time.timeScale = 0f;
        BuildUI();
        yield return StartCoroutine(ShowCountdown());

        gameRunning     = true;
        score           = 0;
        lives           = MAX_LIVES;
        timeLeft        = GAME_DURATION;
        scoreLabel.text = "Score: 0";
        livesLabel.text = BuildLives();
        titleLabel.gameObject.SetActive(false);

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator ShowCountdown()
    {
        titleLabel.gameObject.SetActive(true);
        for (int i = 3; i >= 1; i--)
        {
            titleLabel.text     = i.ToString();
            titleLabel.fontSize = 90;
            yield return new WaitForSecondsRealtime(1f);
        }
        titleLabel.text = "GO!";
        yield return new WaitForSecondsRealtime(0.6f);
    }

    private IEnumerator SpawnLoop()
    {
        while (gameRunning)
        {
            SpawnEnemy();
            yield return new WaitForSecondsRealtime(SPAWN_INTERVAL);
        }
    }

    private void EndGame(bool won)
    {
        gameRunning = false;
        StopAllCoroutines();

        foreach (var e in enemies) if (e != null) Destroy(e);
        enemies.Clear();

        titleLabel.gameObject.SetActive(true);
        titleLabel.fontSize = 52;

        if (won)
        {
            InventoryManager.Instance?.AddItem("Wood",  REWARD_WOOD);
            InventoryManager.Instance?.AddItem("Stone", REWARD_STONE);
            titleLabel.text  = $"YOU WIN!\n+{REWARD_WOOD} Wood  +{REWARD_STONE} Stone\nScore: {score}";
            titleLabel.color = new Color(0.3f, 1f, 0.4f);
        }
        else
        {
            titleLabel.text  = $"OVERRUN!\nScore: {score}";
            titleLabel.color = new Color(1f, 0.3f, 0.3f);
        }

        StartCoroutine(CloseAfterDelay(3f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        if (rootCanvas != null) Destroy(rootCanvas);
    }

    // ── Enemy spawning ────────────────────────────────────────
    private static readonly Vector2[] EDGES =
    {
        new Vector2( 0f,   1f),
        new Vector2( 0f,  -1f),
        new Vector2( 1f,   0f),
        new Vector2(-1f,   0f),
        new Vector2( 0.7f,  0.7f),
        new Vector2(-0.7f,  0.7f),
        new Vector2( 0.7f, -0.7f),
        new Vector2(-0.7f, -0.7f),
    };

    private void SpawnEnemy()
    {
        Vector2 dir      = EDGES[Random.Range(0, EDGES.Length)];
        Vector2 spawnPos = dir * 210f + new Vector2(
            Random.Range(-30f, 30f),
            Random.Range(-30f, 30f)
        );

        GameObject e = new GameObject("MinigameEnemy");
        e.transform.SetParent(playArea, false);

        Image img = e.AddComponent<Image>();
        img.color = new Color(1f, 0.25f, 0.2f);

        RectTransform rt    = e.GetComponent<RectTransform>();
        rt.sizeDelta         = new Vector2(32, 32);
        rt.anchoredPosition  = spawnPos;

        GameObject lbl = new GameObject("Lbl");
        lbl.transform.SetParent(e.transform, false);
        TextMeshProUGUI t = lbl.AddComponent<TextMeshProUGUI>();
        t.text      = "X";
        t.fontSize  = 20;
        t.color     = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        t.fontStyle = FontStyles.Bold;
        RectTransform lr = lbl.GetComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = lr.offsetMax = Vector2.zero;

        enemies.Add(e);
    }

    // ── UI construction ───────────────────────────────────────
    private void BuildUI()
    {
        rootCanvas = new GameObject("MinigameCanvas");
        Canvas cv  = rootCanvas.AddComponent<Canvas>();
        cv.renderMode   = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 200;
        CanvasScaler cs = rootCanvas.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        rootCanvas.AddComponent<GraphicRaycaster>();

        GameObject backdrop = new GameObject("Backdrop");
        backdrop.transform.SetParent(rootCanvas.transform, false);
        Image bdImg = backdrop.AddComponent<Image>();
        bdImg.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform bdRt = backdrop.GetComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.offsetMin = bdRt.offsetMax = Vector2.zero;

        GameObject pa = new GameObject("PlayArea");
        pa.transform.SetParent(rootCanvas.transform, false);
        Image paImg = pa.AddComponent<Image>();
        paImg.color = new Color(0.08f, 0.12f, 0.08f, 1f);
        RectTransform paRt    = pa.GetComponent<RectTransform>();
        paRt.anchorMin         = new Vector2(0.5f, 0.5f);
        paRt.anchorMax         = new Vector2(0.5f, 0.5f);
        paRt.pivot             = new Vector2(0.5f, 0.5f);
        paRt.anchoredPosition  = Vector2.zero;
        paRt.sizeDelta         = new Vector2(500, 500);
        playArea = paRt;

        GameObject center = new GameObject("Center");
        center.transform.SetParent(pa.transform, false);
        Image cImg = center.AddComponent<Image>();
        cImg.color = new Color(0.2f, 0.8f, 0.3f, 0.6f);
        RectTransform cRt    = center.GetComponent<RectTransform>();
        cRt.anchorMin         = cRt.anchorMax = new Vector2(0.5f, 0.5f);
        cRt.sizeDelta         = new Vector2(36, 36);
        cRt.anchoredPosition  = Vector2.zero;

        timerLabel = MakeLabel(rootCanvas.transform, "20s", 28,
            new Vector2(0.5f, 0.5f), new Vector2(0f,    290f), new Vector2(200, 40));
        livesLabel = MakeLabel(rootCanvas.transform, BuildLives(), 26,
            new Vector2(0.5f, 0.5f), new Vector2(-160f, 290f), new Vector2(200, 40));
        scoreLabel = MakeLabel(rootCanvas.transform, "Score: 0", 28,
            new Vector2(0.5f, 0.5f), new Vector2(160f,  290f), new Vector2(160, 40));

        titleLabel           = MakeLabel(rootCanvas.transform, "3", 90,
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 200));
        titleLabel.alignment = TextAlignmentOptions.Center;
        titleLabel.color     = Color.white;
    }

    private TextMeshProUGUI MakeLabel(Transform parent, string text, float size,
        Vector2 anchor, Vector2 pos, Vector2 sizeDelta)
    {
        GameObject obj      = new GameObject("Label");
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt   = obj.GetComponent<RectTransform>();
        rt.anchorMin        = rt.anchorMax = anchor;
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta        = sizeDelta;
        return tmp;
    }

    private string BuildLives()
    {
        return $"Lives: {lives}/{MAX_LIVES}";
    }
}
