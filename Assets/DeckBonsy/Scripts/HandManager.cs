using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HandManager : MonoBehaviour
{
    public static HandManager handManager { get; private set; }

    [Header("References")]
    public Hand playerHand;
    public Hand enemyHand;
    [SerializeField] private GameObject descriptionBox;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private bool isHoveringCard = false;
    private Vector2 boxOffset = new Vector2(160f, -60f);

    private void Awake()
    {
        if (handManager != null && handManager != this)
            Destroy(this);
        else
            handManager = this;

        if (descriptionBox != null)
            descriptionBox.SetActive(false);

        CanvasGroup cg = descriptionBox.GetComponent<CanvasGroup>();
        if (cg == null) cg = descriptionBox.AddComponent<CanvasGroup>();

        cg.blocksRaycasts = false;   
        cg.interactable = false;
    }

    private void Update()
    {
        if (isHoveringCard && descriptionBox != null)
        {
            descriptionBox.SetActive(true);
            descriptionBox.transform.position = Input.mousePosition + (Vector3)boxOffset;
        }
        else if (descriptionBox != null)
        {
            descriptionBox.SetActive(false);
        }

    }

    public void ShowCardDescription(string desc)
    {
        isHoveringCard = true;
        descriptionText.text = desc;
    }

    public void HideCardDescription()
    {
        isHoveringCard = false;
    }

    public int GetMaxHandSize()
    {
        return GameManager.gameManager.GetPlayerTurn() ? playerHand.GetMaxHandSize() : enemyHand.GetMaxHandSize();
    }

    public int GetHandSize()
    {
        return GameManager.gameManager.GetPlayerTurn() ? playerHand.GetHandSize() : enemyHand.GetHandSize();
    }

    public Card GetCardByIndex(int index)
    {
        return GameManager.gameManager.GetPlayerTurn() ? playerHand.GetCardByIndex(index) : enemyHand.GetCardByIndex(index);
    }

    public GameObject GetCardObjectByIndex(int index)
    {
        return GameManager.gameManager.GetPlayerTurn() ? playerHand.GetCardObjectByIndex(index) : enemyHand.GetCardObjectByIndex(index);
    }

    public void ListHand()
    {
        if (GameManager.gameManager.GetPlayerTurn()) playerHand.ListHand();
        else enemyHand.ListHand();
    }

    public void AddCardToHand(Card addedCard)
    {
        if (GameManager.gameManager.GetPlayerTurn()) playerHand.AddCardToHand(addedCard);
        else enemyHand.AddCardToHand(addedCard);
    }

    public void RemoveCardFromHand(int index)
    {
        if (GameManager.gameManager.GetPlayerTurn()) playerHand.RemoveCardFromHand(index);
        else enemyHand.RemoveCardFromHand(index);
    }

    public void ClearHand()
    {
        if (GameManager.gameManager.GetPlayerTurn()) playerHand.ClearHand();
        else enemyHand.ClearHand();
    }

    public void ClearAllHands()
    {
        playerHand.ClearHand();
        enemyHand.ClearHand();
    }
}
