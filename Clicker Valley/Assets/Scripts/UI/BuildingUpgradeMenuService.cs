using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradeMenuService : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private BuildingUpgradeRow rowPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Конфиги построек")]
    [SerializeField]
    private List<BuildingConfig> allBuildingConfigs =
        new List<BuildingConfig>();

    private readonly Dictionary<string, BuildingUpgradeRow> activeRows =
        new Dictionary<string, BuildingUpgradeRow>();

    private void Start()
    {
        if (currencyManager == null)
        {
            Debug.LogWarning($"{name}: не назначен CurrencyManager.");
            return;
        }

        foreach (BuildingConfig config in allBuildingConfigs)
        {
            CreateRowIfNeeded(config);
        }
    }

    public void CreateRowIfNeeded(BuildingConfig config)
    {
        if (config == null || activeRows.ContainsKey(config.id))
        {
            return;
        }

        BuildingState state = new BuildingState
        {
            configId = config.id,
            isUnlocked = config.unlockCost <= 0,
            currentLevel = 1,
            config = config
        };

        BuildingUpgradeRow row = Instantiate(rowPrefab, content);

        // Назначаем CurrencyManager уже после создания копии
        row.SetCurrencyManager(currencyManager);
        row.SetData(config, state);

        activeRows.Add(config.id, row);
    }

    public void RefreshAllRows()
    {
        foreach (BuildingUpgradeRow row in activeRows.Values)
        {
            row.RefreshRow();
        }
    }
}