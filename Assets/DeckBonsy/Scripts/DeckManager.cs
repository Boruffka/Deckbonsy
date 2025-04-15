using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager deckManager { get; private set; }

    [Header("Main Variables")]
    private const int startingDeckSize = 10;
    [SerializeField] private Card[] startingDeck;
    [SerializeField] private List<Card> cardsInDeck;

    private void Awake()
    {
        /// Singleton mechanism
        {
            if (deckManager != null && deckManager != this)
            {
                Destroy(this);
            }
            else
            {
                deckManager = this;
            }
        }

        startingDeck = new Card[startingDeckSize];
        cardsInDeck = new List<Card>();
    }

    private void Start()
    {
        MockDeck();
        ResetDeck();
        ShuffleDeck();
        //ListDeck();
    }

    public void MockDeck()
    {
        // id, name, description, effectId, points, cardType
        (int, string, string, int, int, CardType)[] cardValues = new (int, string, string, int, int, CardType)[]
        {
        (0, "Slave","Bazowa jednostka", 0, 1, CardType.Slave),
        (1, "Emperor","Nie może zostać ukradziony, jednak w dalszym ciągu może być zbity", 1, 6, CardType.Emperor),
        (2, "Kaeso","-1 punkt do karty od każdej karty Politycznej na planszy, +1 do każdej karty Slave na planszy", 2, 5, CardType.Politician),
        (3, "Octavian","Dodaje kartę Citizena na rękę. Boostują go karty Citizenów w tej samej kolumnie", 3, 4, CardType.Politician),
        (4, "Domina Livia Versus","Boostują ją soldierzy w tej samej kolumnie ", 6, 3, CardType.Politician),
        (5, "Maximus Aulus","", 4, 4, CardType.Politician),
        (6, "Magnus","Wybiera kartę przeciwnika z planszy z zakresu punktacji i cofa ją na jego rękę, lub do talii, gdy nie ma miejsca ", 5, 4, CardType.Politician),
        (7, "Soldier","Bazowa jednostka", 0, 1, CardType.Soldier),
        (8, "Elite Soldier","Bazowa jednostka, ulepszony Soldier", 0, 2, CardType.Soldier),
        (9, "Citizen","Bazowa jednostka", 0, 1, CardType.Citizen),
        (10, "Najwyższy Kapłan","Daje immunity wybranej karcie w rzędzie (domyślnie karta poniżej, jednak jeśli karta Najwyższego Kapłana jest kartą na spodzie rzędu, wtedy immunity przechodzi na kartę powyżej", 4, 4, CardType.Politician),
        (11, "Infiltrator","Gracz może podejrzeć pierwszą kartę z wierzchu stosu kart przeciwnika",7, 2,CardType.Citizen),
        (12, "Złodziej", " Gracz zamienia dowolną kartę przeciwnika ze swoją kartą Złodzieja i bierze ją na rękę",8,0,CardType.Citizen)
        
        };

        for (int i = 0; i < startingDeckSize; i++)
        {
            Card createdCard = new Card();
            createdCard.SetValues(cardValues[i]);
            startingDeck[i] = createdCard;
        }
    }

    public void ResetDeck()
    {
        foreach (Card card in startingDeck)
        {
            cardsInDeck.Add(card);
        }
    }
    private void ShuffleDeck()
    {
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            Card temp = cardsInDeck[i];
            int randomIndex = Random.Range(i, cardsInDeck.Count);
            cardsInDeck[i] = cardsInDeck[randomIndex];
            cardsInDeck[randomIndex] = temp;
        }
    }
    public void AddCardToDeck(Card addedCard)
    {
        cardsInDeck.Add(addedCard);
    }

    public void ListDeck()
    {
        foreach (Card card in cardsInDeck)
        {
            Debug.Log(card.cardName);
        }
    }

    public void DrawCard()
    {
        if (HandManager.handManager.GetHandSize() >= HandManager.handManager.GetMaxHandSize())
        {
            // Dodać komunikat, że nie można dobrać więcej kart
            Debug.Log("Max hand size reached!");
            return;
        }
        if (cardsInDeck.Count > 0)
        {
            Card temp = cardsInDeck[cardsInDeck.Count - 1];
            HandManager.handManager.AddCardToHand(temp);
            cardsInDeck.Remove(temp);
            GameManager.gameManager.EndTurn();
        }
    }
}
