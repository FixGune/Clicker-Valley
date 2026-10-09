using UnityEngine;
using UnityEngine.EventSystems;


public class ClickableBuilding3D : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Ссылки")]
    [SerializeField] private ClickerController clickerController;

    [Header("Визуальная обратная связь")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float hoverScale = 1.03f;

    private bool isHovered;


    private void OnValidate()
    {
        if (normalScale <= 0f)
        {
            normalScale = 1f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickerController == null)
        {
            Debug.LogWarning(
                $"{name}: не назначен ClickerController.");
            return;
        }

        clickerController.OnBuildingClicked();

        StartCoroutine(PlayClickAnimation());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        transform.localScale =
            Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        transform.localScale =
            Vector3.one * normalScale;
    }

    private System.Collections.IEnumerator PlayClickAnimation()
    {
        transform.localScale =
            Vector3.one * pressedScale;

        yield return new WaitForSeconds(0.08f);

        transform.localScale = isHovered
            ? Vector3.one * hoverScale
            : Vector3.one * normalScale;
    }
}