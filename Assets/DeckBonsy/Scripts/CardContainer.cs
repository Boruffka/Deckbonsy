using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Card cardInfo;
    [SerializeField] private bool isPlayerCard;
    [SerializeField] private int handIndex;
    [SerializeField] private int columnIndex;
    [SerializeField] private TextMeshProUGUI handPower;
    [SerializeField] private bool inPlay;

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

    public void WhenClicked()
    {
        if (!inPlay)
            GameManager.gameManager.SetChosenCardIndex(handIndex, isPlayerCard);
        else
            GameManager.gameManager.SetChosenCardInPlayObject(this);
    }

    public void UpdateCard()
    {
        cardInfo.SetPoints(cardInfo.basePoints);
        EffectManager.effectManager.TriggerCardEffect(cardInfo.effectId, this, null);
        UpdateCardVisuals();
    }

    public void UpdateCardVisuals()
    {
        handPower.text = "" + cardInfo.points;
      // handName.text = "" + cardInfo.cardName;
        GetComponent<Image>().sprite = cardInfo.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardInfo != null)
            HandManager.handManager.ShowCardDescription(cardInfo.cardDescription);

        // powieksz
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandManager.handManager.HideCardDescription();

        // przywróć normalną skalę
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
    }


}