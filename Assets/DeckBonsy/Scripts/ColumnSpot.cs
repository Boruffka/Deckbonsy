using UnityEngine;

public class ColumnSpot : MonoBehaviour
{
    [SerializeField] private bool isPlayerBoard;
    [SerializeField] private int columnIndex;

    public void SetIsPlayerBoard(bool _isPlayerBoard)
    {
        isPlayerBoard = _isPlayerBoard;
    }

    public void SetColumnIndex(int _columnIndex)
    {
        columnIndex = _columnIndex;
    }

    public void WhenClicked()
    {
        GameManager.gameManager.SetChosenColumnIndex(columnIndex, isPlayerBoard);
    }

    public void ClearColumn()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
