using System.Collections.Generic;
using UnityEngine;

public class JournalDataManager : MonoBehaviour
{
    public static JournalDataManager Instance;

    private Dictionary<int, string> dynamicTexts = new Dictionary<int, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadJournalData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetTextForPage(int pageIndex)
    {
        return dynamicTexts.ContainsKey(pageIndex) ? dynamicTexts[pageIndex] : string.Empty;
    }

    public void AddOrUpdateNote(int pageIndex, string note, bool overwrite = false)
    {
        if (overwrite)
        {
            dynamicTexts[pageIndex] = note;
        }
        else
        {
            if (!dynamicTexts.ContainsKey(pageIndex) || !dynamicTexts[pageIndex].Contains(note))
            {
                dynamicTexts[pageIndex] = dynamicTexts.ContainsKey(pageIndex)
                    ? dynamicTexts[pageIndex] + "\n" + note
                    : note;
            }
        }

        SaveJournalData();
    }




    public void SaveJournalData()
    {
        foreach (var entry in dynamicTexts)
        {
            PlayerPrefs.SetString("JournalPage_" + entry.Key, entry.Value);
        }
        PlayerPrefs.Save();
    }

    private void LoadJournalData()
    {
        for (int i = 0; i < 20; i++)
        {
            if (i == 0) continue; 

            string key = "JournalPage_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                dynamicTexts[i] = PlayerPrefs.GetString(key);
            }
        }
    }


    public bool HasContent(int pageIndex)
    {
        return dynamicTexts.ContainsKey(pageIndex);
    }
    [ContextMenu("Wyczyœæ wszystkie zapisy dziennika")]
    public void ClearAllJournalData()
    {
        for (int i = 0; i < 20; i++)
        {
            string key = "JournalPage_" + i;
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        Debug.Log("Wyczyszczono dane dziennika.");
    }

    [ContextMenu("Resetuj dziennik")]
    public void ResetJournalManually()
    {
        for (int i = 0; i < 20; i++)
            PlayerPrefs.DeleteKey("JournalPage_" + i);

        PlayerPrefs.DeleteKey("JournalClearedOnce");
        PlayerPrefs.Save();

        Debug.Log("Rêczne czyszczenie zakoñczone.");
    }

}
