using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class CardBehaviour : UdonSharpBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private VRCPickup pickup;
    [SerializeField] private MeshRenderer cardRenderer;
    [SerializeField] private BoxCollider cardCollider;
    
    [Header("Card Data")]
    [SerializeField] private string cardId;
    [SerializeField] private bool isFaceUp = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isHeld = false;

    void Start()
    {
        if (pickup == null)
            pickup = GetComponent<VRCPickup>();
        
        if (cardRenderer == null)
            cardRenderer = GetComponent<MeshRenderer>();
        
        if (cardCollider == null)
            cardCollider = GetComponent<BoxCollider>();

        // 初期位置と回転を保存
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public override void OnPickup()
    {
        isHeld = true;
    }

    public override void OnDrop()
    {
        isHeld = false;
        // ドロップ時の位置が有効なプレイエリア内かチェック
        ValidateCardPosition();
    }

    public void FlipCard()
    {
        if (!isHeld) return;

        isFaceUp = !isFaceUp;
        transform.Rotate(0, 0, 180);
        
        // ネットワーク同期のための準備（後で実装）
        RequestSerialization();
    }

    private void ValidateCardPosition()
    {
        // プレイエリア外にドロップされた場合、元の位置に戻す
        // 後でプレイエリアの境界チェックを実装
        bool isValidPosition = true; // 仮の実装

        if (!isValidPosition)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }

    // カード情報の設定
    public void SetCardData(string id, Texture2D frontTexture)
    {
        cardId = id;
        if (cardRenderer != null && frontTexture != null)
        {
            cardRenderer.material.mainTexture = frontTexture;
        }
    }

    // カードの位置をリセット
    public void ResetPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isFaceUp = false;
    }
}
