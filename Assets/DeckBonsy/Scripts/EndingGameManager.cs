using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingSceneManager : MonoBehaviour
{
    public TextMeshProUGUI endingText;

    void Start()
    {
        string ending = PlayerPrefs.GetString("ending", "neutral");

        switch (ending)
        {
            case "good":
                endingText.text = "Zako�czenie dobre: Twoje decyzje doprowadzi�y do wolno�ci.";
                break;
            case "neutral":
                endingText.text = "Zako�czenie neutralne: Nie wszystko posz�o po waszej my�li...";
                break;
            case "bad":
                endingText.text = "Zako�czenie z�e: Twoje decyzje doprowadzi�y do tragedii.";
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
