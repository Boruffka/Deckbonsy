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
        string note = GetChoiceText(npcIndex, playerChoice);

        if (string.IsNullOrEmpty(note) || npcIndex < 0 || npcIndex >= pageSprites.Count)
        {
            Debug.LogWarning("B��dny indeks NPC lub pusty tekst!");
            onNoteComplete?.Invoke(); // dalej kontynuujemy
            return;
        }

        JournalDataManager.Instance.AddOrUpdateNote(npcIndex, note);

        journalClosedCallback = onNoteComplete; // <-- ZAPAMI�TAJ CALLBACK

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
                return "Gracz powiedzia�: 'Halo!'.\nNPC odpowiedzia�: 'Cze��! Mi�o Ci� widzie�.'";
            else
                return "Gracz powiedzia�: 'Nie teraz'.\nNPC odpowiedzia�: 'Rozumiem. Mo�e p�niej?'.";
        }
        else if (npcIndex == 1)
        {
            if (playerChoice == 0)
                return "Gracz powiedzia�: 'Siema!'.\nNPC odpowiedzia�: 'Yo, stary znajomy!'.";
            else
                return "Gracz powiedzia�: 'Nie dzisiaj'.\nNPC odpowiedzia�: 'Szkoda...'.";
        }

        return $"Gracz wybra� opcj� {playerChoice + 1} z NPC {npcIndex}.";
    }
}
