﻿

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



    private int CountType(CardType type)
    {
        int count = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (placedCards[i, j].cardType == type)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void AddCardToColumn(GameObject addedCard, int columnIndex)
    {
        CardContainer addedCardContainer = addedCard.GetComponent<CardContainer>();
        for (int i = 0; i < size; i++)
        {
            if (occupiedBoardSpots[columnIndex, i] == false)
            {
                occupiedBoardSpots[columnIndex, i] = true;
<<<<<<< HEAD
                placedCardsObjects[columnIndex, i] = Instantiate(addedCard, boardSpots[columnIndex, i].transform.position,
                    Quaternion.identity, boardSpots[columnIndex, i]);

                CardContainer addedCardContainer = placedCardsObjects[columnIndex, i].GetComponent<CardContainer>();
                addedCardContainer.SetCardInfo(addedCard.GetComponent<CardContainer>().GetCardInfo());
                addedCardContainer.UpdateCard();
                placedCards[columnIndex, i] = addedCardContainer.GetCardInfo();
=======
                placedCards[columnIndex, i] = addedCardContainer.GetCardInfo();
                placedCardsObjects[columnIndex, i] = Instantiate(addedCard, boardSpots[columnIndex, i].transform.position, Quaternion.identity,
                    boardSpots[columnIndex, i]);
                addedCardContainer.UpdateCard();
>>>>>>> parent of 15abde0 (1.1)
                GameManager.gameManager.PlayedCardTrigger(columnIndex, addedCardContainer.GetCardInfo().points);
                return;
            }
        }
        Debug.Log("Column full!");
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

    public void UpdateColumn(int columnIndex)
    {
<<<<<<< HEAD
        for (int i = size - 1; i > 0; i--)
=======
        for (int i = 1; i < size; i++)
>>>>>>> parent of 15abde0 (1.1)
        {
            if (occupiedBoardSpots[columnIndex, i] == true && occupiedBoardSpots[columnIndex, i - 1] == false)
            {
                occupiedBoardSpots[columnIndex, i - 1] = true;
                occupiedBoardSpots[columnIndex, i] = false;
                placedCardsObjects[columnIndex, i].transform.position = boardSpots[columnIndex, i - 1].position;
                placedCards[columnIndex, i - 1] = placedCards[columnIndex, i];
                placedCards[columnIndex, i] = null;
            }
        }
        for (int i = 1; i < size; i++)
        {
            if (occupiedBoardSpots[columnIndex, i] == true && occupiedBoardSpots[columnIndex, i - 1] == false)
            {
                occupiedBoardSpots[columnIndex, i - 1] = true;
                occupiedBoardSpots[columnIndex, i] = false;
                placedCardsObjects[columnIndex, i].transform.position = boardSpots[columnIndex, i - 1].position;
                placedCards[columnIndex, i - 1] = placedCards[columnIndex, i];
                placedCards[columnIndex, i] = null;
            }
        }
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
        UpdateColumn(columnIndex);
    }

    private void RemoveCardFromColumn(int columnIndex, int rowIndex)
    {
        if (occupiedBoardSpots[columnIndex, rowIndex] == true)
        {
            DeckManager.deckManager.AddCardToDeck(placedCards[columnIndex, rowIndex]);
            occupiedBoardSpots[columnIndex, rowIndex] = false;
            placedCards[columnIndex, rowIndex] = null;
            Destroy(placedCardsObjects[columnIndex, rowIndex]);
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

}
