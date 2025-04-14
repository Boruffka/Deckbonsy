using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CardContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Card cardInfo;
    [SerializeField] private bool isPlayerCard;
    [SerializeField] private int handIndex;
    [SerializeField] private TextMeshProUGUI handPower;
    [SerializeField] private TextMeshProUGUI handName;
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

    public void SetIsPlayerCard(bool _isPlayerCard)
    {
        isPlayerCard = _isPlayerCard;
    }

    public void SetHandIndex(int _handIndex)
    {
        handIndex = _handIndex;
    }

    public void WhenClicked()
    {
        if(!inPlay)
            GameManager.gameManager.SetChosenCardIndex(handIndex, isPlayerCard);
    }

    public void UpdateCard()
    {
        handPower.text = "" + cardInfo.points;
        handName.text = "" + cardInfo.cardName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("ONMOUSEOVER!" + cardInfo + isPlayerCard);
        if (cardInfo != null && isPlayerCard)
            HandManager.handManager.ShowCardDescription(cardInfo.cardDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPlayerCard)
            HandManager.handManager.HideCardDescription();
    }
}
