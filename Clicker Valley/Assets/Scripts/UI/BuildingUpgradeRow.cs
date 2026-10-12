using UnityEngine;
using UnityEngine.UI;

public class BuildingUpgradeRow : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private CurrencyManager currencyManager;

    [Header("UI элементы")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text costText;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Image BackgroundImage;

    [Header("Фото модели")]
    [SerializeField] private Image modelImage;

    private BuildingConfig buildingConfig;
    private BuildingState buildingState;


    private void Awake()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }
    }

    public void SetData(BuildingConfig config, BuildingState state)
    {
        buildingConfig = config;
        buildingState = state;

        if (buildingConfig == null || buildingState == null)
        {
            Debug.LogWarning($"{name}: не заданы BuildingConfig или BuildingState.");
            return;
        }

        buildingState.config = buildingConfig;

        RefreshRow();
        UpdateModelImage();
    }

    public void RefreshRow()
    {
        if (buildingConfig == null || buildingState == null)
        {
            return;
        }

        if (nameText != null)
        {
            nameText.text = buildingConfig.displayName;
        }

        if (levelText != null)
        {
            levelText.text = $"Уровень {buildingState.currentLevel}/{buildingConfig.maxLevel}";
        }

        bool canUpgrade = buildingState.currentLevel < buildingConfig.maxLevel;

        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade;
        }

        if (costText != null)
        {
            if (canUpgrade)
            {
                float cost = buildingConfig.GetUpgradeCost(buildingState.currentLevel);
                costText.text = FormatNumber(cost);

                bool hasEnoughGold =
                    currencyManager != null &&
                    currencyManager.CanSpend(cost);

                if (upgradeButton != null)
                {
                    upgradeButton.interactable = hasEnoughGold;
                }
            }
            else
            {
                costText.text = "Максимум";
            }
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (buildingConfig == null ||
            buildingState == null ||
            currencyManager == null)
        {
            return;
        }

        if (buildingState.currentLevel >= buildingConfig.maxLevel)
        {
            return;
        }

        float upgradeCost =
            buildingConfig.GetUpgradeCost(buildingState.currentLevel);

        if (!currencyManager.TrySpendGold(upgradeCost))
        {
            return;
        }

        buildingState.currentLevel++;

        RefreshRow();
        UpdateModelImage();
    }

    private void UpdateModelImage()
    {
        if (modelImage == null || buildingConfig == null)
        {
            return;
        }

        Sprite stageSprite = GetUiSprite();

        if (stageSprite != null)
        {
            modelImage.sprite = stageSprite;
        }
    }

    private Sprite GetUiSprite()
    {
        if (buildingConfig.uiSprites == null ||
            buildingConfig.uiSprites.Length == 0)
        {
            return null;
        }

        int stageIndex = Mathf.Clamp(
            buildingState.currentLevel / 5,
            0,
            buildingConfig.uiSprites.Length - 1
        );

        return buildingConfig.uiSprites[stageIndex];
    }

    private static string FormatNumber(float value)
    {
        if (value >= 1_000_000_000f)
        {
            return (value / 1_000_000_000f).ToString("0.##") + "B";
        }

        if (value >= 1_000_000f)
        {
            return (value / 1_000_000f).ToString("0.##") + "M";
        }

        if (value >= 1_000f)
        {
            return (value / 1_000f).ToString("0.##") + "K";
        }

        return Mathf.FloorToInt(value).ToString();
    }

    /// <summary>
    /// Назначает CurrencyManager после создания строки сервисом.
    /// Нужно, потому что префаб не может хранить ссылку на объект сцены.
    /// </summary>
    public void SetCurrencyManager(CurrencyManager manager)
    {
        currencyManager = manager;
    }
}