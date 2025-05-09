using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public event Action OnDialogueEnd;
    private int lastPlayerChoice;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private List<UnityEngine.UI.Button> choiceButtons;
    [SerializeField] private UnityEngine.UI.Image npcImageHolder;
    [SerializeField] private UnityEngine.UI.Image backgroundImageHolder;

    [Header("Background Images")]
    [SerializeField] private Sprite npcImageRound0;
    [SerializeField] private Sprite backgroundImageRound0;
    [SerializeField] private Sprite npcImageRound1;
    [SerializeField] private Sprite backgroundImageRound1;

    [Header("Button Images")]
    [SerializeField] private Sprite buttonImageNormal;
    [SerializeField] private Sprite buttonImageHighlighted;

    private DialogueData currentDialogue;
    private int currentRound = 0;

    private void Start()
    {
        HideDialoguePanel();
    }

    public void SetLastPlayerChoice(int choice)
    {
        lastPlayerChoice = choice;
    }
    public int GetLastPlayerChoice()
    {
        return lastPlayerChoice;
    }

    public void HideDialoguePanel()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialoguePanel()
    {
        dialoguePanel.SetActive(true);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        ShowDialoguePanel();
        currentDialogue = dialogue;
        npcText.text = dialogue.npcLine;

        if (dialogue.backgroundImage != null)
            backgroundImageHolder.sprite = dialogue.backgroundImage;

        if (dialogue.npcImage != null)
            npcImageHolder.sprite = dialogue.npcImage;

        HideChoiceButtons();

        StartCoroutine(ShowChoicesAfterNpcSpeech(dialogue.npcLine));
    }

    private void HideChoiceButtons()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowChoicesAfterNpcSpeech(string npcSpeech)
    {
        yield return new WaitForSeconds(npcSpeech.Length * 0.1f);
        ShowChoiceButtons();
    }

    private void ShowChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < currentDialogue.playerChoices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.playerChoices[i];

                choiceButtons[i].GetComponent<UnityEngine.UI.Image>().sprite = buttonImageNormal;

                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));

                UnityEngine.UI.Button button = choiceButtons[i];
                button.onClick.AddListener(() => OnButtonClick(button));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnButtonClick(UnityEngine.UI.Button button)
    {
        UnityEngine.UI.Image buttonImage = button.GetComponent<UnityEngine.UI.Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = buttonImageHighlighted;
        }
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        SetLastPlayerChoice(choiceIndex);

        string playerResponse = currentDialogue.playerExpandedResponses[choiceIndex];
        string npcResponse = currentDialogue.npcResponses[choiceIndex];

        StartCoroutine(PlayChoiceSequence(playerResponse, npcResponse));
    }

    private IEnumerator PlayChoiceSequence(string playerLine, string npcLine)
    {
        npcText.text = "";
        npcText.color = Color.cyan;

        foreach (char c in playerLine)
        {
            npcText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(1f);

        npcText.text = "";
        npcText.color = Color.white;

        foreach (char c in npcLine)
        {
            npcText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(1f);

        EndDialogue();
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public DialogueData GetDialogueForRound(int round)
    {
        currentRound = round;
        switch (round)
        {
            case 0:
                return new DialogueData
                {
                    npcLine = "siema jestem triceps",
                    playerChoices = new string[] { "halo", "nie teraz" },
                    playerExpandedResponses = new string[]
                    {
                        "Tak naprawd� chcia�am tylko si� przywita�.",
                        "Nie mam teraz czasu na rozmow�."
                    },
                    npcResponses = new string[]
                    {
                        "Mi�o Ci� pozna�!",
                        "No trudno, mo�e innym razem."
                    },
                    endings = new int[] { 0, 1 },
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };


            case 1:
                return new DialogueData
                {
                    npcLine = "siema nie jestem triceps",
                    playerChoices = new string[] { "siema", "nie" },
                    playerExpandedResponses = new string[]
                    {
                        "noooooooooooooooooo",
                        "Nie mam teraz czasu na rozmow�."
                    },
                    npcResponses = new string[]
                    {
                        "Mi�o Ci� pozna�!",
                        "No trudno, mo�e innym razem."
                    },
                    endings = new int[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };
            default:
                return null;
        }
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
    }
}
