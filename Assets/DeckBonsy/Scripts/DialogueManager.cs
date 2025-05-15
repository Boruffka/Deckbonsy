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
                    npcLine = "No ju¿, siadaj do sto³u, przekonajmy siê na kogo wychowa³ Ciê najwiêkszy twardziel jakiego by³o mi dane poznaæ. Prawdziw¹ wojowniczkê mo¿na poznaæ tylko na polu bitwy. Mo¿e karty to nie to samo co dobra bitka wrêcz, ale lepsze to ni¿ gadanie o sanda³ach imperatora.",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };
            case 1: //flint
                return new DialogueData
                {
                    npcLine = "Hmmm, trzy razy szeœæ to bêdzie osiemnaœcie, mniej wiêcej tyle zapalniczek dziennie trafia na odrzut, a to wszystko z powodu jakiejœ drobnej wady. Wynios³em ich z fabryki ju¿ wystarczaj¹c¹ iloœæ, ¿eby zacz¹æ pracowaæ nad prototypem! Twój ojciec by³ zawsze sceptyczny w stosunku do moich wynalazków, ale w twoich oczach widzê… iskrê, iskrê która roznieci tutaj ogieñ. A jeœli chodzi o po¿ary to nie mog³aœ trafiæ lepiej! Siadaj zagrajmy jak piroman z piromanem, hahaha!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "Œmieræ twojego ojca jest wielk¹ strat¹, nie tylko dla waszego gatunku... M¹dry by³ z niego towarzysz, a jak siê z nim gra³o w deckbonsy! Wiem, ¿e ciê¿ko jest w to uwierzyæ, w koñcu jestem stra¿nikiem, ale za czasów mojej s³u¿by cieliœmy z Twoim starszym w karty jak równy z równym. Mo¿e moja kondycja nie jest jak dawniej, ale uwierz mi… mój umys³ nadal pracuje. widzê co siê dzieje. ta agresja… Wiem, ¿e jesteœcie wœciekli,, ale politycy… oni s¹ bezwzglêdni. ¯ycia tylu ludzi…niewolników, cywili, nie znacz¹ dla nich wiêcej ni¿ ten piach (kopie w ziemiê z rezygnacj¹)… Mo¿e jest jakieœ pokojowe wyjœcie, które pomo¿e nam unikn¹æ masakry.  Jestem w stanie zaoferowaæ swoj¹ pomoc!",
                    playerChoices = new string[0],
                    endings = new int[0],
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "Proszê proszê… Nowy przedstawiciel wielkiego wyzwolenia. Widzê, ¿e jak na razie wasze irracjonalne plany siê nie zmieniaj¹. Ale có¿… nie ka¿dy myœli o tych najs³abszych, których czeka najwiêksze niebezpieczeñstwo… albo o tych, których nie do koñca interesuj¹ losy waszej rasy. Ale do rzeczy. Twój ojciec twierdzi³, ¿e ma dla mnie ca³kiem nie najgorszy uk³ad. Mam nadziejê, ¿e masz w sobie tyle rozumu co on i zaoferujesz mi coœ co bêdzie korzystne nie tylko dla ciebie, ale i dla mnie. Mo¿e moja przychylnoœæ do was nie niesie za sob¹ niczego wielkiego, ale jej brak… Nie wiem czy ryzykowa³abym go doœwiadczaæ.",
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

            // jeœli to ostatnie zdanie — koñczymy dialog
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
            Debug.LogWarning("Auto zakoñczono dialog po czasie");
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
                    npcLine = "Muszê przyznaæ, ¿e masz ³eb do taktyki, nic nie wpienia mnie tak jak przegrana, a¿ ma siê ochotê krzykn¹æ IMPERATOR MA HALUKSA!!",
                    playerChoices = new[] { "Ucisz krzykiem", "Ucisz niepewnie" },
                    playerExpandedResponses = new[] {
                    "Triceps! Paszcza w kube³, jak chcesz siê wy¿yæ to zorganizuje ci pierwszy rz¹d w powstaniu, wtedy sklepiesz tyle blaszanych he³mów ile tylko zechcesz.",
                    "*G³os ze strachu za³amuje ci siê w po³owie zdania* Panie Maximusie…. proszê niech Pan nie robi teraz ha³asu."
                },
                    npcResponses = new[] {
                    "*gdy dociera do niego twój krzyk, okazuje niepewnoœæ* Wybacz szefowo… *przechodzi gwa³townie z krzyku w szept*. Oczywiœcie, ¿e chcê walczyæ w pierwszym rzêdzie. Po prostu… bardzo jestem zdenerwowany, wkurzony i te inne… Przepraszam za bycie ma³o dyskretnym. Zajmê siê przygotowaniami i ojej… po¿a³uj¹ wszystkiego co nam zrobili. ",
                    "Nie robiæ ha³asu?! Nie mo¿na siê baæ tych skurczybyków! Trzeba im pokazaæ gdzie ich miejsce! Elity Rzymu upadn¹! I co najwa¿niejsze: IMPERATOR MA HALUKSA!! *po krótkiej chwili widzisz, ¿e w wasz¹ stronê idzie patrol rzymskich stra¿ników. Wiedz¹c co siê œwieci porzucasz rozmowê z Tricepsem i uciekasz ukradkiem*"
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound0,
                    backgroundImage = backgroundImageRound0
                };

            case 1: //flint
                return new DialogueData
                {
                    npcLine = "To by³a gor¹ca rozgrywka. No ale ale! Czy zdecydujesz siê na moj¹ propozycjê? Gor¹co zachêcam.",
                    playerChoices = new[] { "To z³y pomys³", "To nas uratuje" },
                    playerExpandedResponses = new[] {
                    "Chyba muszê ostudziæ twój zapa³. Ten prototyp… on mo¿e byæ niebezpieczny równie¿ dla nas. Oczywiœcie, ¿e ka¿da pomoc siê przyda, ale nie ryzykujmy ¿ycia naszych.",
                    "Musimy próbowaæ z u¿yciem ka¿dej si³y. Rzymianie s¹ bezwzglêdni. Mo¿e to ryzykowny pomys³, ale jeœli ten wynalazek oka¿e siê skuteczny, mo¿e byæ znacz¹cy dla naszego zwyciêstwa."
                },
                    npcResponses = new[] {
                    "Có¿… trochê spodziewa³em siê, ¿e mo¿esz byæ jak twój poprzednik. Niedaleko pada jab³ko od jab³oni, jak to mawiaj¹. Jedyne co mogê powiedzieæ - twoja strata. A raczej nasza. No ale co mogê zrobiæ. Widocznie nie sprzeda³em swojego wynalazku wystarczaj¹co dobrze. Ale jak mo¿esz siê spodziewaæ, mimo twojego braku zgody na jego u¿ycie, oferujê swoj¹ pomoc. ",
                    "Ha ha! Wiedzia³em, ¿e jesteœ inna. Wyczu³em to od razu. Zapewniam ciê, ¿e wygramy t¹ walkê, a mój gad¿et siê do tego przyczyni. Mo¿e nawet bêdê s³awny. Rzymianie zapamiêtaj¹ mnie jako ich najwiêkszy koszmar. Jeœli któryœ z nich wyjdzie z tego wszystkiego ca³o."
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };

            case 2: //fabius
                return new DialogueData
                {
                    npcLine = "Ca³kiem nie najgorzej grasz, moja droga. Widzê, ¿e twój ojciec dobrze nauczy³ ciê taktyki, co pewnie udzieli siê równie¿ na polu bitwy, jednak mo¿e pos³uchasz mojej rady i wykorzystasz swoje zdolnoœci w bardziej dyplomatyczny sposób. B³agam ciê ca³ym sob¹, bo to jedyne co mogê teraz zrobiæ. Rzymianie te¿ potrafi¹ wspó³czuæ, musimy im tylko wskazaæ drogê, pokazaæ, ¿e niewolnictwo to bestialstwo. A wiêc jaka jest twoja decyzja?",
                    playerChoices = new[] { "Wybieram si³ê", "Wybieram pokój" },
                    playerExpandedResponses = new[] {
                    "Nasze ¿ycie siê dla nich nie liczy. Jeœli powstanie ma wybuchn¹æ to musi byæ ono przeprowadzone z ca³¹ si³¹, na jak¹ nas staæ.",
                    "Mo¿e rzeczywiœcie uda nam siê coœ ugraæ na drodze dyplomacji. Myœlisz, ¿e potrafi³byœ mi w tym pomóc?"
                },
                    npcResponses = new[] {
                    "Oh… czyli to postanowione, historia znów zatacza ko³o… (jego oczy wydaj¹ siê puste, jakby powróci³y do niego wszystkie dawne wspomnienia). (Fabius wstaje od sto³u i przygl¹da siê tobie). Wybacz, ale w takim wypadku nasze drogi musz¹ siê tutaj rozejœæ. ¯yczê wam powodzenia, naprawdê. Ale nawet jeœli wszystko uda siê wam na drodze agresji, to musisz wiedzieæ, ¿e takie wydarzenia zmieniaj¹ ka¿dego. Niech Bogowie maj¹ pod opiek¹ wasze dusze… ",
                    "Tak! Na bogów, wiem dok³adnie co robiæ! Audiencja. To jest to. Na szczêœcie masz do czynienia z by³ym cenionym stra¿nikiem. Myœlê, ¿e uda mi siê tak¹ zorganizowaæ. Zobaczysz, cywile te¿ bêd¹ po naszej stronie. Przekonamy ich, no oczywiœcie nie wszystkich, ale to zawsze coœ. "
                },
                    endings = new[] { 0, 1 },
                    npcImage = npcImageRound1,
                    backgroundImage = backgroundImageRound1
                };
                
            case 3: //minerva
                return new DialogueData
                {
                    npcLine = "No dobrze. Przyznam, ¿e jestem w szoku. Pokona³aœ w karty najbardziej przebieg³¹ mistrzyniê. Ale có¿… zapewniam, ¿e jedna przegrana mnie nie definiuje, poza tym… nie jesteœmy tu tak naprawdê dla kart, prawda? A wiêc poka¿ mi co dla mnie masz.",
                    playerChoices = new[] { "Ofiaruj azyl", "Oferuj przepustkê" },
                    playerExpandedResponses = new[] {
                    "No dobrze. Skoro tak bardzo zale¿y ci na potencjalnych ofiarach, myœlê ¿e mo¿emy poœwiêciæ kilku walcz¹cych i zorganizowaæ dla dzieci  i bezbronnych jakiœ azyl.",
                    "Myœlê, ¿e mam coœ, co mog³oby ciê zainteresowaæ… (wyci¹gnij przepustkê wyjœcia z getta)."
                },
                    npcResponses = new[] {
                    "Oh, jak¿e dobroduszny gest. I to dopiero rozmowa ze mn¹ sprawi³a, ¿e obudzi³o siê wasze sumienie? Có¿… chyba bêdê musia³a przyj¹æ tak¹ ofertê, chocia¿ nie ukrywam, ¿e liczy³am na coœ zgo³a innego. No ale… nie mo¿na mieæ wszystkiego, tak? W takim razie pozwól, ¿e na tym zakoñczymy. Mo¿e jeszcze zobaczymy siê w niedalekiej przysz³oœci, chocia¿ mam ku temu spore w¹tpliwoœci. ",
                    "Hm… nieczyste zagranie.(uœmiecha siê pod nosem) Ale có¿ innego mi pozosta³o… Przynajmniej bêdê mog³a zaj¹æ siê w koñcu problemami swoich ludzi. Zatem… powodzenia. Mo¿e jeœli wygracie bogowie jakimœ cudem oczyszcz¹ twoje sumienie. "
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
