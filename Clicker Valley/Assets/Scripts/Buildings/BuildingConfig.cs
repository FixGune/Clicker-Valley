using UnityEngine;

[CreateAssetMenu(
    fileName = "NewBuilding",
    menuName = "Incremental Game/Building Config"
)]
public class BuildingConfig : ScriptableObject
{
    [Header("Основное")]
    public string id;
    public string displayName;
    public string zoneId;

    [Header("Экономика")]
    public int unlockCost;
    public float baseGoldPerMinute;
    public float goldGrowthPerLevel;

    [Header("Улучшение")]
    public float baseUpgradeCost;
    public float costGrowthPerLevel;
    public int maxLevel = 25;

    [Header("Визуал")]
    public GameObject[] visualStages;

    public float GetIncome(int level)
    {
        return baseGoldPerMinute *
            Mathf.Pow(goldGrowthPerLevel, level - 1);
    }

    public float GetUpgradeCost(int level)
    {
        return baseUpgradeCost *
            Mathf.Pow(costGrowthPerLevel, level - 1);
    }
}