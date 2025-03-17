using UnityEngine;

public class DropZone : MonoBehaviour
{
    public void OnCardDropped(CardWrapper card)
    {
        // Ustawiamy kart� jako dziecko DropZone
        card.transform.SetParent(transform);
        card.transform.position = transform.position; // Mo�esz ustawi� kart� w innej pozycji w strefie

        Debug.Log("Karta dodana do DropZone!");
    }
}