using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combat Settings")]
    public float detectionRadius = 5f;
    public float fireRate = 1.5f;
    public float projectileSpeed = 8f;
    public float damage = 10f;

    [Header("Level Sprites - Base")]
    public Sprite[] levelSprites; // level 1, 2, 3

    [Header("References")]
    public GameObject projectilePrefab;   // single prefab
    public GameObject impactEffectPrefab; // single prefab
    public Transform firePoint;
    public Transform weaponPivot;

    [Header("Weapon Level Animators")]
    public RuntimeAnimatorController[] weaponLevelAnimators; // swap on upgrade
    
    [Header("Layer")]
    public LayerMask enemyLayer;

    private SpriteRenderer baseSpriteRenderer;
    private int currentLevel = 0;
    private float fireCooldown = 0f;

    void Start()
    {
        baseSpriteRenderer = GetComponent<SpriteRenderer>();
        fireCooldown = 1f / fireRate;
        ApplyLevelVisuals();
    }


    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Collider2D enemyInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);

        if (enemyInRange != null)
        {
            if (weaponPivot != null)
            {
                Vector2 dir = (enemyInRange.transform.position - weaponPivot.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            if (fireCooldown <= 0f)
            {
                Shoot(enemyInRange.transform);
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            fireCooldown = Mathf.Max(fireCooldown, 0f);
        }
    }
    
    private void Shoot(Transform target)
    {
        if (projectilePrefab == null) return;

        Vector2 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector2 dir = (target.position - (Vector3)spawnPos).normalized;
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * projectileSpeed;

        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
            projectileScript.Init(damage, currentLevel, impactEffectPrefab);
    }
    
    public void Upgrade()
    {
        if (currentLevel < 2) // max level 3 (index 2)
        {
            currentLevel++;
            damage += 5f;
            detectionRadius += 0.5f;
            ApplyLevelVisuals();
        }
    }

    private void ApplyLevelVisuals()
    {
        if (baseSpriteRenderer != null && levelSprites != null && levelSprites.Length > currentLevel)
            baseSpriteRenderer.sprite = levelSprites[currentLevel];

        Animator weaponAnimator = weaponPivot != null ? weaponPivot.GetComponent<Animator>() : null;
        if (weaponAnimator != null && weaponLevelAnimators != null && weaponLevelAnimators.Length > currentLevel)
            weaponAnimator.runtimeAnimatorController = weaponLevelAnimators[currentLevel];
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
