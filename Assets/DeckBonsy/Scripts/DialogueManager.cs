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
                    npcLine = "No ju�, siadaj do sto�u, przekonajmy si� na kogo wychowa� Ci� najwi�kszy twardziel jakiego by�o mi dane pozna�. Prawdziw� wojowniczk� mo�na pozna� tylko na polu bitwy. Mo�e karty to nie to samo co dobra bitka wr�cz, ale lepsze to ni� gadanie o sanda�ach imperatora.",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };
            case 1: //flint
                return new DialogueData
                {
                    npcLine = "Hmmm, trzy razy sze�� to b�dzie osiemna�cie, mniej wi�cej tyle zapalniczek dziennie trafia na odrzut, a to wszystko z powodu jakiej� drobnej wady. Wynios�em ich z fabryki ju� wystarczaj�c� ilo��, �eby zacz�� pracowa� nad prototypem! Tw�j ojciec by� zawsze sceptyczny w stosunku do moich wynalazk�w, ale w twoich oczach widz� iskr�, iskr� kt�ra roznieci tutaj ogie�. A je�li chodzi o po�ary to nie mog�a� trafi� lepiej! Siadaj zagrajmy jak piroman z piromanem, hahaha!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "�mier� twojego ojca jest wielk� strat�, nie tylko dla waszego gatunku... M�dry by� z niego towarzysz, a jak si� z nim gra�o w deckbonsy! Wiem, �e ci�ko jest w to uwierzy�, w ko�cu jestem stra�nikiem, ale za czas�w mojej s�u�by cieli�my z Twoim starszym w karty jak r�wny z r�wnym. Mo�e moja kondycja nie jest jak dawniej, ale uwierz mi� m�j umys� nadal pracuje. widz� co si� dzieje. ta agresja� Wiem, �e jeste�cie w�ciekli,, ale politycy� oni s� bezwzgl�dni. �ycia tylu ludzi�niewolnik�w, cywili, nie znacz� dla nich wi�cej ni� ten piach (kopie w ziemi� z rezygnacj�)� Mo�e jest jakie� pokojowe wyj�cie, kt�re pomo�e nam unikn�� masakry.  Jestem w stanie zaoferowa� swoj� pomoc!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "Prosz� prosz� Nowy przedstawiciel wielkiego wyzwolenia. Widz�, �e jak na razie wasze irracjonalne plany si� nie zmieniaj�. Ale c� nie ka�dy my�li o tych najs�abszych, kt�rych czeka najwi�ksze niebezpiecze�stwo� albo o tych, kt�rych nie do ko�ca interesuj� losy waszej rasy. Ale do rzeczy. Tw�j ojciec twierdzi�, �e ma dla mnie ca�kiem nie najgorszy uk�ad. Mam nadziej�, �e masz w sobie tyle rozumu co on i zaoferujesz mi co� co b�dzie korzystne nie tylko dla ciebie, ale i dla mnie. Mo�e moja przychylno�� do was nie niesie za sob� niczego wielkiego, ale jej brak� Nie wiem czy ryzykowa�abym go do�wiadcza�.",
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


    private IEnumerator TypewriterEffect(string fullText)
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

            continueIndicator.gameObject.SetActive(true);
            waitingForClick = true;
            blinkTimer = 0f;
            blinkDark = false;

            yield return new WaitUntil(() =>
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetMouseButtonDown(0) ||
                hasClickedContinue
            );

            hasClickedContinue = false;
            waitingForClick = false;
            continueIndicator.gameObject.SetActive(false);

            // je�li to ostatnie zdanie � ko�czymy dialog
            if (i == sentences.Count - 1)
            {
                EndDialogue();
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

    public void OnContinueClicked()
    {
        if (!waitingForClick) return;

        waitingForClick = false;
        continueIndicator.gameObject.SetActive(false);
        indicatorImage.color = new Color(1, 1, 1, 1);

        hasClickedContinue = true; 
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
            Debug.LogWarning("Auto zako�czono dialog po czasie");
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
                    npcLine = "Musz� przyzna�, �e masz �eb do taktyki, nic nie wpienia mnie tak jak przegrana, a� ma si� ochot� krzykn�� IMPERATOR MA HALUKSA!!",
                    playerChoices = new[] { "Ucisz krzykiem", "Ucisz niepewnie" },
                    playerExpandedResponses = new[] {
                    "Triceps! Paszcza w kube�, jak chcesz si� wy�y� to zorganizuje ci pierwszy rz�d w powstaniu, wtedy sklepiesz tyle blaszanych he�m�w ile tylko zechcesz.",
                    "*G�os ze strachu za�amuje ci si� w po�owie zdania* Panie Maximusie�. prosz� niech Pan nie robi teraz ha�asu."
                },
                    npcResponses = new[] {
                    "*gdy dociera do niego tw�j krzyk, okazuje niepewno��* Wybacz szefowo� *przechodzi gwa�townie z krzyku w szept*. Oczywi�cie, �e chc� walczy� w pierwszym rz�dzie. Po prostu� bardzo jestem zdenerwowany, wkurzony i te inne� Przepraszam za bycie ma�o dyskretnym. Zajm� si� przygotowaniami i ojej� po�a�uj� wszystkiego co nam zrobili. ",
                    "Nie robi� ha�asu?! Nie mo�na si� ba� tych skurczybyk�w! Trzeba im pokaza� gdzie ich miejsce! Elity Rzymu upadn�! I co najwa�niejsze: IMPERATOR MA HALUKSA!! *po kr�tkiej chwili widzisz, �e w wasz� stron� idzie patrol rzymskich stra�nik�w. Wiedz�c co si� �wieci porzucasz rozmow� z Tricepsem i uciekasz ukradkiem*"
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };

            case 1: //flint
                return new DialogueData
                {
                    npcLine = "To by�a gor�ca rozgrywka. No ale ale! Czy zdecydujesz si� na moj� propozycj�? Gor�co zach�cam.",
                    playerChoices = new[] { "To z�y pomys�", "To nas uratuje" },
                    playerExpandedResponses = new[] {
                    "Chyba musz� ostudzi� tw�j zapa�. Ten prototyp� on mo�e by� niebezpieczny r�wnie� dla nas. Oczywi�cie, �e ka�da pomoc si� przyda, ale nie ryzykujmy �ycia naszych.",
                    "Musimy pr�bowa� z u�yciem ka�dej si�y. Rzymianie s� bezwzgl�dni. Mo�e to ryzykowny pomys�, ale je�li ten wynalazek oka�e si� skuteczny, mo�e by� znacz�cy dla naszego zwyci�stwa."
                },
                    npcResponses = new[] {
                    "C� troch� spodziewa�em si�, �e mo�esz by� jak tw�j poprzednik. Niedaleko pada jab�ko od jab�oni, jak to mawiaj�. Jedyne co mog� powiedzie� - twoja strata. A raczej nasza. No ale co mog� zrobi�. Widocznie nie sprzeda�em swojego wynalazku wystarczaj�co dobrze. Ale jak mo�esz si� spodziewa�, mimo twojego braku zgody na jego u�ycie, oferuj� swoj� pomoc. ",
                    "Ha ha! Wiedzia�em, �e jeste� inna. Wyczu�em to od razu. Zapewniam ci�, �e wygramy t� walk�, a m�j gad�et si� do tego przyczyni. Mo�e nawet b�d� s�awny. Rzymianie zapami�taj� mnie jako ich najwi�kszy koszmar. Je�li kt�ry� z nich wyjdzie z tego wszystkiego ca�o."
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "Ca�kiem nie najgorzej grasz, moja droga. Widz�, �e tw�j ojciec dobrze nauczy� ci� taktyki, co pewnie udzieli si� r�wnie� na polu bitwy, jednak mo�e pos�uchasz mojej rady i wykorzystasz swoje zdolno�ci w bardziej dyplomatyczny spos�b. B�agam ci� ca�ym sob�, bo to jedyne co mog� teraz zrobi�. Rzymianie te� potrafi� wsp�czu�, musimy im tylko wskaza� drog�, pokaza�, �e niewolnictwo to bestialstwo. A wi�c jaka jest twoja decyzja?",
                    playerChoices = new[] { "Wybieram si��", "Wybieram pok�j" },
                    playerExpandedResponses = new[] {
                    "Nasze �ycie si� dla nich nie liczy. Je�li powstanie ma wybuchn�� to musi by� ono przeprowadzone z ca�� si��, na jak� nas sta�.",
                    "Mo�e rzeczywi�cie uda nam si� co� ugra� na drodze dyplomacji. My�lisz, �e potrafi�by� mi w tym pom�c?"
                },
                    npcResponses = new[] {
                    "Oh� czyli to postanowione, historia zn�w zatacza ko�o� (jego oczy wydaj� si� puste, jakby powr�ci�y do niego wszystkie dawne wspomnienia). (Fabius wstaje od sto�u i przygl�da si� tobie). Wybacz, ale w takim wypadku nasze drogi musz� si� tutaj rozej��. �ycz� wam powodzenia, naprawd�. Ale nawet je�li wszystko uda si� wam na drodze agresji, to musisz wiedzie�, �e takie wydarzenia zmieniaj� ka�dego. Niech Bogowie maj� pod opiek� wasze dusze� ",
                    "Tak! Na bog�w, wiem dok�adnie co robi�! Audiencja. To jest to. Na szcz�cie masz do czynienia z by�ym cenionym stra�nikiem. My�l�, �e uda mi si� tak� zorganizowa�. Zobaczysz, cywile te� b�d� po naszej stronie. Przekonamy ich, no oczywi�cie nie wszystkich, ale to zawsze co�. "
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };
                
            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "No dobrze. Przyznam, �e jestem w szoku. Pokona�a� w karty najbardziej przebieg�� mistrzyni�. Ale c� zapewniam, �e jedna przegrana mnie nie definiuje, poza tym� nie jeste�my tu tak naprawd� dla kart, prawda? A wi�c poka� mi co dla mnie masz.",
                    playerChoices = new[] { "Ofiaruj azyl", "Oferuj przepustk�" },
                    playerExpandedResponses = new[] {
                    "No dobrze. Skoro tak bardzo zale�y ci na potencjalnych ofiarach, my�l� �e mo�emy po�wi�ci� kilku walcz�cych i zorganizowa� dla dzieci  i bezbronnych jaki� azyl.",
                    "My�l�, �e mam co�, co mog�oby ci� zainteresowa� (wyci�gnij przepustk� wyj�cia z getta)."
                },
                    npcResponses = new[] {
                    "Oh, jak�e dobroduszny gest. I to dopiero rozmowa ze mn� sprawi�a, �e obudzi�o si� wasze sumienie? C� chyba b�d� musia�a przyj�� tak� ofert�, chocia� nie ukrywam, �e liczy�am na co� zgo�a innego. No ale� nie mo�na mie� wszystkiego, tak? W takim razie pozw�l, �e na tym zako�czymy. Mo�e jeszcze zobaczymy si� w niedalekiej przysz�o�ci, chocia� mam ku temu spore w�tpliwo�ci. ",
                    "Hm� nieczyste zagranie.(u�miecha si� pod nosem) Ale c� innego mi pozosta�o� Przynajmniej b�d� mog�a zaj�� si� w ko�cu problemami swoich ludzi. Zatem� powodzenia. Mo�e je�li wygracie bogowie jakim� cudem oczyszcz� twoje sumienie. "
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
