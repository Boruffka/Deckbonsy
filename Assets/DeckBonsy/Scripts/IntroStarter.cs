using UnityEngine;
using System.Collections;

public class IntroStarter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject journalButton;




    private bool introEnded = false;
    private bool waitingForPlayerInput = false;

    private void Start()
    {
        if (GameManager.gameManager.CurrentRound != 0)
        {
            this.enabled = false;
            return;
        }

        StartCoroutine(DelayedIntro());
    }


    private IEnumerator DelayedIntro()
    {
        yield return null;

        if (dialogueManager == null || gameManager == null)
        {
            Debug.LogError("❌ DialogueManager lub GameManager nie przypisany!");
            yield break;
        }

        var intro = dialogueManager.GetIntroDialogueForRound(0);
        if (intro == null)
        {
            yield break;
        }

        SetGameReady(false);
        dialogueManager.StartDialogue(intro);
        dialogueManager.OnDialogueEnd += OnIntroEnded;
    }

    private void OnIntroEnded()
    {
        dialogueManager.OnDialogueEnd -= OnIntroEnded;
        introEnded = true;
        waitingForPlayerInput = true;
        //StartGame();
    }

    private void Update()
    {
        if (!waitingForPlayerInput) return;

      // (fallback) automatycznie po 1.5 sekundach
        if (introEnded)
        {
            StartCoroutine(AutoStartAfterDelay(1f));
            introEnded = false; // zabezpieczenie, żeby tylko raz
        }
    }

    private IEnumerator AutoStartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (waitingForPlayerInput)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false); // zamknij dialog

        SetGameReady(false);
        SetStartCardGameAfterIntro(true);
        SetCurrentRound(0);
        if (startGameButton != null)
            startGameButton.SetActive(false);

        if (journalButton != null)
        {
            journalButton.SetActive(true);

            var display = FindFirstObjectByType<JournalDisplayManager>();
            if (display != null && gameManager.CurrentRound == 0)
            {
                display.SetMaxPageIndex(0); // zablokuj inne strony
            }

            journalButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                var display = FindFirstObjectByType<JournalDisplayManager>();
                if (display != null)
                {
                    display.OpenJournal(0); // otwórz stronę 0
                }
            });
        }

    }


    private void SetGameReady(bool value)
    {
        typeof(GameManager).GetField("gameReady", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                           ?.SetValue(gameManager, value);
    }

    private void SetStartCardGameAfterIntro(bool value)
    {
        typeof(GameManager).GetField("startCardGameAfterIntro", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                           ?.SetValue(gameManager, value);
    }

    private void SetCurrentRound(int round)
    {
        typeof(GameManager).GetField("currentRound", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                           ?.SetValue(gameManager, round);
    }
}
