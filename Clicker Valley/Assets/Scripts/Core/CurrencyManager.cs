using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float startingGold = 0f;

    [Header("Текущее состояние")]
    [SerializeField] private float currentGold;

    /// <summary>
    /// Вызывается при любом изменении количества золота.
    /// Передаёт новое значение золота.
    /// </summary>
    public event Action<float> OnGoldChanged;

    public float CurrentGold => currentGold;

    private void Awake()
    {
        currentGold = startingGold;
    }

    private void Start()
    {
        NotifyGoldChanged();
    }

    /// <summary>
    /// Добавляет золото игроку.
    /// Используется для клика, пассивного дохода и офлайн-дохода.
    /// </summary>
    public void AddGold(float amount)
    {
        if (amount <= 0f)
        {
            Debug.LogWarning("Нельзя добавить отрицательное или нулевое количество золота.");
            return;
        }

        currentGold += amount;

        NotifyGoldChanged();
    }

    /// <summary>
    /// Проверяет, хватает ли золота для покупки или улучшения.
    /// </summary>
    public bool CanSpend(float amount)
    {
        return currentGold >= amount && amount > 0f;
    }

    /// <summary>
    /// Пытается списать золото.
    /// Возвращает true, если списание прошло успешно.
    /// </summary>
    public bool TrySpendGold(float amount)
    {
        if (!CanSpend(amount))
        {
            return false;
        }

        currentGold -= amount;

        NotifyGoldChanged();
        return true;
    }

    /// <summary>
    /// Полностью сбрасывает золото.
    /// Пригодится для отладки или будущего престижа.
    /// </summary>
    public void ResetGold()
    {
        currentGold = startingGold;

        NotifyGoldChanged();
    }

    private void NotifyGoldChanged()
    {
        OnGoldChanged?.Invoke(currentGold);
    }
}