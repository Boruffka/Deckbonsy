using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [Header("Main Variables")]
    static private int size = 3;
    [SerializeField] private bool playerBoard = true;
    [SerializeField] private RectTransform[] columns;
    [SerializeField] private GameObject columnPrefab;
    [SerializeField] private RectTransform[,] boardSpots;
    [SerializeField] private bool[,] occupiedBoardSpots;
    [SerializeField] private GameObject boardSpotPrefab;
    [SerializeField] private Card[,] placedCards;
    [SerializeField] private GameObject[,] placedCardsObjects;

    public int CountScore()
    {
        int score = 0;

        for (int i = 0; i < size; i++)
        {
            Dictionary<int, int> valueCounts = new Dictionary<int, int>();

            for (int j = 0; j < size; j++)
            {
                Card card = placedCards[i, j];
                if (card != null)
                {
                    if (valueCounts.ContainsKey(card.points))
                        valueCounts[card.points]++;
                    else
                        valueCounts[card.points] = 1;
                }
            }

            foreach (var pair in valueCounts)
            {
                int value = pair.Key;
                int count = pair.Value;
                score += value * count * count;
            }
        }

        return score;
    }

    public int CountTypeOnBoard(CardType type)
    {
        int count = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (placedCards[i, j] != null && placedCards[i, j].cardType == type)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public int CountTypeInColumn(CardType type, int columnIndex)
    {
        int count = 0;

        for (int i = 0; i < size; i++)
        {
            if (placedCards[columnIndex, i] != null && placedCards[columnIndex, i].cardType == type)
            {
                count++;
            }
        }
        return count;
    }

    public void AddCardToColumn(GameObject addedCard, int columnIndex)
    {
        for (int i = 0; i < size; i++)
        {
            if (!occupiedBoardSpots[columnIndex, i])
            {
                occupiedBoardSpots[columnIndex, i] = true;

                GameObject newCard = Instantiate(addedCard);
                RectTransform newCardRect = newCard.GetComponent<RectTransform>();

                newCardRect.SetParent(boardSpots[columnIndex, i], false);
                newCardRect.anchoredPosition = Vector2.zero;
                newCardRect.localRotation = Quaternion.identity;
                newCardRect.localScale = Vector3.one;

                
                placedCardsObjects[columnIndex, i] = newCard;

                CardContainer addedContainer = addedCard.GetComponent<CardContainer>();
                CardContainer newContainer = newCard.GetComponent<CardContainer>();

                newContainer.SetCardInfo(addedContainer.GetCardInfo());
                newContainer.SetColumnIndex(columnIndex);
                newContainer.SetRowIndex(i);
                newContainer.SetInPlay(true); // blokuje klikanie

                foreach (var graphic in newCard.GetComponentsInChildren<Graphic>())
                {
                    graphic.raycastTarget = false;
                }
                if (newCard.TryGetComponent<Button>(out var btn))
                {
                    btn.interactable = false;
                }

                placedCards[columnIndex, i] = newContainer.GetCardInfo();


                UpdateBoard();
                return;
            }
        }

        Debug.Log("Column full!");
    }




    public bool IsBoardEmpty()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (occupiedBoardSpots[i, j] == true)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool CheckForEmptyInColumn(int columnIndex)
    {

        for (int i = 0; i < size; i++)
        {
            if (occupiedBoardSpots[columnIndex, i] == false)
            {
                return true;
            }
        }
        return false;
    }

    public void DropFloatingCards(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= size)
        {
            Debug.LogWarning("Invalid column index");
            return;
        }

        int checkedIndex = 2;
        TryDropCard(columnIndex, checkedIndex, 2);
        TryDropCard(columnIndex, checkedIndex, 1);

        checkedIndex = 1;
        TryDropCard(columnIndex, checkedIndex, 1);
    }

    private void TryDropCard(int column, int fromRow, int toRowOffset)
    {
        int toRow = fromRow - toRowOffset;
        if (toRow < 0 || fromRow >= size || column >= size) return;

        if (occupiedBoardSpots[column, fromRow] &&
            !occupiedBoardSpots[column, toRow])
        {
            GameObject cardObj = placedCardsObjects[column, fromRow];
            if (cardObj == null || boardSpots[column, toRow] == null)
            {
                Debug.Log("Null object at column {column}, row {fromRow} or target {toRow}");
                return;
            }

            CardContainer container = cardObj.GetComponent<CardContainer>();
            if (container == null)
            {
                Debug.Log("CardContainer missing on card object.");
                return;
            }

            occupiedBoardSpots[column, toRow] = true;
            occupiedBoardSpots[column, fromRow] = false;

            container.SetRowIndex(toRow);
            cardObj.transform.position = boardSpots[column, toRow].position;
            placedCards[column, toRow] = placedCards[column, fromRow];
            placedCards[column, fromRow] = null;
            placedCardsObjects[column, toRow] = cardObj;
            placedCardsObjects[column, fromRow] = null;
        }
    }

    public void UpdateBoard()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (occupiedBoardSpots[i, j] == true && placedCardsObjects[i, j] != null)
                {
                    placedCardsObjects[i, j].GetComponent<CardContainer>().ResetCard();
                }
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (occupiedBoardSpots[i, j] == true && placedCardsObjects[i, j] != null)
                {
                    placedCardsObjects[i, j].GetComponent<CardContainer>().UpdateCard();
                }
            }
        }
        Debug.Log("Board updated!");
    }

    public void RemoveCardsFromColumn(int columnIndex, int pointValue)
    {
        for (int i = 0; i < size; i++)
        {
            if (occupiedBoardSpots[columnIndex, i] == true && placedCards[columnIndex, i].points == pointValue)
            {
                Debug.Log("Removing card at [" + columnIndex + "," + i + "] ");
                RemoveCardAtPosition(columnIndex, i);
            }
        }
        DropFloatingCards(columnIndex);
    }

    private void RemoveCardAtPosition(int columnIndex, int rowIndex)
    {
        if (occupiedBoardSpots[columnIndex, rowIndex] == true && placedCards[columnIndex, rowIndex].removable == true)
        {
            DeckManager.deckManager.AddCardToDeck(placedCards[columnIndex, rowIndex]);
            occupiedBoardSpots[columnIndex, rowIndex] = false;
            placedCards[columnIndex, rowIndex] = null;
            placedCardsObjects[columnIndex, rowIndex].GetComponent<CardContainer>().SetInPlay(false);
            Destroy(placedCardsObjects[columnIndex, rowIndex]);
            UpdateBoard();
        }
        else
        {
            if (placedCards[columnIndex, rowIndex].removable == true)
            {
                Debug.Log("Card is protected!");
            }
            else
            {
                Debug.Log("You just tried removing a card that does not exist.");
            }
        }
    }

    public void RemoveCardAtPosition(int columnIndex, int rowIndex, bool bypassProtection)
    {
        if (bypassProtection)
        {
            if (occupiedBoardSpots[columnIndex, rowIndex] == true)
            {
                DeckManager.deckManager.AddCardToDeck(placedCards[columnIndex, rowIndex]);
                occupiedBoardSpots[columnIndex, rowIndex] = false;
                placedCards[columnIndex, rowIndex] = null;
                placedCardsObjects[columnIndex, rowIndex].GetComponent<CardContainer>().SetInPlay(false);
                Destroy(placedCardsObjects[columnIndex, rowIndex]);
                UpdateBoard();
            }
            else
            {
                {
                    Debug.Log("You just tried removing a card that does not exist.");
                }
            }
        }
        else
        {
            RemoveCardAtPosition(columnIndex, rowIndex);
        }
    }

    public void ListBoard()
    {
        string s = "";
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                s += occupiedBoardSpots[j, i] + " ";
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    private void Start()
    {
        columns = new RectTransform[size];
        boardSpots = new RectTransform[size, size];
        occupiedBoardSpots = new bool[size, size];
        placedCards = new Card[size, size];
        placedCardsObjects = new GameObject[size, size];

        Transform columnParent = transform.Find("Column Spots");

        if (columnParent == null)
        {
            Debug.LogError("Brakuje Column Spots w hierarchii!");
            return;
        }

        for (int i = 0; i < size; i++)
        {
            GameObject columnObj = Instantiate(columnPrefab, columnParent);
            columnObj.name = $"Column_{i}";
            RectTransform columnRect = columnObj.GetComponent<RectTransform>();
            columns[i] = columnRect;

            ColumnSpot columnSpot = columnObj.GetComponent<ColumnSpot>();
            if (columnSpot == null)
                columnSpot = columnObj.AddComponent<ColumnSpot>();

            columnSpot.SetColumnIndex(i);
            columnSpot.SetIsPlayerBoard(playerBoard);

            GameObject cardSpotsContainer = new GameObject("Card Spots", typeof(RectTransform));
            cardSpotsContainer.transform.SetParent(columnObj.transform, false);

            RectTransform cardSpotsRect = cardSpotsContainer.GetComponent<RectTransform>();

            cardSpotsRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardSpotsRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardSpotsRect.pivot = new Vector2(0.5f, 0.5f);
            cardSpotsRect.anchoredPosition = new Vector2(0f, -90f); //no ja pierdole
            cardSpotsRect.sizeDelta = new Vector2(180, 450); 

            GridLayoutGroup layout = cardSpotsContainer.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(140, 140);
            layout.spacing = new Vector2(0, 15);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 1;
            layout.startAxis = GridLayoutGroup.Axis.Vertical;
            layout.startCorner = GridLayoutGroup.Corner.LowerLeft;
            layout.childAlignment = TextAnchor.LowerCenter;

            layout.startAxis = GridLayoutGroup.Axis.Vertical;
            layout.startCorner = GridLayoutGroup.Corner.LowerLeft;
            layout.childAlignment = TextAnchor.LowerCenter;


            for (int j = 0; j < size; j++)
            {
                GameObject spot = Instantiate(boardSpotPrefab, cardSpotsContainer.transform);
                spot.name = $"Spot_{i}_{j}";
                boardSpots[i, j] = spot.GetComponent<RectTransform>();
                occupiedBoardSpots[i, j] = false;
            }
        }
    }



    public bool IsBoardFull()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!occupiedBoardSpots[i, j])
                    return false;
            }
        }
        return true;
    }
    public void ClearBoard()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (occupiedBoardSpots[i, j])
                {
                    Destroy(placedCardsObjects[i, j]);
                    placedCards[i, j] = null;
                    placedCardsObjects[i, j] = null;
                    occupiedBoardSpots[i, j] = false;
                }
            }
        }
    }

    public Card GetCardAtPosition(int columnIndex, int rowIndex)
    {
        if (occupiedBoardSpots[columnIndex, rowIndex])
        {
            return placedCards[columnIndex, rowIndex];
        }
        Debug.Log("Error in GetCardAtPosition(" + columnIndex + "," + rowIndex + ")");
        return null;
    }
    public CardContainer GetCardContainerAtPosition(int columnIndex, int rowIndex)
    {
        if (occupiedBoardSpots[columnIndex, rowIndex])
        {
            return placedCardsObjects[columnIndex, rowIndex].GetComponent<CardContainer>();
        }
        Debug.Log("Error in GetCardContainerAtPosition(" + columnIndex + "," + rowIndex + ")");
        return null;
    }

}