using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public static DeckManager deckManager { get; private set; }

    [Header("Main Variables")]
    private const int startingDeckSize = 19;
    [SerializeField] private Card[] startingDeck;
    [SerializeField] private List<Card> cardsInDeck;
    [SerializeField] private Sprite[] cardSprite;
    [SerializeField] private Button playerDrawButton;
    [SerializeField] private Button enemyDrawButton;
    private Image playerDrawImage;
    private Image enemyDrawImage;
    


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
        ListDeck();

        playerDrawImage = playerDrawButton.GetComponent<Image>();
        enemyDrawImage = enemyDrawButton.GetComponent<Image>();

        UpdateDrawButtons(GameManager.gameManager.GetPlayerTurn());
    }


    public void MockDeck()
    {
        // id, name, description, effectId, points, cardType, sprite
        (int, string, string, int, int, CardType, Sprite)[] cardValues = new (int, string, string, int, int, CardType, Sprite)[]
        {
        (0, "Slave","NIEWOLNIK\nJednostka bazowa o wartości 1.", 0, 1, CardType.Slave,cardSprite[0]),
        (1, "Slave","NIEWOLNIK\nJednostka bazowa o wartości 1.", 0, 1, CardType.Slave,cardSprite[1]),
        (2, "Slave","NIEWOLNIK\nJednostka bazowa o wartości 1.", 0, 1, CardType.Slave,cardSprite[2]),
        (3, "Emperor","IMPERATOR\nKarta specjalna o wartości 6.\nPostrach: Nie może zostać ukradziony przez przeciwnika", 1, 6, CardType.Emperor,cardSprite[3]),
        (4, "Kaeso","SENATOR KAESO MARCELLUS IGNATIUS\nKarta specjalna o wartości 5.\nWsparcie w wyzwoleniu: Dodaje od każdej karty Niewolnik na planszy 1 punkt. Opozycja: Odejmuje 1 punkt za każdą osobę polityczną na planszy.", 2, 5, CardType.Politician,cardSprite[4]),
        (5, "Octavian Helion","PIERWSZY SENATOR OCTAVIANUS HELION\nKarta specjalna o wartości 4.\nZwolennicy: Dodaje kartę Mieszczanin z talii na rękę.\nManipulacja tlumem: Karty Mieszczan umieszczone w tej samej kolumnie zwiększają jego wartość o równowartość ich ilości pomnożonej przez siebie.", 3, 4, CardType.Politician,cardSprite[5]),
        (6, "Domina Livia Versus","DOMINA LIVIA VERUS\nKarta specjalna o wartości 3.\nDowódczyni: Zwiększa swoją wartość o 1 dla każdej karty Żolnierz i karty Gwardzista umieszczonej na planszy po stronie gracza. ", 6, 3, CardType.Politician,cardSprite[6]),
        (7, "Pontifex Maximus Aulus","NAJWYŻSZY KAPŁAN\nKarta specjalna o wartości 4.\nModlitwa: Sprawia, że inna karta w tym samym rzędzie staje się nietykalna(z pierwszeństwem dla karty znajdującej się poniżej)", 4, 4, CardType.Politician,cardSprite[7]),
        (8, "Magnus","SZEF TAJNEJ POLICJI\nKarta specjalna o wartości 4.\nZastraszenie: Cofa wybraną kartę przeciwnika z planszy na jego rękę.", 5, 4, CardType.Politician,cardSprite[8]),
        (9, "Solider","ŻOŁNIERZ\nJednostka bazowa o wartości 1.", 0, 1, CardType.Soldier,cardSprite[9]),
        (10, "Soldier","ŻOŁNIERZ\nJednostka bazowa o wartości 1.", 0, 1, CardType.Soldier,cardSprite[10]),
        (11, "Soldier","ŻOŁNIERZ\nJednostka bazowa o wartości 1.", 0, 1, CardType.Soldier,cardSprite[11]),
        (12, "Elite Soldier","GWARDZISTA\nJednostka bazowa o wartości 2.", 0, 2, CardType.Soldier,cardSprite[12]),
        (13, "Elite Soldier","GWARDZISTA\nJednostka bazowa o wartości 2.", 0, 2, CardType.Soldier,cardSprite[13]),
        (14, "Elite Soldier","GWARDZISTA\nJednostka bazowa o wartości 2.", 0, 2, CardType.Soldier,cardSprite[14]),
        (15, "Citizen","MIESZCZANIN\nJednostka bazowa o wartości 1.", 0, 1, CardType.Citizen,cardSprite[15]),
        (16, "Citizen","MIESZCZANIN\nJednostka bazowa o wartości 1.", 0, 1, CardType.Citizen,cardSprite[16]),
        (17, "Citizen","MIESZCZANIN\nJednostka bazowa o wartości 1.", 0, 1, CardType.Citizen,cardSprite[17]),
        (18, "Infiltrator","INFILTRATOR\nKarta specjalna o wartości 2.\nBadanie terenu: Zdradza tożsamość pierwszej karty z wierzchu stosu przeciwnika.",7, 2,CardType.Citizen,cardSprite[18]),
        (19, "Złodziej", "ZŁODZIEJ\nKarta specjalna o wartości 0.\nKradzież tożsamości: Umieszcza dowolną kartę przeciwnika z planszy na ręce gracza i zajmuje jej miejsce.",8,0,CardType.Citizen,cardSprite[19])
        
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
        cardsInDeck.Clear();
        foreach (Card card in startingDeck)
        {
            cardsInDeck.Add(card);
        }
    }
    public void ShuffleDeck()
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
            Debug.Log("Max hand size reached!");
            return;
        }

        if (cardsInDeck.Count > 0)
        {
            Card temp = cardsInDeck[^1];
            HandManager.handManager.AddCardToHand(temp);
            cardsInDeck.RemoveAt(cardsInDeck.Count - 1);

            GameManager.gameManager.EndTurn();  // Automatycznie zmienia turę
            UpdateDrawButtons(GameManager.gameManager.GetPlayerTurn());
        }
    }

    public void UpdateDrawButtons(bool isPlayerTurn)
    {
        if (playerDrawButton != null)
            playerDrawButton.GetComponent<Image>().color = isPlayerTurn ? Color.white : new Color(1, 1, 1, 0.3f); // aktywny lub przyciemniony

        if (enemyDrawButton != null)
            enemyDrawButton.GetComponent<Image>().color = !isPlayerTurn ? Color.white : new Color(1, 1, 1, 0.3f); // przeciwnik odwrotnie

        playerDrawButton.GetComponent<Button>().interactable = isPlayerTurn;
        enemyDrawButton.GetComponent<Button>().interactable = !isPlayerTurn;
    }


}
