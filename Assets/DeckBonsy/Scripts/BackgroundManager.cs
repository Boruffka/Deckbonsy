using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite playerTurnSprite;
    [SerializeField] private Sprite enemyTurnSprite;

    public void SetPlayerTurnBackground()
    {
        backgroundImage.sprite = playerTurnSprite;
    }

    public void SetEnemyTurnBackground()
    {
        backgroundImage.sprite = enemyTurnSprite;
    }
}
