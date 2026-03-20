using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class YSortManager : MonoBehaviour
{
    [Header("Settings")]
    public float sortingScale = 100f;
    public int baseOffset = 10000;
    public int alwaysOnTopOrder = 32000;

    [Header("Always On Top Tags")]
    public List<string> alwaysOnTopTags = new List<string> { "Player", "Enemy" };

    [Header("Excluded Tags (never touched by YSort)")]
    public List<string> excludedTags = new List<string> { "YSortExclude" };

    private List<SpriteRenderer> trackedRenderers = new List<SpriteRenderer>();
    private float refreshTimer = 0f;

    private void LateUpdate()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= 0.1f)
        {
            refreshTimer = 0f;
            trackedRenderers.Clear();
            foreach (SpriteRenderer sr in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
            {
                if (sr.GetComponent<TilemapRenderer>() != null) continue;
                if (sr.GetComponentInParent<Tilemap>() != null) continue;
                trackedRenderers.Add(sr);
            }
        }

        foreach (SpriteRenderer sr in trackedRenderers)
        {
            if (sr == null) continue;
            if (HasExcludedTag(sr.transform)) continue; 

            if (HasAlwaysOnTopTag(sr.transform))
                sr.sortingOrder = alwaysOnTopOrder;
            else
                sr.sortingOrder = baseOffset - Mathf.RoundToInt(sr.transform.position.y * sortingScale);
        }
    }

    private bool HasAlwaysOnTopTag(Transform t)
    {
        while (t != null)
        {
            if (alwaysOnTopTags.Contains(t.gameObject.tag)) return true;
            t = t.parent;
        }
        return false;
    }

    private bool HasExcludedTag(Transform t)
    {
        while (t != null)
        {
            if (excludedTags.Contains(t.gameObject.tag)) return true;
            t = t.parent;
        }
        return false;
    }
}
