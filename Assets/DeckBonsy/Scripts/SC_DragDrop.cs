using UnityEngine;
using UnityEngine.EventSystems;

public class SC_Roman_Elite : MonoBehaviour
{
    private bool IsDragging = false;
    private Vector3 startPosition;
    private Transform startParent;

    void Start()
    {
        startParent = transform.parent; // Zapisujemy pocz�tkowego rodzica (r�ka)
    }

    public void StartDrag()
    {
        IsDragging = true;
        startPosition = transform.position;
    }

    public void EndDrag()
    {
        IsDragging = false;

        GameObject dropZone = GameObject.Find("DropZone"); // Znajd� DropZone
        if (dropZone != null && IsOverDropZone(dropZone))
        {
            transform.SetParent(dropZone.transform, false); // Ustawienie karty w DropZone
            transform.position = dropZone.transform.position; // Blokowanie pozycji karty
        }
        else
        {
            transform.position = startPosition; // Je�li nie trafi�a w DropZone, wraca na r�k�
        }
    }

    void Update()
    {
        if (IsDragging)
        {
            transform.position = Input.mousePosition; // Karta porusza si� za myszk�
        }
    }

    private bool IsOverDropZone(GameObject dropZone)
    {
        RectTransform dropRect = dropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropRect, Input.mousePosition);
    }
}
