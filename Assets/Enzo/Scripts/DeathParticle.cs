using System.Collections;
using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    public void Init(Vector2 velocity, float lifetime, Color color)
    {
        StartCoroutine(Run(velocity, lifetime, color));
    }

    private IEnumerator Run(Vector2 velocity, float lifetime, Color startColor)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            transform.position += (Vector3)(velocity * Time.deltaTime);
            velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * 4f); // drag

            float alpha = 1f - (elapsed / lifetime);
            if (sr != null) sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.1f, elapsed / lifetime);
            yield return null;
        }

        Destroy(gameObject);
    }
}