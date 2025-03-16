using UnityEngine;
using System.Collections.Generic;

public class SC_DrawCards : MonoBehaviour
{
    public List<GameObject> deckCards = new List<GameObject>(); // Talia kart (widoczna w Inspectorze)
    public GameObject Hand; // Obszar reprezentuj�cy r�k� (np. panel UI)

    private List<GameObject> handCards = new List<GameObject>(); // Lista kart na r�ce
    private int maxCardsInHand = 3; // Maksymalna liczba kart na r�ce

    public void OnClick()
    {
        if (handCards.Count >= maxCardsInHand) // Sprawdzenie, czy mo�na dobra� kart�
        {
            Debug.Log("Masz ju� maksymaln� liczb� kart na r�ce!");
            return;
        }

        if (deckCards.Count == 0) // Je�li talia jest pusta
        {
            Debug.Log("Brak kart w talii!");
            return;
        }

        // Losowanie karty z talii
        int randomIndex = Random.Range(0, deckCards.Count);
        GameObject randomCardPrefab = deckCards[randomIndex];

        // Tworzenie karty i dodanie jej na r�k�
        GameObject card = Instantiate(randomCardPrefab, new Vector2(0, 0), Quaternion.identity);
        card.transform.SetParent(Hand.transform, false);
        handCards.Add(card);

        // Usuni�cie karty z talii (opcjonalnie)
        deckCards.RemoveAt(randomIndex);

        Debug.Log("Dobra�e� kart�: " + randomCardPrefab.name + ". Aktualna liczba kart na r�ce: " + handCards.Count);
    }

    public void RemoveCard(GameObject card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            Destroy(card);
            Debug.Log("Usuni�to kart�. Aktualna liczba kart: " + handCards.Count);
        }
    }
}
