using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler

{
    [SerializeField] private Card cardInfo;
    [SerializeField] private bool isPlayerCard;
    [SerializeField] private int handIndex;
    [SerializeField] private int columnIndex;
    [SerializeField] private int rowIndex;
    [SerializeField] private TextMeshProUGUI handPower;
    [SerializeField] private TextMeshProUGUI handName;
    [SerializeField] private bool inPlay;

    private Tween floatingTween;
    private bool isSelected = false;


    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void ResetCard()
    {
        cardInfo.SetRemovable(true);
        cardInfo.SetStealable(true);
    }
    public bool GetInPlay()
    {
        return inPlay;
    }

    public void SetInPlay(bool _inPlay)
    {
        inPlay = _inPlay;
    }

    public Card GetCardInfo()
    {
        return cardInfo;
    }

    public void SetCardInfo(Card _cardInfo)
    {
        cardInfo = _cardInfo;
    }

    public bool GetIsPlayerCard()
    {
        return isPlayerCard;
    }

    public void SetIsPlayerCard(bool _isPlayerCard)
    {
        isPlayerCard = _isPlayerCard;
    }

    public void SetHandIndex(int _handIndex)
    {
        handIndex = _handIndex;
    }

    public void SetColumnIndex(int _columnIndex)
    {
        columnIndex = _columnIndex;
    }

    public int GetColumnIndex()
    {
        return columnIndex;
    }
  
    public void SetRowIndex(int _rowIndex)
    {
        rowIndex = _rowIndex;
    }

    public int GetRowIndex()
    {
        return rowIndex;
    }
    public void UpdateCard()
    {
        cardInfo.SetPoints(cardInfo.basePoints);
        EffectManager.effectManager.TriggerCardEffect(cardInfo.effectId, this, null);
        UpdateCardVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isMyTurn = GameManager.gameManager.GetPlayerTurn() == isPlayerCard;
        if (!isMyTurn)
            return;

        DeselectAllCards();

        GameManager.gameManager.SetChosenCardIndex(handIndex, isPlayerCard);
        GameManager.gameManager.selectedCardIndex = handIndex;

        SelectCard();
    }

    public void UpdateCardVisuals()
    {
        handPower.text = "" + cardInfo.points;
        // handName.text = "" + cardInfo.cardName;
        GetComponent<Image>().sprite = cardInfo.sprite;
    }

    private Vector3 originalPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool isMyTurn = GameManager.gameManager.GetPlayerTurn() == isPlayerCard;
        if (!isMyTurn)
            return;

        if (!inPlay && !isSelected)
        {
            transform.DOLocalMoveY(originalPosition.y + 30f, 0.2f).SetEase(Ease.OutQuad);
        }

        if (isSelected)
        {
            transform.DOScale(1.1f, 0.2f); // powiększ kartę
        }

        if (cardInfo != null)
            HandManager.handManager.ShowCardDescription(cardInfo.cardDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!inPlay && !isSelected)
        {
            transform.DOLocalMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad);
        }

        if (isSelected)
        {
            transform.DOScale(1f, 0.2f); // wróć do normalnej skali
        }

        HandManager.handManager.HideCardDescription();
    }

    public void SelectCard()
    {
        if (isSelected) return;
        isSelected = true;

        transform.DOLocalMoveY(originalPosition.y + 90f, 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            floatingTween = transform.DOLocalMoveY(originalPosition.y + 95f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }


    public void DeselectCard()
    {
        isSelected = false;

        if (floatingTween != null && floatingTween.IsActive()) floatingTween.Kill();
        transform.DOLocalMoveY(originalPosition.y, 0.25f).SetEase(Ease.InOutQuad);
        transform.DOScale(Vector3.one, 0.2f); // wróć do normalnej wielkości
    }
    public static void DeselectAllCards()
    {
        foreach (var card in FindObjectsOfType<CardContainer>())
        {
            card.DeselectCard();
        }
    }

    public void UpdateInteractivityVisual()
    {
        bool isMyTurn = GameManager.gameManager.GetPlayerTurn() == isPlayerCard;
        Image image = GetComponent<Image>();

        if (image != null)
        {
            image.color = isMyTurn ? Color.white : new Color(1, 1, 1, 0.5f); // przyciemnij nieaktywną
        }
    }

}