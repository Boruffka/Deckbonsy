using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }

    [Header("Main Variables")]
    [SerializeField] public bool isPlayerTurn { get; private set; }
    [SerializeField] private bool gameReady;
    [SerializeField] private bool isCardBeingPlayed;
    [SerializeField] private CardContainer cardContainerBeingPlayed;
    [SerializeField] private int scoreToWin;
    [SerializeField] private int currentRound;
    public int CurrentRound => currentRound;

    [Header("Input System")]
    [SerializeField] private bool chosenCard;
    [SerializeField] private int chosenCardIndex;
    [SerializeField] private bool chosenColumn;
    [SerializeField] private int chosenColumnIndex;
    [SerializeField] private bool chosenCardContainerInPlay;
    [SerializeField] private CardContainer chosenCardContainerInPlayObject;
    [SerializeField] public int selectedCardIndex;

    [Header("Board References")]
    [SerializeField] private Board playerBoard;
    [SerializeField] private Board enemyBoard;
    [SerializeField] private TextMeshProUGUI playerScoreCounter;
    [SerializeField] private TextMeshProUGUI enemyScoreCounter;

    [Header("Background Sprites")]
    [SerializeField] private UnityEngine.UI.Image backgroundImage;
    [SerializeField] private Sprite playerBackground;
    [SerializeField] private Sprite enemyBackground;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private UnityEngine.UI.Button restartButton;

    [Header("Draw Texts")]
    [SerializeField] private GameObject playerDrawText;
    [SerializeField] private GameObject enemyDrawText;

    [Header("Dialogues")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Journal")]
    [SerializeField] private GameObject journalButton;
    [SerializeField] private JournalUpdateManager journalUpdateManager;

    private bool journalOpenedThisDialogue = false;
    private bool introShownThisRound = false;
    private bool startCardGameAfterIntro = false;

    private void Awake()
    {
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey("JournalPage_" + i);
        }
        PlayerPrefs.Save();


        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        }
        else
        {
            gameManager = this;
        }

        if (journalUpdateManager == null)
        {
            journalUpdateManager = FindFirstObjectByType<JournalUpdateManager>();
            if (journalUpdateManager == null)
            {
                Debug.LogError("JournalUpdateManager nie znaleziony!");
            }
        }

        // Ustawienia startowe — PRZENIESIONE Z START()
        currentRound = 0;
        introShownThisRound = false;
        gameReady = false;
        startCardGameAfterIntro = false;

        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager nie przypisany w Inspectorze – próbuję znaleźć go ręcznie.");
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }

        ShowIntroDialogueForRound();
    }


    private void Start()
    {
       
        //currentRound = 0;
        endGamePanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);

        scoreToWin = 5;
        isCardBeingPlayed = false;
        chosenCard = false;
        chosenColumn = false;
        isPlayerTurn = true;

        if (journalButton != null)
        {
            journalButton.SetActive(true);
            journalButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OpenJournal);
        }

        if (!PlayerPrefs.HasKey("JournalClearedOnce"))
        {
            ClearAllJournalDataOnce();
        }
        if (PlayerPrefs.GetInt("GameStartedOnce", 0) == 0)
        {
            currentRound = 0;
            PlayerPrefs.SetInt("GameStartedOnce", 1);
        }


        // ShowIntroDialogueForRound();
    }

    public void StartRound(int round)
    {
        currentRound = round;
        // Resetuj planszę, dobierz karty itd.
        playerBoard.ClearBoard(); 
        enemyBoard.ClearBoard();

        HandManager.handManager.ClearHands();
        DeckManager.deckManager.ResetDeck();
        DeckManager.deckManager.ShuffleDeck();

        gameManager.isPlayerTurn = true;

        endGamePanel.SetActive(false);

        // Nie odpalaj dialogu — tylko gra:
        StartCardGameForNewRound();
    }

    private void ShowIntroDialogueForRound()
    {
       
    
            if (currentRound == 0)
            {
                Debug.Log("⛔ Pomijam intro rundy 0 – obsługuje je IntroStarter.");
                return;
            }

            Debug.Log("Próba odpalenia intro dla rundy: " + currentRound);

            if (introShownThisRound)
            {
                Debug.Log("Intro już pokazane.");
                return;
            }


            var intro = dialogueManager.GetIntroDialogueForRound(currentRound);

        if (intro != null)
        {
            Debug.Log("Znalazłem intro tekst, uruchamiam StartDialogue!");
            gameReady = false;
            dialoguePanel.SetActive(true);
            dialogueManager.StartDialogue(intro);
            dialogueManager.OnDialogueEnd += OnIntroDialogueEnd;
        }
        else
        {
            Debug.Log("Brak intro, przechodzę dalej.");
            OnIntroDialogueEnd();
        }
    }


    private void OnIntroDialogueEnd()
    {
        dialogueManager.OnDialogueEnd -= OnIntroDialogueEnd;
        dialoguePanel.SetActive(false);
        startCardGameAfterIntro = true;
    }

    private void StartCardGameForNewRound()
    {
        DeckManager.deckManager.ResetDeck();
        DeckManager.deckManager.ShuffleDeck();
        HandManager.handManager.ClearHand();
        playerBoard.ClearBoard();
        enemyBoard.ClearBoard();

        chosenCard = false;
        chosenColumn = false;
        isCardBeingPlayed = false;
        isPlayerTurn = true;
        introShownThisRound = true;
        gameReady = true;

        UpdateScore();
    }

    private void Update()
    {
        if (startCardGameAfterIntro)
        {
            startCardGameAfterIntro = false;
            StartCardGameForNewRound();
            return;
        }

        if (!gameReady) return;

        if (!isCardBeingPlayed && chosenCard && chosenColumn)
        {
            isCardBeingPlayed = true;
            cardContainerBeingPlayed = HandManager.handManager.GetCardObjectByIndex(chosenCardIndex).GetComponent<CardContainer>();

            if (isPlayerTurn && playerBoard.CheckForEmptyInColumn(chosenColumnIndex))
            {
                playerBoard.AddCardToColumn(HandManager.handManager.GetCardObjectByIndex(chosenCardIndex), chosenColumnIndex);
                HandManager.handManager.RemoveCardFromHand(chosenCardIndex);
                ResetChoices();
                UpdateScore();
                CheckForRoundEnd();
            }
            else if (!isPlayerTurn && enemyBoard.CheckForEmptyInColumn(chosenColumnIndex))
            {
                enemyBoard.AddCardToColumn(HandManager.handManager.GetCardObjectByIndex(chosenCardIndex), chosenColumnIndex);
                HandManager.handManager.RemoveCardFromHand(chosenCardIndex);
                ResetChoices();
                UpdateScore();
                CheckForRoundEnd();
            }
            else
            {
                Debug.Log("Column full! Pick again.");
                ResetChoices();
            }
        }

        if (isCardBeingPlayed)
        {
            Card playedCard = cardContainerBeingPlayed.GetCardInfo();
            Board inactiveBoard = isPlayerTurn ? enemyBoard : playerBoard;

            if (DoesEffectIdRequireInput(playedCard.effectId) && !inactiveBoard.IsBoardEmpty())
            {
                if (chosenCardContainerInPlay)
                {
                    isCardBeingPlayed = false;
                    EffectManager.effectManager.TriggerCardEffect(playedCard.effectId, cardContainerBeingPlayed, chosenCardContainerInPlayObject);
                    RemoveCardsWithEqualPoints(chosenColumnIndex, playedCard.points);
                    EndTurn();
                    chosenCardContainerInPlay = false;
                }
            }
            else
            {
                isCardBeingPlayed = false;
                EffectManager.effectManager.TriggerCardEffect(playedCard.effectId, cardContainerBeingPlayed, null);
                RemoveCardsWithEqualPoints(chosenColumnIndex, playedCard.points);
                EndTurn();
            }
        }
    }

    private void PrepareNextRound()
    {
        DeckManager.deckManager.ResetDeck();
        DeckManager.deckManager.ShuffleDeck();
        HandManager.handManager.ClearHand();
        playerBoard.ClearBoard();
        enemyBoard.ClearBoard();

        chosenCard = false;
        chosenColumn = false;
        isCardBeingPlayed = false;
        isPlayerTurn = true;
        introShownThisRound = false;

        UpdateScore();
        ShowIntroDialogueForRound();

        var display = FindFirstObjectByType<JournalDisplayManager>();
        if (display != null)
        {
            display.SetMaxPageIndex(3); // odblokuj wszystkie 4 strony
        }

    }

    private void FinishGame(string result, int playerScore, int enemyScore, bool showDialogue)
    {
        gameReady = false;

        if (showDialogue)
        {
            StartDialogueScene(dialogueManager.GetDialogueForRound(currentRound));
        }
        else
        {
            endGamePanel.SetActive(true);
            resultText.text = result.ToUpper();
            scoreText.text = $"PLAYER: {playerScore}    ENEMY: {enemyScore}";

        }
    }


    public void StartDialogueScene(DialogueData dialogue)
    {
        gameReady = false;
        dialoguePanel.SetActive(true);
        dialogueManager.StartDialogue(dialogue);

        dialogueManager.OnDialogueEnd -= OnPostGameDialogueFinished;
        dialogueManager.OnDialogueEnd += OnPostGameDialogueFinished;
    }

    private void OnPostGameDialogueFinished()
    {
        dialogueManager.OnDialogueEnd -= OnPostGameDialogueFinished;
        dialoguePanel.SetActive(false);

        int npcIndex = currentRound;
        int playerChoice = dialogueManager.GetLastPlayerChoice();

        if (!journalOpenedThisDialogue && npcIndex == currentRound && playerChoice >= 0 && journalUpdateManager != null)
        {
            journalOpenedThisDialogue = true;
            journalUpdateManager.ShowNoteAfterDialogue(npcIndex, playerChoice, () =>
            {
                journalOpenedThisDialogue = false;
                currentRound++;
                PrepareNextRound();
            });
        }
        else
        {
            currentRound++;
            PrepareNextRound();
        }
    }

    private void CheckForRoundEnd()
    {
        if (!gameReady) return;

        bool isPlayerFull = playerBoard.IsBoardFull();
        bool isEnemyFull = enemyBoard.IsBoardFull();
        int playerScore = playerBoard.CountScore();
        int enemyScore = enemyBoard.CountScore();
        bool boardFull = isPlayerFull || isEnemyFull;

        if (playerScore >= scoreToWin && playerScore > enemyScore)
        {
            FinishGame("PLAYER WINS!", playerScore, enemyScore, true);
        }
        else if (enemyScore >= scoreToWin && enemyScore > playerScore)
        {
            FinishGame("ENEMY WINS!", playerScore, enemyScore, false);
        }
        else if (boardFull)
        {
            if (playerScore > enemyScore)
                FinishGame("PLAYER WINS!", playerScore, enemyScore, true);
            else if (enemyScore > playerScore)
                FinishGame("ENEMY WINS!", playerScore, enemyScore, false);
            else
                FinishGame("DRAW!", playerScore, enemyScore, true);
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        CardContainer.DeselectAllCards();
        UpdateDrawTexts();
        UpdateBackground();
        DeckManager.deckManager.UpdateDrawButtons(isPlayerTurn);
        UpdateScore();
        foreach (var card in FindObjectsOfType<CardContainer>())
        {
            card.UpdateInteractivityVisual();
        }
    }

    public void UpdateScore()
    {
        Debug.Log(" UpdateScore called!");

        int playerScore = playerBoard.CountScore();
        int enemyScore = enemyBoard.CountScore();

        Debug.Log($"Player score: {playerScore}, Enemy score: {enemyScore}");

        playerScoreCounter.text = $"Your score:\n{playerScore}";
        enemyScoreCounter.text = $"Enemy score:\n{enemyScore}";
    }

    private void UpdateDrawTexts()
    {
        playerDrawText.SetActive(isPlayerTurn);
        enemyDrawText.SetActive(!isPlayerTurn);
    }

    private void UpdateBackground()
    {
        backgroundImage.sprite = isPlayerTurn ? playerBackground : enemyBackground;
    }

    private void ResetChoices()
    {
        chosenCard = false;
        chosenColumn = false;
    }

    private bool DoesEffectIdRequireInput(int effectId)
    {
        return effectId == 5 || effectId == 8;
    }

    public bool GetPlayerTurn() => isPlayerTurn;

    public int CountTypeOfCardOnBoard(CardType type, bool isPlayerBoard)
    {
        return isPlayerBoard ? playerBoard.CountTypeOnBoard(type) : enemyBoard.CountTypeOnBoard(type);
    }

    public int CountTypeOfCardInColumn(CardType type, bool isPlayerBoard, int columnIndex)
    {
        return isPlayerBoard ? playerBoard.CountTypeInColumn(type, columnIndex) : enemyBoard.CountTypeInColumn(type, columnIndex);
    }

    public void SetChosenCardIndex(int _chosenCardIndex, bool _isPlayerCard)
    {
        if (isPlayerTurn == _isPlayerCard)
        {
            chosenCard = true;
            chosenCardIndex = _chosenCardIndex;
        }
    }

    public void SetChosenCardInPlayObject(CardContainer _chosenCardContainerInPlayObject)
    {
        chosenCardContainerInPlay = true;
        chosenCardContainerInPlayObject = _chosenCardContainerInPlayObject;
    }

    public void SetChosenColumnIndex(int _chosenColumnIndex, bool _isPlayerBoard)
    {
        if (isPlayerTurn == _isPlayerBoard)
        {
            chosenColumn = true;
            chosenColumnIndex = _chosenColumnIndex;
        }
    }

    public void RemoveCardsWithEqualPoints(int columnIndex, int cardPoints)
    {
        if (isPlayerTurn)
            enemyBoard.RemoveCardsFromColumn(columnIndex, cardPoints);
        else
            playerBoard.RemoveCardsFromColumn(columnIndex, cardPoints);
    }

    private void RestartGame()
    {
        endGamePanel.SetActive(false);
        introShownThisRound = false;
        journalOpenedThisDialogue = false;

        PrepareCurrentRound(); 
    }

    private void PrepareCurrentRound()
    {
        playerBoard.ClearBoard(); 
        enemyBoard.ClearBoard();

        HandManager.handManager.ClearHands();
        DeckManager.deckManager.ResetDeck();
        DeckManager.deckManager.ShuffleDeck();

        chosenCard = false;
        chosenColumn = false;
        isCardBeingPlayed = false;
        isPlayerTurn = true;

        UpdateScore();

        // ⬇️ UWAGA: NIE pokazuj dialogu ponownie po porażce!
        startCardGameAfterIntro = true;
    }


    private void OpenJournal()
    {
        var display = FindFirstObjectByType<JournalDisplayManager>();
        if (display != null)
        {
            int pageToOpen = currentRound > 0 ? currentRound - 1 : 0;
            display.OpenJournal(pageToOpen);
        }
        else
        {
            Debug.LogError("Brak JournalDisplayManager w scenie!");
        }
    }
    public Card GetCardAtPosition(int columnIndex, int rowIndex, bool isPlayerBoard)
    {
        return isPlayerBoard ? playerBoard.GetCardAtPosition(columnIndex, rowIndex) : enemyBoard.GetCardAtPosition(columnIndex, rowIndex);
    }
    public void RemoveCardAtPosition(int columnIndex, int rowIndex, bool bypassProtection, bool _isPlayerBoard)
    {
        if (_isPlayerBoard)
        {
            enemyBoard.RemoveCardAtPosition(columnIndex, rowIndex, bypassProtection);
        }
        else
        {
            playerBoard.RemoveCardAtPosition(columnIndex, rowIndex, bypassProtection);
        }
    }

    private void ClearAllJournalDataOnce()
    {
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey("JournalPage_" + i);
        }

        PlayerPrefs.SetInt("JournalClearedOnce", 1);
        PlayerPrefs.Save();
        Debug.Log("Dziennik wyczyszczony przy starcie gry.");
    }

}
