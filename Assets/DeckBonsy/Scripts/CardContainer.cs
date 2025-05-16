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
    [SerializeField] private int rowIndex;
    [SerializeField] private TextMeshProUGUI handPower;
    [SerializeField] private TextMeshProUGUI handName;
    [SerializeField] private bool inPlay;


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
        if (!inPlay && isPlayerCard && GameManager.gameManager.GetPlayerTurn())
        {
            GameManager.gameManager.SetChosenCardIndex(handIndex, true);
        }
    }


    public void UpdateCardVisuals()
    {
        handPower.text = "" + cardInfo.points;
        // handName.text = "" + cardInfo.cardName;
        GetComponent<Image>().sprite = cardInfo.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("ONMOUSEOVER!" + cardInfo + isPlayerCard);
        if (cardInfo != null)//&& isPlayerCard)
            HandManager.handManager.ShowCardDescription(cardInfo.cardDescription);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        HandManager.handManager.HideCardDescription();
    }
}