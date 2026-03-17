using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    [Header("Level Animator Controllers")]
    public RuntimeAnimatorController[] levelAnimators;

    private Animator animator;

    public void Init(int level)
    {
        animator = GetComponent<Animator>();
        if (animator != null && levelAnimators != null && levelAnimators.Length > level)
            animator.runtimeAnimatorController = levelAnimators[level];

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, clipLength);
    }
}