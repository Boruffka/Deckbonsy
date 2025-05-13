using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("Main Variables")]
    public const int maxHandSize = 3;
    private int currentHandSize = 0;
    [SerializeField] private bool isPlayerHand;
    [SerializeField] private RectTransform[] positions;
    [SerializeField] private GameObject handSpotPrefab;
    [SerializeField] private bool[] positionsOccupied;
    [SerializeField] private GameObject[] cardObjects;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Card[] hand = new Card[maxHandSize];

    public int GetMaxHandSize()
    {
        return maxHandSize;
    }
    public int GetHandSize()
    {
        return currentHandSize;
    }

    public Card GetCardByIndex(int index)
    {
        return hand[index];
    }

    public GameObject GetCardObjectByIndex(int index)
    {
        return cardObjects[index];
    }

    public void AddCardToHand(Card addedCard)
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            if (positionsOccupied[i] == false)
            {
                positionsOccupied[i] = true;
                cardObjects[i] = Instantiate(cardPrefab, positions[i].position, Quaternion.identity);
                cardObjects[i].GetComponent<CardContainer>().SetCardInfo(addedCard);
                cardObjects[i].GetComponent<CardContainer>().SetIsPlayerCard(isPlayerHand);
                cardObjects[i].GetComponent<Transform>().SetParent(positions[i].GetComponent<Transform>());
                cardObjects[i].GetComponent<CardContainer>().SetHandIndex(i);
                cardObjects[i].GetComponent<CardContainer>().UpdateCardVisuals();
                currentHandSize++;
                return;
            }
        }
    }

    public void RemoveCardFromHand(int index)
    {
        Destroy(cardObjects[index]);
        positionsOccupied[index] = false;
        hand[index] = null;
        currentHandSize--;
    }

    public void ListHand()
    {
        string s = "";
        for (int i = 0; i < maxHandSize; i++)
        {
            s += positionsOccupied[i];
        }
        Debug.Log(s);
    }

    private void Awake()
    {

    }

    private void Start()
    {
        currentHandSize = 0;
        cardObjects = new GameObject[maxHandSize];
        positions = new RectTransform[maxHandSize];
        positionsOccupied = new bool[maxHandSize];

        for (int i = 0; i < maxHandSize; i++)
        {
            GameObject newHandSpot = Instantiate(handSpotPrefab, Vector3.zero, Quaternion.identity, transform.GetChild(0));
            positions[i] = newHandSpot.GetComponent<RectTransform>();
            positionsOccupied[i] = false;
        }
    }

    public void ClearHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            if (positionsOccupied[i])
            {
                Destroy(cardObjects[i]);
                hand[i] = null;
                cardObjects[i] = null;
                positionsOccupied[i] = false;
            }
        }

        currentHandSize = 0;
    }

}