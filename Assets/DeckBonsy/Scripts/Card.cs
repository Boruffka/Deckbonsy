using System.Collections;
using UnityEngine;

public class Card
{
    public int id { get; private set; }
    public string cardName { get; private set; }
    public string cardDescription { get; private set; }
    public int effectId { get; private set; }
    public int points { get; private set; }
    public CardType cardType { get; private set; }


    public void SetValues(int _id, string _cardName, string _cardDescription, int _effectId, int _points, CardType _cardType)
    {
        id = _id;
        cardName = _cardName;
        cardDescription = _cardDescription;
        effectId = _effectId;
        points = _points;
        cardType = _cardType;
    }

    public void SetValues((int _id, string _cardName, string _cardDescription, int _effectId, int _points, CardType _cardType) _cardValue)
    {
        id = _cardValue._id;
        cardName = _cardValue._cardName;
        cardDescription = _cardValue._cardDescription;
        effectId = _cardValue._effectId;
        points = _cardValue._points;
        cardType = _cardValue._cardType;
    }

    public void CopyFrom(Card other)
    {
        id = other.id;
        cardName = other.cardName;
        cardDescription = other.cardDescription;
        effectId = other.effectId;
        points = other.points;
        cardType = other.cardType;
    }

    public void SetPoints(int _points)
    {
        points = _points;
    }
    
}
