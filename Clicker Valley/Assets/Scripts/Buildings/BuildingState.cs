using UnityEngine;

[System.Serializable]
public class BuildingState
{
    public string configId;
    public bool isUnlocked;
    public int currentLevel = 1;

    [HideInInspector]
    public BuildingConfig config;

    public float GetIncomePerMinute()
    {
        if (config == null || !isUnlocked)
        {
            return 0f;
        }

        return config.GetIncome(currentLevel);
    }
}