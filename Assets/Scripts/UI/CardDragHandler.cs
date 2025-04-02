using UdonSharp;
using UnityEngine;
using UnityEngine.EventSystems;
using VRC.SDKBase;
using VRC.Udon;

public class CardDragHandler : UdonSharpBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private string cardId;
    private DeckEditorUI deckEditorUI;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public void Setup(string id, DeckEditorUI ui)
    {
        cardId = id;
        deckEditorUI = ui;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        originalPosition = rectTransform.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // マウス位置にカードを追従
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos
        );
        rectTransform.position = canvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // ドロップ位置を取得してデッキエディターに通知
        deckEditorUI.OnCardDropped(cardId, rectTransform.position);

        // 元の位置に戻す（実際のカードはデッキエディターによって適切な位置に生成される）
        rectTransform.position = originalPosition;
    }
}
