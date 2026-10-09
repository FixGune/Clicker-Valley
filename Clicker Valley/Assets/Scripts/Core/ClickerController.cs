using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickerController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Настройки клика")]
    [Tooltip("Базовое количество золота за один клик.")]
    [SerializeField] private float baseClickGold = 1f;

    [Tooltip("Во сколько раз увеличивается сила клика за каждый уровень улучшения.")]
    [SerializeField] private float clickGrowthPerLevel = 1.15f;

    [Tooltip("Базовая цена первого улучшения клика.")]
    [SerializeField] private float baseClickUpgradeCost = 50f;

    [Tooltip("Во сколько раз увеличивается цена следующего улучшения клика.")]
    [SerializeField] private float clickUpgradeCostGrowth = 1.25f;

    [Tooltip("Максимальный уровень улучшения клика.")]
    [SerializeField] private int maxClickUpgradeLevel = 25;

    [Header("Текущее состояние")]
    [SerializeField] private int clickUpgradeLevel = 1;

    /// <summary>
    /// Вызывается после успешного клика.
    /// Передаёт количество полученного золота.
    /// </summary>
    public event Action<float> OnClickPerformed;

    /// <summary>
    /// Вызывается при изменении уровня улучшения клика.
    /// </summary>
    public event Action<int> OnClickUpgradeLevelChanged;

    public float BaseClickGold => baseClickGold;
    public int ClickUpgradeLevel => clickUpgradeLevel;
    public int MaxClickUpgradeLevel => maxClickUpgradeLevel;

    private void Start()
    {
        NotifyClickUpgradeLevelChanged();
    }

    /// <summary>
    /// Возвращает текущее количество золота за один клик.
    /// </summary>
    public float GetClickGold()
    {
        return baseClickGold *
            Mathf.Pow(clickGrowthPerLevel, clickUpgradeLevel - 1);
    }

    /// <summary>
    /// Возвращает цену следующего улучшения клика.
    /// </summary>
    public float GetClickUpgradeCost()
    {
        return baseClickUpgradeCost *
            Mathf.Pow(clickUpgradeCostGrowth, clickUpgradeLevel - 1);
    }

    /// <summary>
    /// Проверяет, можно ли улучшить клик.
    /// </summary>
    public bool CanUpgradeClick()
    {
        return clickUpgradeLevel < maxClickUpgradeLevel;
    }

    /// <summary>
    /// Вызывается при нажатии кнопки добычи золота.
    /// </summary>
    public void OnClickButtonPressed()
    {
        if (currencyManager == null)
        {
            Debug.LogWarning("ClickerController: не назначен CurrencyManager.");
            return;
        }

        float goldToAdd = GetClickGold();

        currencyManager.AddGold(goldToAdd);

        OnClickPerformed?.Invoke(goldToAdd);
    }

    /// <summary>
    /// Пытается улучшить клик.
    /// Возвращает true, если улучшение удалось.
    /// </summary>
    public bool TryUpgradeClick()
    {
        if (currencyManager == null)
        {
            Debug.LogWarning("ClickerController: не назначен CurrencyManager.");
            return false;
        }

        if (!CanUpgradeClick())
        {
            return false;
        }

        float upgradeCost = GetClickUpgradeCost();

        if (!currencyManager.TrySpendGold(upgradeCost))
        {
            return false;
        }

        clickUpgradeLevel++;

        NotifyClickUpgradeLevelChanged();
        return true;
    }

    private void NotifyClickUpgradeLevelChanged()
    {
        OnClickUpgradeLevelChanged?.Invoke(clickUpgradeLevel);
    }
}