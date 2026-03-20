using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UpgradeLevel
{
    public string levelName = "Level 1";
    public List<ResourceCost> costs = new List<ResourceCost>();
}

public class Upgradeable : MonoBehaviour
{
    [Header("Settings")]
    public bool canBeUpgraded = true;
    public List<UpgradeLevel> levels;

    public int CurrentLevel { get; private set; } = 0;
    public bool IsMaxLevel => CurrentLevel >= levels.Count - 1;
    public string CurrentLevelName => (levels != null && levels.Count > CurrentLevel) ? levels[CurrentLevel].levelName : "?";
    public string NextLevelName => (!IsMaxLevel && levels != null) ? levels[CurrentLevel + 1].levelName : "";

    public UnityEvent<int> OnLevelChanged;

    public List<ResourceCost> GetNextUpgradeCost()
    {
        if (!canBeUpgraded || IsMaxLevel) return null;
        return levels[CurrentLevel + 1].costs;
    }

    public void Upgrade()
    {
        if (!canBeUpgraded || IsMaxLevel) return;
        CurrentLevel++;
        OnLevelChanged?.Invoke(CurrentLevel);
    }
}