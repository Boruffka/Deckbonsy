
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
            if (occupiedBoardSpots[columnIndex, i] == false)
            {
                occupiedBoardSpots[columnIndex, i] = true;
                placedCardsObjects[columnIndex, i] = Instantiate(addedCard, boardSpots[columnIndex, i].transform.position,
                    Quaternion.identity, boardSpots[columnIndex, i]);

                CardContainer addedCardContainer = placedCardsObjects[columnIndex, i].GetComponent<CardContainer>();
                addedCardContainer.SetInPlay(true);
                addedCardContainer.SetCardInfo(addedCard.GetComponent<CardContainer>().GetCardInfo());
                addedCardContainer.SetColumnIndex(columnIndex);
                placedCards[columnIndex, i] = addedCardContainer.GetCardInfo();
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
        int checkedIndex = 2;
        if (occupiedBoardSpots[columnIndex, checkedIndex] == true && occupiedBoardSpots[columnIndex, 2 - checkedIndex] == false
            && occupiedBoardSpots[columnIndex, 2 - checkedIndex] == false)
        {
            occupiedBoardSpots[columnIndex, checkedIndex - 2] = true;
            occupiedBoardSpots[columnIndex, checkedIndex] = false;
            placedCardsObjects[columnIndex, checkedIndex].transform.position = boardSpots[columnIndex, checkedIndex - 2].position;
            placedCards[columnIndex, checkedIndex - 2] = placedCards[columnIndex, checkedIndex];
            placedCards[columnIndex, checkedIndex] = null;
        }
        else if (occupiedBoardSpots[columnIndex, checkedIndex] == true && occupiedBoardSpots[columnIndex, checkedIndex - 1] == false)
        {
            occupiedBoardSpots[columnIndex, checkedIndex - 1] = true;
            occupiedBoardSpots[columnIndex, checkedIndex] = false;
            placedCardsObjects[columnIndex, checkedIndex].transform.position = boardSpots[columnIndex, checkedIndex - 1].position;
            placedCards[columnIndex, checkedIndex - 1] = placedCards[columnIndex, checkedIndex];
            placedCards[columnIndex, checkedIndex] = null;
        }
        checkedIndex = 1;
        if (occupiedBoardSpots[columnIndex, checkedIndex] == true && occupiedBoardSpots[columnIndex, checkedIndex - 1] == false)
        {
            occupiedBoardSpots[columnIndex, checkedIndex - 1] = true;
            occupiedBoardSpots[columnIndex, checkedIndex] = false;
            placedCardsObjects[columnIndex, checkedIndex].transform.position = boardSpots[columnIndex, checkedIndex - 1].position;
            placedCards[columnIndex, checkedIndex - 1] = placedCards[columnIndex, checkedIndex];
            placedCards[columnIndex, checkedIndex] = null;
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
                RemoveCardFromColumn(columnIndex, i);
            }
        }
        DropFloatingCards(columnIndex);
        UpdateBoard();
    }

    private void RemoveCardFromColumn(int columnIndex, int rowIndex)
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
            Debug.Log("You just tried removing a card that does not exist.");
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

        for (int i = 0; i < size; i++)
        {
            GameObject newColumn = Instantiate(columnPrefab, Vector3.zero, Quaternion.identity, transform.GetChild(0));
            newColumn.GetComponent<ColumnSpot>().SetColumnIndex(i);
            newColumn.GetComponent<ColumnSpot>().SetIsPlayerBoard(playerBoard);
            columns[i] = newColumn.GetComponent<RectTransform>();

            for (int j = 0; j < size; j++)
            {
                GameObject newBoardSpot = Instantiate(boardSpotPrefab, Vector3.zero, Quaternion.identity, transform.GetChild(1));
                boardSpots[i, j] = newBoardSpot.GetComponent<RectTransform>();
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


}