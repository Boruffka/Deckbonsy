using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class JournalDisplayManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject journalPanel;
    public Image pageImage;
    public TextMeshProUGUI pageText;
    public Button nextPageButton;
    public Button previousPageButton;
    public Button closeButton;

    [Header("Arrow Buttons")]
    [SerializeField] private Sprite leftArrowNormal;
    [SerializeField] private Sprite leftArrowHighlighted;
    [SerializeField] private Sprite rightArrowNormal;
    [SerializeField] private Sprite rightArrowHighlighted;

    [Header("Scene Button")]
    [SerializeField] private Button journalSceneButton;
    [SerializeField] private Sprite journalNormal;
    [SerializeField] private Sprite journalHighlighted;

    [Header("Page Sprites")]
    public List<Sprite> pageSprites;

    private int currentPage = 0;

    private void Start()
    {
        {
            var triggers = GetComponents<EventTrigger>();
            foreach (var t in triggers)
            {
                Destroy(t);
            }
        }

        if (!journalPanel.activeSelf)
        {
            journalPanel.SetActive(false);
        }

        if (nextPageButton) nextPageButton.onClick.AddListener(NextPage);
        if (previousPageButton) previousPageButton.onClick.AddListener(PreviousPage);
        if (closeButton) closeButton.onClick.AddListener(CloseJournal);

        if (nextPageButton) AddHoverEffect(nextPageButton, rightArrowNormal, rightArrowHighlighted);
        if (previousPageButton) AddHoverEffect(previousPageButton, leftArrowNormal, leftArrowHighlighted);

        if (journalSceneButton) AddHoverEffect(journalSceneButton, journalNormal, journalHighlighted);
    }

    private int maxPageIndex = int.MaxValue; // domyślnie bez ograniczenia

    public void SetMaxPageIndex(int maxIndex)
    {
        maxPageIndex = Mathf.Clamp(maxIndex, 0, pageSprites.Count - 1);
    }

    private void AddHoverEffect(Button button, Sprite normal, Sprite highlighted)
    {
        var image = button.GetComponent<Image>();
        if (image == null) return;

        var trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((_) => image.sprite = highlighted);
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((_) => image.sprite = normal);
        trigger.triggers.Add(entryExit);
    }

    public void OpenJournal(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageSprites.Count)
        {
            Debug.LogWarning("Nieprawidłowy indeks strony: " + pageIndex);
            return;
        }

        currentPage = pageIndex;
        journalPanel.SetActive(true);
        UpdateJournalPage();
    }

    private void CloseJournal()
    {
        journalPanel.SetActive(false);

        var manager = JournalUpdateManager.Instance;
        if (manager != null)
        {
            manager.OnJournalClosedByPlayer(); // wywołuje callback z GameManagera
        }
    }

    private void NextPage()
    {
        if (currentPage < maxPageIndex)
        {
            currentPage++;
            UpdateJournalPage();
        }
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateJournalPage();
        }
    }


    private void UpdateJournalPage()
    {
        if (currentPage >= 0 && currentPage < pageSprites.Count)
        {
            pageImage.sprite = pageSprites[currentPage];

            string text = JournalDataManager.Instance.GetTextForPage(currentPage);
            pageText.text = string.IsNullOrWhiteSpace(text) ? "<i>Brak zapisu dla tej strony</i>" : text;
        }
    }
}
