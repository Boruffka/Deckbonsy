using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ColumnSpot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isPlayerBoard;
    [SerializeField] private int columnIndex;

    private Image backgroundImage;
    private Tween highlightTween;

    private static ColumnSpot currentlyHighlightedColumn;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
            backgroundImage = gameObject.AddComponent<Image>();

        backgroundImage.color = new Color(1f, 1f, 1f, 0f); // ca³kowicie przezroczysty na starcie
    }


    public void SetIsPlayerBoard(bool _isPlayerBoard) => isPlayerBoard = _isPlayerBoard;
    public void SetColumnIndex(int _columnIndex) => columnIndex = _columnIndex;

    public void WhenClicked() => GameManager.gameManager.SetChosenColumnIndex(columnIndex, isPlayerBoard);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardContainer.IsAnyCardSelected()) return;
        if (GameManager.gameManager.GetPlayerTurn() != isPlayerBoard) return;

        highlightTween?.Kill();
        highlightTween = backgroundImage.DOColor(new Color(0f, 1f, 1f, 0.4f), 0.25f); // cyan z przezroczystoœci¹
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHighlight();
    }

    public void ResetHighlight()
    {
        highlightTween?.Kill();
        highlightTween = backgroundImage
            .DOColor(new Color(1f, 1f, 1f, 0f), 0.25f); // z powrotem przezroczysty
        if (currentlyHighlightedColumn == this)
            currentlyHighlightedColumn = null;
    }
}
