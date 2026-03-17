using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;

    [Header("Level Animator Controllers")]
    public RuntimeAnimatorController[] levelAnimators;

    public GameObject impactEffectPrefab;

    private float damage;
    private int level;
    private Animator animator;

    // No Start() — everything happens in Init()
    public void Init(float towerDamage, int towerLevel, GameObject towerImpactPrefab)
    {
        damage = towerDamage;
        level = towerLevel;
        impactEffectPrefab = towerImpactPrefab;

        animator = GetComponent<Animator>();
        if (animator != null && levelAnimators != null && levelAnimators.Length > level)
            animator.runtimeAnimatorController = levelAnimators[level];

        Destroy(gameObject, lifetime); // now correctly called after init
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                ImpactEffect impactScript = impact.GetComponent<ImpactEffect>();
                if (impactScript != null) impactScript.Init(level);
            }

            Destroy(gameObject);
        }
    }
}
