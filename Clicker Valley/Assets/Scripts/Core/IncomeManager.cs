using System;
using System.Collections.Generic;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Настройки дохода")]
    [Tooltip("Сколько раз в секунду начисляется доход.")]
    [SerializeField] private int ticksPerSecond = 20;

    private readonly List<BuildingState> buildings = new List<BuildingState>();

    private float tickInterval;
    private float accumulatedIncome;

    /// <summary>
    /// Общий доход всех построек в минуту.
    /// </summary>
    public float TotalIncomePerMinute { get; private set; }

    /// <summary>
    /// Вызывается, когда общий доход изменился.
    /// UI может подписаться на это событие.
    /// </summary>
    public event Action<float> OnTotalIncomeChanged;

    private void Awake()
    {
        tickInterval = 1f / ticksPerSecond;
    }

    private void Start()
    {
        RecalculateTotalIncome();
        InvokeRepeating(nameof(AddIncomeTick), tickInterval, tickInterval);
    }

    /// <summary>
    /// Добавляет постройку в систему дохода.
    /// </summary>
    public void RegisterBuilding(BuildingState buildingState)
    {
        if (buildingState == null)
        {
            return;
        }

        if (!buildings.Contains(buildingState))
        {
            buildings.Add(buildingState);
        }

        RecalculateTotalIncome();
    }

    /// <summary>
    /// Удаляет постройку из системы дохода.
    /// </summary>
    public void UnregisterBuilding(BuildingState buildingState)
    {
        if (buildingState == null)
        {
            return;
        }

        if (buildings.Remove(buildingState))
        {
            RecalculateTotalIncome();
        }
    }

    /// <summary>
    /// Пересчитывает общий доход в минуту.
    /// Вызывай после покупки или улучшения постройки.
    /// </summary>
    public void RecalculateTotalIncome()
    {
        float total = 0f;

        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingState building = buildings[i];

            if (building == null || !building.isUnlocked)
            {
                continue;
            }

            total += building.GetIncomePerMinute();
        }

        TotalIncomePerMinute = total;

        OnTotalIncomeChanged?.Invoke(TotalIncomePerMinute);
    }

    /// <summary>
    /// Начисляет долю дохода за один тик.
    /// Вызывается 20 раз в секунду.
    /// </summary>
    private void AddIncomeTick()
    {
        if (currencyManager == null || TotalIncomePerMinute <= 0f)
        {
            return;
        }

        float incomePerSecond = TotalIncomePerMinute / 60f;
        float incomeForTick = incomePerSecond * tickInterval;

        accumulatedIncome += incomeForTick;

        // Начисляем золото только когда накопился хотя бы 1 золотой.
        if (accumulatedIncome >= 1f)
        {
            float goldToAdd = Mathf.Floor(accumulatedIncome);

            currencyManager.AddGold(goldToAdd);

            accumulatedIncome -= goldToAdd;
        }
    }
}