using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;


public class DialogueManager : MonoBehaviour
{
    public event Action OnDialogueEnd;
    private int lastPlayerChoice;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private List<Button> choiceButtons;
    [SerializeField] private Image npcImageHolder;
    [SerializeField] private Image backgroundImageHolder;

    [SerializeField] private Button continueIndicator;
    private Image indicatorImage;



    [Header("Background Images")]
    [SerializeField] private Sprite npcImageRound0;
    [SerializeField] private Sprite backgroundImageRound0;
    [SerializeField] private Sprite npcImageRound1;
    [SerializeField] private Sprite backgroundImageRound1;
    [SerializeField] private Sprite npcImageRound2;
    [SerializeField] private Sprite backgroundImageRound2;
    [SerializeField] private Sprite npcImageRound3;
    [SerializeField] private Sprite backgroundImageRound3;

    [Header("Button Images")]
    [SerializeField] private Sprite buttonImageNormal;
    [SerializeField] private Sprite buttonImageHighlighted;
    
    private bool waitingForClick = false;
    private float blinkTimer = 0f;
    private bool blinkDark = false;


    private DialogueData currentDialogue;
    private int currentRound = 0;
    private bool hasChosen = false;
    private Coroutine fallbackCoroutine;


    private void Start()
    {
        HideDialoguePanel();
        indicatorImage = continueIndicator.GetComponent<Image>();
        continueIndicator.gameObject.SetActive(false);

        continueIndicator.onClick.RemoveAllListeners();
        continueIndicator.onClick.AddListener(OnContinueClicked);
        npcCanvasGroup = npcImageHolder.GetComponent<CanvasGroup>();

    }


    private void Update()
    {
        if (waitingForClick && indicatorImage != null)
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= 0.5f)
            {
                blinkTimer = 0f;
                var color = indicatorImage.color;
                color.a = blinkDark ? 1f : 0.5f;
                indicatorImage.color = color;
                blinkDark = !blinkDark;
            }
        }
    }

    public DialogueData GetIntroDialogueForRound(int round)
    {
        switch (round)
        {
            case 0: //triceps (nie dziala, przeniesc do sceny po tutorialu)
                return new DialogueData
                {
                    npcLine = "No już, siadaj do stołu, przekonajmy się na kogo wychował Cię największy twardziel jakiego było mi dane poznać. Prawdziwą wojowniczkę można poznać tylko na polu bitwy. Może karty to nie to samo co dobra bitka wręcz, ale lepsze to niż gadanie o sandałach imperatora.",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };
            case 1: //flint
                return new DialogueData
                {
                    npcLine = "Hmmm, trzy razy sześć to będzie osiemnaście, mniej więcej tyle zapalniczek dziennie trafia na odrzut, a to wszystko z powodu jakiejś drobnej wady. Wyniosłem ich z fabryki już wystarczającą ilość, żeby zacząć pracować nad prototypem! Twój ojciec był zawsze sceptyczny w stosunku do moich wynalazków, ale w twoich oczach widzę… iskrę, iskrę która roznieci tutaj ogień. A jeśli chodzi o pożary to nie mogłaś trafić lepiej! Siadaj zagrajmy jak piroman z piromanem, hahaha!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "Śmierć twojego ojca jest wielką stratą, nie tylko dla waszego gatunku... Mądry był z niego towarzysz, a jak się z nim grało w deckbonsy! Wiem, że ciężko jest w to uwierzyć, w końcu jestem strażnikiem, ale za czasów mojej służby cieliśmy z Twoim starszym w karty jak równy z równym. Może moja kondycja nie jest jak dawniej, ale uwierz mi… mój umysł nadal pracuje. widzę co się dzieje. ta agresja… Wiem, że jesteście wściekli,, ale politycy… oni są bezwzględni. Życia tylu ludzi…niewolników, cywili, nie znaczą dla nich więcej niż ten piach (kopie w ziemię z rezygnacją)… Może jest jakieś pokojowe wyjście, które pomoże nam uniknąć masakry.  Jestem w stanie zaoferować swoją pomoc!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound2,
                    backgroundImage = backgroundImageRound2
                };

            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "Proszę proszę… Nowy przedstawiciel wielkiego wyzwolenia. Widzę, że jak na razie wasze irracjonalne plany się nie zmieniają. Ale cóż… nie każdy myśli o tych najsłabszych, których czeka największe niebezpieczeństwo… albo o tych, których nie do końca interesują losy waszej rasy. Ale do rzeczy. Twój ojciec twierdził, że ma dla mnie całkiem nie najgorszy układ. Mam nadzieję, że masz w sobie tyle rozumu co on i zaoferujesz mi coś co będzie korzystne nie tylko dla ciebie, ale i dla mnie. Może moja przychylność do was nie niesie za sobą niczego wielkiego, ale jej brak… Nie wiem czy ryzykowałabym go doświadczać.",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound3,
                    backgroundImage = backgroundImageRound3
                };


            default:
                return null;
        }
    }

    public void SetLastPlayerChoice(int choice) => lastPlayerChoice = choice;

    public int GetLastPlayerChoice() => lastPlayerChoice;

    public void HideDialoguePanel() => dialoguePanel.SetActive(false);

    public void ShowDialoguePanel() => dialoguePanel.SetActive(true);

    private CanvasGroup npcCanvasGroup;

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

        npcText.text = "";
        HideChoiceButtons();

        StartCoroutine(AnimateNpcAndType(dialogue));
    }

    private IEnumerator AnimateNpcAndType(DialogueData dialogue)
    {
        npcCanvasGroup.alpha = 0f;
        float duration = 0.6f;

        npcCanvasGroup.DOFade(1f, duration);
        yield return new WaitForSeconds(duration);

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(TypewriterEffect(dialogue.npcLine, dialogue));

        yield return new WaitForSeconds(0.1f);

        if (dialogue.playerChoices == null || dialogue.playerChoices.Length == 0)
        {
            yield return new WaitForSeconds(1f);
            EndDialogue();
        }
        else
        {
            ShowChoiceButtons();
        }
    }

    private List<string> SplitIntoSentences(string text)
    {
        var sentences = new List<string>();
        string[] parts = text.Split(new[] { ". ", "? ", "! " }, StringSplitOptions.None);

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                string trimmed = part.Trim();
                if (!trimmed.EndsWith(".") && !trimmed.EndsWith("?") && !trimmed.EndsWith("!"))
                    trimmed += ".";
                sentences.Add(trimmed);
            }
        }

        return sentences;
    }

    private Coroutine blinkCoroutine;
    private bool hasClickedContinue = false;


    private IEnumerator TypewriterEffect(string fullText, DialogueData dialogue)
    {
        npcText.text = "";
        List<string> sentences = SplitIntoSentences(fullText);

        for (int i = 0; i < sentences.Count; i++)
        {
            npcText.text = "";

            foreach (char c in sentences[i])
            {
                npcText.text += c;
                yield return new WaitForSeconds(0.03f);
            }

            bool isLastSentence = (i == sentences.Count - 1);
            continueIndicator.gameObject.SetActive(true);
            waitingForClick = true;
            blinkTimer = 0f;
            blinkDark = false;

            yield return new WaitUntil(() => hasClickedContinue);

            hasClickedContinue = false;
            waitingForClick = false;
            continueIndicator.gameObject.SetActive(false);

            if (isLastSentence)
            {
                yield break;
            }
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

        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);
        fallbackCoroutine = StartCoroutine(FallbackAutoEndDialogue());

    }

    private void OnButtonClick(Button button)
    {
        var image = button.GetComponent<Image>();
        if (image != null)
            image.sprite = buttonImageHighlighted;
    }

    public void OnContinueClicked()
    {
        if (!waitingForClick) return;

       
        hasClickedContinue = true;
        waitingForClick = false;
        continueIndicator.gameObject.SetActive(false);
        indicatorImage.color = new Color(1, 1, 1, 1);
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
        HideChoiceButtons(); 

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

        List<string> sentences = SplitIntoSentences(npcLine);

        for (int i = 0; i < sentences.Count; i++)
        {
            npcText.text = "";
            continueIndicator.gameObject.SetActive(false);

            foreach (char c in sentences[i])
            {
                npcText.text += c;
                yield return new WaitForSeconds(0.03f);
            }

            continueIndicator.gameObject.SetActive(true);
            waitingForClick = true;
            blinkTimer = 0f;
            blinkDark = false;

            yield return new WaitUntil(() => hasClickedContinue);

            hasClickedContinue = false;
            waitingForClick = false;
            continueIndicator.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        OnDialogueEnd?.Invoke(); 

    }




    private IEnumerator FallbackAutoEndDialogue()
    {
        yield return new WaitForSeconds(10f);
        if (!hasChosen)
        {
            Debug.LogWarning("Auto zakończono dialog po czasie");
            EndDialogue();
        }
    }
    public DialogueData GetDialogueForRound(int round)
    {
        currentRound = round;
        switch (round)
        {
            case 0: //triceps
                return new DialogueData
                {
                    npcLine = "Muszę przyznać, że masz łeb do taktyki, nic nie wpienia mnie tak jak przegrana, aż ma się ochotę krzyknąć IMPERATOR MA HALUKSA!!",
                    playerChoices = new[] { "Ucisz krzykiem", "Ucisz niepewnie" },
                    playerExpandedResponses = new[] {
                    "Triceps! Paszcza w kubeł, jak chcesz się wyżyć to zorganizuje ci pierwszy rząd w powstaniu, wtedy sklepiesz tyle blaszanych hełmów ile tylko zechcesz.",
                    "*Głos ze strachu załamuje ci się w połowie zdania* Panie Maximusie…. proszę niech Pan nie robi teraz hałasu."
                },
                    npcResponses = new[] {
                    "*gdy dociera do niego twój krzyk, okazuje niepewność* Wybacz szefowo… *przechodzi gwałtownie z krzyku w szept*. Oczywiście, że chcę walczyć w pierwszym rzędzie. Po prostu… bardzo jestem zdenerwowany, wkurzony i te inne… Przepraszam za bycie mało dyskretnym. Zajmę się przygotowaniami i ojej… pożałują wszystkiego co nam zrobili. ",
                    "Nie robić hałasu?! Nie można się bać tych skurczybyków! Trzeba im pokazać gdzie ich miejsce! Elity Rzymu upadną! I co najważniejsze: IMPERATOR MA HALUKSA!! *po krótkiej chwili widzisz, że w waszą stronę idzie patrol rzymskich strażników. Wiedząc co się świeci porzucasz rozmowę z Tricepsem i uciekasz ukradkiem*"
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };

            case 1: //flint
                return new DialogueData
                {
                    npcLine = "To była gorąca rozgrywka. No ale ale! Czy zdecydujesz się na moją propozycję? Gorąco zachęcam.",
                    playerChoices = new[] { "To zły pomysł", "To nas uratuje" },
                    playerExpandedResponses = new[] {
                    "Chyba muszę ostudzić twój zapał. Ten prototyp… on może być niebezpieczny również dla nas. Oczywiście, że każda pomoc się przyda, ale nie ryzykujmy życia naszych.",
                    "Musimy próbować z użyciem każdej siły. Rzymianie są bezwzględni. Może to ryzykowny pomysł, ale jeśli ten wynalazek okaże się skuteczny, może być znaczący dla naszego zwycięstwa."
                },
                    npcResponses = new[] {
                    "Cóż… trochę spodziewałem się, że możesz być jak twój poprzednik. Niedaleko pada jabłko od jabłoni, jak to mawiają. Jedyne co mogę powiedzieć - twoja strata. A raczej nasza. No ale co mogę zrobić. Widocznie nie sprzedałem swojego wynalazku wystarczająco dobrze. Ale jak możesz się spodziewać, mimo twojego braku zgody na jego użycie, oferuję swoją pomoc. ",
                    "Ha ha! Wiedziałem, że jesteś inna. Wyczułem to od razu. Zapewniam cię, że wygramy tą walkę, a mój gadżet się do tego przyczyni. Może nawet będę sławny. Rzymianie zapamiętają mnie jako ich największy koszmar. Jeśli któryś z nich wyjdzie z tego wszystkiego cało."
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "Całkiem nie najgorzej grasz, moja droga. Widzę, że twój ojciec dobrze nauczył cię taktyki, co pewnie udzieli się również na polu bitwy, jednak może posłuchasz mojej rady i wykorzystasz swoje zdolności w bardziej dyplomatyczny sposób. Błagam cię całym sobą, bo to jedyne co mogę teraz zrobić. Rzymianie też potrafią współczuć, musimy im tylko wskazać drogę, pokazać, że niewolnictwo to bestialstwo. A więc jaka jest twoja decyzja?",
                    playerChoices = new[] { "Wybieram siłę", "Wybieram pokój" },
                    playerExpandedResponses = new[] {
                    "Nasze życie się dla nich nie liczy. Jeśli powstanie ma wybuchnąć to musi być ono przeprowadzone z całą siłą, na jaką nas stać.",
                    "Może rzeczywiście uda nam się coś ugrać na drodze dyplomacji. Myślisz, że potrafiłbyś mi w tym pomóc?"
                },
                    npcResponses = new[] {
                    "Oh… czyli to postanowione, historia znów zatacza koło… (jego oczy wydają się puste, jakby powróciły do niego wszystkie dawne wspomnienia). (Fabius wstaje od stołu i przygląda się tobie). Wybacz, ale w takim wypadku nasze drogi muszą się tutaj rozejść. Życzę wam powodzenia, naprawdę. Ale nawet jeśli wszystko uda się wam na drodze agresji, to musisz wiedzieć, że takie wydarzenia zmieniają każdego. Niech Bogowie mają pod opieką wasze dusze… ",
                    "Tak! Na bogów, wiem dokładnie co robić! Audiencja. To jest to. Na szczęście masz do czynienia z byłym cenionym strażnikiem. Myślę, że uda mi się taką zorganizować. Zobaczysz, cywile też będą po naszej stronie. Przekonamy ich, no oczywiście nie wszystkich, ale to zawsze coś. "
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };
                
            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "No dobrze. Przyznam, że jestem w szoku. Pokonałaś w karty najbardziej przebiegłą mistrzynię. Ale cóż… zapewniam, że jedna przegrana mnie nie definiuje, poza tym… nie jesteśmy tu tak naprawdę dla kart, prawda? A więc pokaż mi co dla mnie masz.",
                    playerChoices = new[] { "Ofiaruj azyl", "Oferuj przepustkę" },
                    playerExpandedResponses = new[] {
                    "No dobrze. Skoro tak bardzo zależy ci na potencjalnych ofiarach, myślę że możemy poświęcić kilku walczących i zorganizować dla dzieci  i bezbronnych jakiś azyl.",
                    "Myślę, że mam coś, co mogłoby cię zainteresować… (wyciągnij przepustkę wyjścia z getta)."
                },
                    npcResponses = new[] {
                    "Oh, jakże dobroduszny gest. I to dopiero rozmowa ze mną sprawiła, że obudziło się wasze sumienie? Cóż… chyba będę musiała przyjąć taką ofertę, chociaż nie ukrywam, że liczyłam na coś zgoła innego. No ale… nie można mieć wszystkiego, tak? W takim razie pozwól, że na tym zakończymy. Może jeszcze zobaczymy się w niedalekiej przyszłości, chociaż mam ku temu spore wątpliwości. ",
                    "Hm… nieczyste zagranie.(uśmiecha się pod nosem) Ale cóż innego mi pozostało… Przynajmniej będę mogła zająć się w końcu problemami swoich ludzi. Zatem… powodzenia. Może jeśli wygracie bogowie jakimś cudem oczyszczą twoje sumienie. "
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
