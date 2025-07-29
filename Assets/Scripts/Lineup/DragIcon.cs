using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	RectTransform rectTransform;
	Canvas canvas;
	CanvasGroup canvasGroup;
    Vector2 position;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
        canvas = FindFirstObjectByType<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();
        position = rectTransform.localPosition;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = position;
	}
}
