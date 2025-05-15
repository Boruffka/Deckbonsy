using UnityEngine;
using System.Collections;

public class IntroStarter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject startGameButton;



    private bool introEnded = false;
    private bool waitingForPlayerInput = false;

    private void Start()
    {
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
            Debug.LogWarning("⚠️ Brak intro rundy 0!");
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
        Debug.Log("✅ Intro zakończone — czekam na input gracza (SPACJA)...");
    }

    private void Update()
    {
        if (!waitingForPlayerInput) return;

        // Opcja 1: Start po naciśnięciu spacji
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        // Opcja 2 (fallback): automatycznie po 5 sekundach
        if (introEnded)
        {
            StartCoroutine(AutoStartAfterDelay(5f));
            introEnded = false; // zabezpieczenie, żeby tylko raz
        }
    }

    private IEnumerator AutoStartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (waitingForPlayerInput)
        {
            Debug.Log("⏱ Minęło 5 sek — startuję grę automatycznie.");
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

        Debug.Log("🎮 Gra rusza przez przycisk lub auto!");
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
