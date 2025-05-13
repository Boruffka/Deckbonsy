using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public event Action OnDialogueEnd;
    private int lastPlayerChoice;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private List<Button> choiceButtons;
    [SerializeField] private Image npcImageHolder;
    [SerializeField] private Image backgroundImageHolder;

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
    private bool hasChosen = false;
    private Coroutine fallbackCoroutine;

    private void Start()
    {
        HideDialoguePanel();
    }

    public DialogueData GetIntroDialogueForRound(int round)
    {
        switch (round)
        {
            case 0:
                return new DialogueData
                {
                    npcLine = "NPC1 Witaj",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };
            case 1:
                return new DialogueData
                {
                    npcLine = "NPC2 Witaj",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };
            default:
                return null;
        }
    }

    public void SetLastPlayerChoice(int choice) => lastPlayerChoice = choice;

    public int GetLastPlayerChoice() => lastPlayerChoice;

    public void HideDialoguePanel() => dialoguePanel.SetActive(false);

    public void ShowDialoguePanel() => dialoguePanel.SetActive(true);

    public void StartDialogue(DialogueData dialogue)
    {
        hasChosen = false;
        ShowDialoguePanel();
        currentDialogue = dialogue;

        if (dialogue == null)
        {
            Debug.LogWarning("Dialogue is null!");
            return;
        }

        if (dialogue.backgroundImage != null)
            backgroundImageHolder.sprite = dialogue.backgroundImage;

        if (dialogue.npcImage != null)
            npcImageHolder.sprite = dialogue.npcImage;

        npcImageHolder.rectTransform.anchoredPosition = new Vector2(-500f, 0);
        npcText.text = "";
        HideChoiceButtons();

        StartCoroutine(AnimateNpcAndType(dialogue));
    }

    private IEnumerator AnimateNpcAndType(DialogueData dialogue)
    {
        Vector2 finalPos = new Vector2(196f, -96f);
        Vector2 startPos = new Vector2(finalPos.x - 600f, finalPos.y);

        npcImageHolder.rectTransform.anchoredPosition = startPos;

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            npcImageHolder.rectTransform.anchoredPosition = Vector2.Lerp(startPos, finalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        npcImageHolder.rectTransform.anchoredPosition = finalPos;

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(TypewriterEffect(dialogue.npcLine));

        if (dialogue.playerChoices == null || dialogue.playerChoices.Length == 0)
        {
            yield return new WaitForSeconds(1f);
            EndDialogue();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            ShowChoiceButtons();
        }
    }

    private IEnumerator TypewriterEffect(string line)
    {
        npcText.text = "";
        foreach (char c in line)
        {
            npcText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void HideChoiceButtons()
    {
        foreach (var button in choiceButtons)
            button.gameObject.SetActive(false);
    }

    private void ShowChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < currentDialogue.playerChoices.Length)
            {
                var button = choiceButtons[i];
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.playerChoices[i];

                var image = button.GetComponent<Image>();
                image.sprite = buttonImageNormal;

                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnChoiceSelected(index));
                button.onClick.AddListener(() => OnButtonClick(button));

                AddHoverEffect(button);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        // START fallback
        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);
        fallbackCoroutine = StartCoroutine(FallbackAutoEndDialogue());
    }

    private void OnButtonClick(Button button)
    {
        var image = button.GetComponent<Image>();
        if (image != null)
            image.sprite = buttonImageHighlighted;
    }

    private void AddHoverEffect(Button button)
    {
        var trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((_) => button.transform.localScale = Vector3.one * 0.95f);
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((_) => button.transform.localScale = Vector3.one);
        trigger.triggers.Add(entryExit);
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        if (hasChosen) return;
        hasChosen = true;

        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);

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

    private IEnumerator FallbackAutoEndDialogue()
    {
        yield return new WaitForSeconds(10f);
        if (!hasChosen)
        {
            Debug.LogWarning("Auto zakoñczono dialog po czasie");
            EndDialogue();
        }
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
                    playerChoices = new[] { "halo", "nie teraz" },
                    playerExpandedResponses = new[] {
                    "Tak naprawdê chcia³am tylko siê przywitaæ.",
                    "Nie mam teraz czasu na rozmowê."
                },
                    npcResponses = new[] {
                    "Mi³o Ciê poznaæ!",
                    "No trudno, mo¿e innym razem."
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };

            case 1:
                return new DialogueData
                {
                    npcLine = "siema nie jestem triceps",
                    playerChoices = new[] { "siema", "nie" },
                    playerExpandedResponses = new[] {
                    "noooooooooooooooooo",
                    "Nie mam teraz czasu na rozmowê."
                },
                    npcResponses = new[] {
                    "Mi³o Ciê poznaæ!",
                    "No trudno, mo¿e innym razem."
                },
                    endings = new[] { 0, 1 },
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
