using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UpgradeLevel
{
    public string levelName = "Level 1";
    public List<ResourceCost> costs = new List<ResourceCost>(); // cost to reach this level
}

public class Upgradeable : MonoBehaviour
{
    [Header("Settings")]
    public bool canBeUpgraded = true;
    public List<UpgradeLevel> levels; // slot 0 = base, slot 1 = upgrade 1, slot 2 = upgrade 2...

    public int CurrentLevel { get; private set; } = 0;
    public bool IsMaxLevel => CurrentLevel >= levels.Count - 1;

    // Any building script (Tower, Barrier, etc.) subscribes to this
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