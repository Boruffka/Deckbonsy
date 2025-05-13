using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class JournalUpdateManager : MonoBehaviour
{
    public static JournalUpdateManager Instance;
    private Action journalClosedCallback;

    public GameObject journalPanel;
    public TextMeshProUGUI pageText;
    public Image pageImage;
    public List<Sprite> pageSprites;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowNoteAfterDialogue(int npcIndex, int playerChoice, Action onNoteComplete)
{
    Debug.Log($"🟡 ShowNoteAfterDialogue: npcIndex={npcIndex}, playerChoice={playerChoice}");

    string note = GetChoiceText(npcIndex, playerChoice);
    Debug.Log($"🟡 Note wygenerowany: {note}");

    if (string.IsNullOrEmpty(note) || npcIndex < 0 || npcIndex >= pageSprites.Count)
    {
        onNoteComplete?.Invoke(); 
        return;
    }

        JournalDataManager.Instance.AddOrUpdateNote(npcIndex, note);
        JournalDataManager.Instance.SaveJournalData();

        journalClosedCallback = onNoteComplete;
        journalPanel.SetActive(true);
        pageImage.sprite = pageSprites[npcIndex];

    if (typingCoroutine != null)
        StopCoroutine(typingCoroutine);

    typingCoroutine = StartCoroutine(TypeNote(note));
}


    private IEnumerator TypeNote(string note)
    {
        pageText.text = "";
        foreach (char c in note)
        {
            pageText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        //gracz sam zamyka dziennik
    }

    public void ClosePanel()
    {
        journalPanel.SetActive(false);

        if (journalClosedCallback != null)
        {
            var callback = journalClosedCallback;
            journalClosedCallback = null;
            callback.Invoke(); 
        }
    }

    public void OnJournalClosedByPlayer()
    {
        if (journalClosedCallback != null)
        {
            journalClosedCallback.Invoke();
            journalClosedCallback = null;
        }
    }


    public string GetChoiceText(int npcIndex, int playerChoice)
    {
        if (npcIndex == 0)
        {
            if (playerChoice == 0)
                return "Gracz powiedział: 'Halo!'.\nNPC odpowiedział: 'Cześć! Miło Cię widzieć.'";
            else
                return "Gracz powiedział: 'Nie teraz'.\nNPC odpowiedział: 'Rozumiem. Może później?'.";
        }
        else if (npcIndex == 1)
        {
            if (playerChoice == 0)
                return "Gracz powiedział: 'Siema!'.\nNPC odpowiedział: 'Yo, stary znajomy!'.";
            else
                return "Gracz powiedział: 'Nie dzisiaj'.\nNPC odpowiedział: 'Szkoda...'.";
        }

        return $"Gracz wybrał opcję {playerChoice + 1} z NPC {npcIndex}.";
    }
}
