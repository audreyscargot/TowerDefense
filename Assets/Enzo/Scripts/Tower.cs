using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerLevelData
{
    public float detectionRadius = 5f;
    public float fireRate = 1.5f;
    public float projectileSpeed = 8f;
    public float damage = 10f;
    public Sprite baseSprite;
    public RuntimeAnimatorController weaponAnimator;
}

public class Tower : MonoBehaviour
{
    [Header("Level Data")]
    public List<TowerLevelData> levels; // must match Upgradeable.levels count

    [Header("References")]
    public GameObject projectilePrefab;
    public GameObject impactEffectPrefab;
    public Transform firePoint;
    public Transform weaponPivot;

    [Header("Layer")]
    public LayerMask enemyLayer;

    private SpriteRenderer baseSpriteRenderer;
    private Animator weaponAnimator;
    private float fireCooldown = 0f;

    private float detectionRadius;
    private float fireRate;
    private float projectileSpeed;
    private float damage;
    private int currentLevel = 0;

    void Start()
    {
        baseSpriteRenderer = GetComponent<SpriteRenderer>();
        weaponAnimator = weaponPivot != null ? weaponPivot.GetComponent<Animator>() : null;

        // Subscribe to upgrade events
        Upgradeable upgradeable = GetComponent<Upgradeable>();
        if (upgradeable != null) upgradeable.OnLevelChanged.AddListener(OnUpgraded);

        ApplyLevelData(0);
        fireCooldown = 1f / fireRate;
        if (weaponAnimator != null) weaponAnimator.speed = 0f;
    }

    private void OnUpgraded(int newLevel)
    {
        currentLevel = newLevel;
        ApplyLevelData(newLevel);
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Collider2D enemyInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);

        if (enemyInRange != null)
        {
            if (weaponAnimator != null) weaponAnimator.speed = 1f;

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
            if (weaponAnimator != null) weaponAnimator.speed = 0f;
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

    private void ApplyLevelData(int level)
    {
        if (levels == null || levels.Count <= level) return;

        TowerLevelData data = levels[level];
        detectionRadius = data.detectionRadius;
        fireRate = data.fireRate;
        projectileSpeed = data.projectileSpeed;
        damage = data.damage;

        if (baseSpriteRenderer != null && data.baseSprite != null)
            baseSpriteRenderer.sprite = data.baseSprite;

        if (weaponAnimator != null && data.weaponAnimator != null)
        {
            weaponAnimator.runtimeAnimatorController = data.weaponAnimator;
            weaponAnimator.speed = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float radius = (levels != null && levels.Count > 0) ? levels[0].detectionRadius : 5f;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
