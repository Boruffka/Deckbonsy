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
                endingText.text = "Zakoñczenie dobre: Twoje decyzje doprowadzi³y do wolnoœci.";
                break;
            case "neutral":
                endingText.text = "Zakoñczenie neutralne: Nie wszystko posz³o po waszej myœli...";
                break;
            case "bad":
                endingText.text = "Zakoñczenie z³e: Twoje decyzje doprowadzi³y do tragedii.";
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
