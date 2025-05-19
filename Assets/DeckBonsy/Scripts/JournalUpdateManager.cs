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
        if (npcIndex == 0) // triceps
        {
            if (playerChoice == 0)
                return "Ten Triceps to niezły kretyn. Zaczął się wydzierać przy strażnikach o powstaniu. Na szczęście przynajmniej potrafi słuchać i szybko go uciszyłam. Nie wiem czy z kimś takim jest sens iść na powstanie, ale wiem, że jest w to bardzo zaangażowany.";
            else
                return "Ten Triceps to niezły kretyn. Zaczął się wydzierać przy strażnikach o powstaniu. Spanikowałam i zaczęłam go uciszać ale to jeszcze bardziej go rozjuszyło i krzyczał coś o haluksach. Czym jest do cholery ten haluks? Oczywiście, że zwróciło to uwagę wszystkich, w tym strażników. Uciekłam, bo co miałam zrobić? Nie wiem nawet co się z tym kretynem stało, mam nadzieję, że go nie złapali...";
        }
        else if (npcIndex == 1) //flint
        {
            if (playerChoice == 0)
                return "Flint ofiarował mi ten swój wynalazek, ale wydaje mi się to zbyt ryzykowne. To dobry koleś, ale troche szurnięty. Trochę się zdenerwował jak mu to powiedziałam, mam nadzieję, że nie zrobi nic głupiego.";
            else
                return "Flint jest nieźle szurnięty ale takich nam właśnie potrzeba. On wie co robi, mój ojciec nie wierzył w jego wynalazki, ale dla powstania warto spróbować.";
        }
        else if (npcIndex == 2) //fabius
        {
            if (playerChoice == 0)
                return "Fabius jest trochę dziwny, ale był przyjacielem mojego ojca, więc musiałam mu zaufać. Nie chce jednak słuchać rad byłego eliciarza. On coś mówi o pokoju, ale to my codziennie umieramy z rąk ludzi. To nas wykorzystują. Gdyby zależało im na pokoju to by to dawno wszystko przerwali. Naszą wolność musimy odebrać siłą jaką nam odebrano. Tak by chciał ojciec.";
            else
                return "Nie wiem, czy to dobry pomysł, ale może nie warto wszystkiego załatwiać siłą? Może kiedy wyciągają do nas dłoń powinniśmy ją przyjąć. Fabius podobno jest cenionym przemówcą, może ta gadka o pokoju przekona więcej ludzi na naszą stronę. Ale powstania już nie powstrzymamy.";
        }
        else if (npcIndex == 3) //minerva
        {
            if (playerChoice == 0)
                return "Ona jest bardzo przebiegła. Każde słowo waży i ciągle mam wrażenie, że coś knuje. Zaproponowałam azyl dla niej i ofiar, ale dziwnie się zachowywała. Jakby oczekiwała czegoś więcej, ale nie wiem czego. Mam mieszane uczucia, mam nadzieję, że tym razem intuicja mnie oszukuje.";
            else
                return "W dzienniku ojcec zostawił przepustkę, która podobno pozwala niewolnikowi uciec z tego miejsca. Myślałam o tym długo, ale...współczuję jej. To nawet nie jej wojna. Chciałabym, żebyśmy mogli coś zmienić dla wszystkich ras niewolników, ale jedno powstanie tego nie załatwi. Mam nadzieję, że jej przebiegłość pomoże zmienić prawo na jej planecie";
        }

        return $"Gracz wybrał opcję {playerChoice + 1} z NPC {npcIndex}.";
    }
}
