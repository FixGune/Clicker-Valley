using System.Collections;
using UnityEngine;

public class UpgradeMenuController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private GameObject outsideClickBlocker;

    [Header("Анимация")]
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField]
    private AnimationCurve slideCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Состояние")]
    [SerializeField] private bool startClosed = true;

    private Vector2 openedPosition;
    private Vector2 closedPosition;
    private Coroutine slideCoroutine;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (panel == null)
        {
            Debug.LogWarning($"{name}: не назначена панель меню улучшений.");
            return;
        }

        openedPosition = panel.anchoredPosition;
        closedPosition = new Vector2(
            openedPosition.x,
            openedPosition.y - panel.rect.height
        );

        if (startClosed)
        {
            SetPanelState(false);
        }
    }

    public void ToggleMenu()
    {
        if (IsOpen)
        {
            CloseMenu();
            Debug.Log("Закрываюсь");
        }
        else
        {
            OpenMenu();
            Debug.Log("Открываюсь");
        }
    }

    public void OpenMenu()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        if (outsideClickBlocker != null)
        {
            outsideClickBlocker.SetActive(true);
        }

        StartSlide(openedPosition, true);
    }

    public void CloseMenu()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;

        if (outsideClickBlocker != null)
        {
            outsideClickBlocker.SetActive(false);
        }

        StartSlide(closedPosition, false);
    }

    public void OnOutsideClick()
    {
        CloseMenu();
    }

    private void StartSlide(Vector2 targetPosition, bool targetState)
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlidePanel(targetPosition, targetState));
    }

    private IEnumerator SlidePanel(Vector2 targetPosition, bool targetState)
    {
        if (panel == null)
        {
            yield break;
        }

        Vector2 startPosition = panel.anchoredPosition;
        float elapsedTime = 0f;

        panelCanvasGroup.blocksRaycasts = false;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float normalizedTime = Mathf.Clamp01(elapsedTime / slideDuration);
            float curveValue = slideCurve.Evaluate(normalizedTime);

            panel.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                curveValue
            );

            panelCanvasGroup.alpha = Mathf.Lerp(
                panelCanvasGroup.alpha,
                targetState ? 1f : 0f,
                curveValue
            );

            yield return null;
        }

        panel.anchoredPosition = targetPosition;
        panelCanvasGroup.alpha = targetState ? 1f : 0f;
        panelCanvasGroup.blocksRaycasts = targetState;

        slideCoroutine = null;
    }

    private void SetPanelState(bool isOpen)
    {
        IsOpen = isOpen;

        if (panel == null)
        {
            return;
        }

        panel.anchoredPosition = isOpen ? openedPosition : closedPosition;
        panelCanvasGroup.alpha = isOpen ? 1f : 0f;
        panelCanvasGroup.blocksRaycasts = isOpen;

        if (outsideClickBlocker != null)
        {
            outsideClickBlocker.SetActive(isOpen);
        }
    }
}