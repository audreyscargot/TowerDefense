using UnityEngine;

public class PlayerGrassTrail : MonoBehaviour
{
    [Header("Grass Settings")]
    public int   poolSize      = 25;
    public float spawnInterval = 0.07f;
    public float minSpeed      = 0.1f;
    public float lifetime      = 0.45f;
    public float startSize     = 0.35f;
    public float endSize       = 0f;
    public Color startColor    = new Color(0.3f, 0.75f, 0.2f, 0.95f);
    public Color endColor      = new Color(0.2f, 0.55f, 0.1f, 0f);
    public Vector2 spawnOffset = new Vector2(0f, -0.3f);

    [Header("Spread & Motion")]
    public float spreadRadius  = 0.18f;
    public float launchSpeed   = 1.2f;   // initial upward kick
    public float gravity       = 2.5f;   // pulls blades back down
    public float rotateSpeed   = 200f;   // degrees per second spin

    [Header("Rendering")]
    public string sortingLayerName = "Default";
    public int    sortingOrder     = 5;

    private Rigidbody2D rb;
    private GameObject[]     pool;
    private SpriteRenderer[] poolSR;
    private float[]          poolBirthTime;
    private float[]          poolLifetime;
    private Vector3[]        poolVelocity;   // per-particle velocity (with gravity)
    private float[]          poolRotSpeed;   // per-particle spin direction
    private int   poolIndex  = 0;
    private float spawnTimer = 0f;
    private Sprite bladeSprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
        if (playerSR != null)
        {
            sortingLayerName = playerSR.sortingLayerName;
            sortingOrder     = playerSR.sortingOrder - 1;
        }

        bladeSprite = CreateBladeSprite();
        BuildPool();
    }

    private void BuildPool()
    {
        pool          = new GameObject[poolSize];
        poolSR        = new SpriteRenderer[poolSize];
        poolBirthTime = new float[poolSize];
        poolLifetime  = new float[poolSize];
        poolVelocity  = new Vector3[poolSize];
        poolRotSpeed  = new float[poolSize];

        GameObject parent = new GameObject("GrassPool");

        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = new GameObject("Blade_" + i);
            pool[i].transform.SetParent(parent.transform, false);

            poolSR[i] = pool[i].AddComponent<SpriteRenderer>();
            poolSR[i].sprite           = bladeSprite;
            poolSR[i].sortingLayerName = sortingLayerName;
            poolSR[i].sortingOrder     = sortingOrder;

            pool[i].SetActive(false);
            poolBirthTime[i] = -999f;
        }
    }

    private void Update()
    {
        float now = Time.time;
        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].activeSelf) continue;

            float age  = now - poolBirthTime[i];
            float life = poolLifetime[i];

            if (age >= life) { pool[i].SetActive(false); continue; }

            float t = age / life;

            // Fade + shrink
            poolSR[i].color = Color.Lerp(startColor, endColor, t);
            float size = Mathf.Lerp(startSize, endSize, t);
            pool[i].transform.localScale = new Vector3(size * 0.35f, size, 1f); // keep blade ratio

            // Physics: apply gravity to velocity then move
            poolVelocity[i].y -= gravity * Time.deltaTime;
            pool[i].transform.position += poolVelocity[i] * Time.deltaTime;

            // Spin
            pool[i].transform.Rotate(0f, 0f, poolRotSpeed[i] * Time.deltaTime);
        }

        if (rb == null || rb.linearVelocity.magnitude < minSpeed) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;
        spawnTimer = spawnInterval;
        EmitBlade();
    }

    private void EmitBlade()
    {
        int i     = poolIndex;
        poolIndex = (poolIndex + 1) % poolSize;

        Vector2 scatter = Random.insideUnitCircle * spreadRadius;
        pool[i].transform.position   = (Vector2)transform.position + spawnOffset + scatter;
        pool[i].transform.rotation   = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        pool[i].transform.localScale = new Vector3(startSize * 0.35f, startSize, 1f);

        poolSR[i].color  = startColor;
        poolBirthTime[i] = Time.time;
        poolLifetime[i]  = lifetime + Random.Range(-0.08f, 0.08f);

        // Random sideways kick + upward launch
        float angle = Random.Range(-50f, 50f) * Mathf.Deg2Rad;
        poolVelocity[i] = new Vector3(
            Mathf.Sin(angle) * launchSpeed,
            launchSpeed * Random.Range(0.6f, 1.2f),
            0f
        );

        // Random spin direction
        poolRotSpeed[i] = Random.Range(-rotateSpeed, rotateSpeed);

        pool[i].SetActive(true);
    }

    // Thin vertical blade — like a cut grass snippet
    private Sprite CreateBladeSprite()
    {
        int w = 8, h = 24;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] px = new Color[w * h];

        for (int i = 0; i < px.Length; i++)
        {
            int x = i % w;
            int y = i / w;

            // Horizontal soft edge
            float xEdge  = Mathf.Clamp01(1f - Mathf.Abs((x - w / 2f) / (w / 2f)));
            // Taper toward tip
            float yTaper = Mathf.Clamp01((float)y / h);
            float alpha  = xEdge * (0.4f + 0.6f * yTaper);
            alpha = alpha * alpha;

            px[i] = new Color(1f, 1f, 1f, alpha);
        }

        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0f), h);
    }
}
