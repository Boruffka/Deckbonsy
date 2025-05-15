using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : MonoBehaviour
{
    public const int maxHandSize = 3;
    private int currentHandSize = 0;

    [SerializeField] private bool isPlayerHand;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<GameObject> cardObjects = new();

    public int GetMaxHandSize() => maxHandSize;
    public int GetHandSize() => currentHandSize;

    public GameObject GetCardObjectByIndex(int index) => cardObjects[index];

    public void AddCardToHand(Card addedCard)
    {
        if (cardObjects.Count >= maxHandSize)
        {
            Debug.LogWarning("Nie uda³o siê dodaæ karty – rêka pe³na.");
            return;
        }

        GameObject cardObj = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
        cardObjects.Add(cardObj);

        var container = cardObj.GetComponent<CardContainer>();
        container.SetCardInfo(addedCard);
        container.SetIsPlayerCard(isPlayerHand);
        container.SetHandIndex(cardObjects.Count - 1);
        container.UpdateCardVisuals();

        RearrangeHand();
    }


    public void RemoveCardFromHand(int index)
    {
        if (index < 0 || index >= cardObjects.Count || cardObjects[index] == null) return;

        Destroy(cardObjects[index]);
        cardObjects[index] = null;
        cardObjects.RemoveAt(index);
        currentHandSize--;
        RearrangeHand();
    }

    public void ClearHand()
    {
        foreach (var card in cardObjects)
        {
            if (card != null) Destroy(card);
        }
        cardObjects.Clear();
        currentHandSize = 0;
    }

    public void RearrangeHand()
    {
        int count = cardObjects.Count;
        float spacing = 80f;
        float center = (count - 1) / 2f;
        float angleStep = 10f;

       Vector3 handCenter = transform.GetChild(0).position + new Vector3(100f, 0, 0);


        for (int i = 0; i < count; i++)
        {
            var card = cardObjects[i];
            if (card == null) continue;

            Vector3 targetPosition = handCenter + new Vector3((i - center) * spacing, 0, 0);

            card.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
            card.transform.DORotate(new Vector3(0, 0, (i - center) * -angleStep), 0.3f);
        }
    }


    public Card GetCardByIndex(int index)
    {
        if (index < 0 || index >= cardObjects.Count) return null;
        return cardObjects[index].GetComponent<CardContainer>().GetCardInfo();
    }

    public void ListHand()
    {
        string s = "";
        foreach (var obj in cardObjects)
            s += obj != null ? "1 " : "0 ";
        Debug.Log("Zawartoœæ rêki: " + s);
    }

    public RectTransform GetNextFreeSlot(out int index)
    {
        index = cardObjects.Count < maxHandSize ? cardObjects.Count : -1;
        return index != -1 ? transform as RectTransform : null;
    }

}
