using UnityEngine;
using System.Collections.Generic;

public class SC_DrawCards : MonoBehaviour
{
    public GameObject[] deckCards; 
    public GameObject Hand;

    private List<GameObject> handCards = new List<GameObject>();
    private int maxCardsInHand = 3;


    public void RemoveCard(GameObject card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card); 
            Debug.Log("Usuni�to kart�. Aktualna liczba kart: " + handCards.Count);
            UpdateHandCardCount();
        }
    }

   
    public void UpdateHandCardCount()
    {
        int currentHandCount = handCards.Count;
        Debug.Log("Aktualna liczba kart na r�ce: " + currentHandCount);

        
    }

    public void OnClick()
    {
        if (handCards.Count >= maxCardsInHand)
        {
            Debug.Log("Masz ju� maksymaln� liczb� kart na r�ce!");
            return;
        }

        if (deckCards.Length == 0)
        {
            Debug.Log("Talia jest pusta!");
            return;
        }

        int randomIndex = Random.Range(0, deckCards.Length);
        GameObject card = Instantiate(deckCards[randomIndex], Hand.transform, false);
        handCards.Add(card);

        UpdateHandCardCount(); 

        Debug.Log("Dobra�e� kart�. Aktualna liczba kart: " + handCards.Count);
    }
}
