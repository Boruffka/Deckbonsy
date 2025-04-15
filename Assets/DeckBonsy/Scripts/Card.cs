using System.Collections;
using UnityEngine;

public class Card
{
    public int id { get; private set; }
    public string cardName { get; private set; }
    public string cardDescription { get; private set; }
    public int effectId { get; private set; }
    public int basePoints { get; private set; }
    public int points { get; private set; }
    public CardType cardType { get; private set; }


    public void SetValues(int _id, string _cardName, string _cardDescription, int _effectId, int _basePoints, CardType _cardType)
    {
        id = _id;
        cardName = _cardName;
        cardDescription = _cardDescription;
        effectId = _effectId;
        basePoints = _basePoints;
        points = basePoints;
        cardType = _cardType;
    }

    public void SetValues((int _id, string _cardName, string _cardDescription, int _effectId, int _basePoints, CardType _cardType) _cardValue)
    {
        id = _cardValue._id;
        cardName = _cardValue._cardName;
        cardDescription = _cardValue._cardDescription;
        effectId = _cardValue._effectId;
        basePoints = _cardValue._basePoints;
        points = basePoints;
        cardType = _cardValue._cardType;
    }

    public void CopyFrom(Card other)
    {
        id = other.id;
        cardName = other.cardName;
        cardDescription = other.cardDescription;
        effectId = other.effectId;
        basePoints = other.basePoints;
        points = basePoints;
        cardType = other.cardType;
    }

    public void SetPoints(int _points)
    {
        points = _points;
    }
    
    
    public void IncreasePoints(int increase)
    {
        points += increase;
    }
    
}
