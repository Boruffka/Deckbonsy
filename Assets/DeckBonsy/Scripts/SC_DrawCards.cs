using UnityEngine;
using System.Collections.Generic;

public class SC_DrawCards : MonoBehaviour
{
    public GameObject[] deckCards; // Talia kart (r�ne prefabry)
    public GameObject Hand;

    private List<GameObject> handCards = new List<GameObject>();
    private int maxCardsInHand = 3;

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

        Debug.Log("Dobra�e� kart�. Aktualna liczba kart: " + handCards.Count);
    }

    public void RemoveCard(GameObject card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            Debug.Log("Usuni�to kart�. Aktualna liczba kart: " + handCards.Count);
        }
    }
}
