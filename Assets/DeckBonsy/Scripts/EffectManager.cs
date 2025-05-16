using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectManager : MonoBehaviour
{
    public static EffectManager effectManager { get; private set; }

    [Header("Main Variables")]
    [SerializeField] private Dictionary<int, Action<CardContainer, CardContainer>> effects = new Dictionary<int, Action<CardContainer, CardContainer>>();

    private void Awake()
    {
        /// Singleton mechanism
        {
            if (effectManager != null && effectManager != this)
            {
                Destroy(this);
            }
            else
            {
                effectManager = this;
            }
        }

    }

    private void Start()
    {
        effects[0] = (playedCard, chosenCard) => Effects.NoEffect(); // no effect; default
        effects[1] = (playedCard, chosenCard) => Effects.Emperor(playedCard); // no effect; stealable=false
        effects[2] = (playedCard, chosenCard) => Effects.Kaeso(playedCard); // halfway done
        effects[3] = (playedCard, chosenCard) => Effects.Helion(playedCard); // ez
        effects[4] = (playedCard, chosenCard) => Effects.Aulus(playedCard); // "memory cloak"
        effects[5] = (playedCard, chosenCard) => Effects.Magnus(playedCard, chosenCard); // "sap"
        effects[6] = (playedCard, chosenCard) => Effects.Domina(playedCard); // done
        effects[7] = (playedCard, chosenCard) => Effects.Infiltrator(playedCard); // "scry??? 1"
        effects[8] = (playedCard, chosenCard) => Effects.Robber(playedCard); // "mind control"
        effects[9] = (playedCard, chosenCard) => Debug.Log("9"); // nan
        effects[10] = (playedCard, chosenCard) => Debug.Log("10"); // nan
        effects[11] = (playedCard, chosenCard) => Debug.Log("11"); // nan
        effects[12] = (playedCard, chosenCard) => Debug.Log("12"); // nan
    }

    public void TriggerCardEffect(int effectId, CardContainer playedCard, CardContainer chosenCard)
    {
        effects[effectId](playedCard, chosenCard);
    }

    class Effects
    {
        static public void NoEffect()
        {
            Debug.Log("No effect!");
        }

        static public void Emperor(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            cardInfo.SetRemovable(false);
        }

        static public void Kaeso(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            int dec = GameManager.gameManager.CountTypeOfCardOnBoard(CardType.Politician, playedCard.GetIsPlayerCard());
            int inc = GameManager.gameManager.CountTypeOfCardOnBoard(CardType.Slave, playedCard.GetIsPlayerCard());

            Debug.Log("Kaeso effect trigger! (" + (1 + (inc * inc) - dec) + ")");
            cardInfo.IncreasePoints(1 + (inc * inc) - dec); // despite it saying "Increase" it can decrease the value :*
            // 1+ is there because this fella counts himself :D
        }

        static public void Helion(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            if (!cardInfo.effectFired)
            {
                cardInfo.SetEffectFired(true);

                Card mockCitizen = new Card();
                mockCitizen.SetValues(15, "Citizen", "MIESZCZANIN\nJednostka bazowa o wartoœci 1.", 0, 1, CardType.Citizen, DeckManager.deckManager.GetCardSprite(15));

                Debug.Log("Helion's Citizen card added!");
                HandManager.handManager.AddCardToHand(mockCitizen, playedCard.GetIsPlayerCard());
            }

            int inc = GameManager.gameManager.CountTypeOfCardInColumn(CardType.Citizen, playedCard.GetIsPlayerCard(), playedCard.GetColumnIndex());

            Debug.Log("Helion effect trigger! (" + (inc * inc) + ")");
            cardInfo.IncreasePoints(inc * inc);
            // notice no more +1
        }

        static public void Aulus(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            if (playedCard.GetRowIndex() > 0)
            {
                if (GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() - 1, playedCard.GetIsPlayerCard()) != null)
                {
                    GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() - 1, playedCard.GetIsPlayerCard()).SetRemovable(false);
                    GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() - 1, playedCard.GetIsPlayerCard()).SetStealable(false);
                }
            }
            else
            {
                if (GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() + 1, playedCard.GetIsPlayerCard()) != null)
                {
                    GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() + 1, playedCard.GetIsPlayerCard()).SetRemovable(false);
                    GameManager.gameManager.GetCardAtPosition(playedCard.GetColumnIndex(), playedCard.GetRowIndex() + 1, playedCard.GetIsPlayerCard()).SetStealable(false);
                }
            }
        }

        static public void Magnus(CardContainer playedCard, CardContainer chosenCard)
        {
            Card cardInfo = playedCard.GetCardInfo();
            int enemyHandSize = HandManager.handManager.GetHandSize(!playedCard.GetIsPlayerCard());

            /*if(chosenCard.GetCardInfo().points<4 && !cardInfo.effectFired)
            {
                cardInfo.SetEffectFired(true);
                if (enemyHandSize < HandManager.handManager.GetMaxHandSize())
                {
                    HandManager.handManager.AddCardToHand(chosenCard.GetCardInfo(), !playedCard.GetIsPlayerCard());
                }
                GameManager.gameManager.RemoveCardAtPosition(chosenCard.GetColumnIndex(), chosenCard.GetRowIndex(), true, playedCard.GetIsPlayerCard());
            }*/
        }

        static public void Domina(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            int inc = GameManager.gameManager.CountTypeOfCardInColumn(CardType.Soldier, playedCard.GetIsPlayerCard(), playedCard.GetColumnIndex());

            Debug.Log("Domina effect trigger! (" + inc + ")");
            cardInfo.IncreasePoints(inc);
        }

        static public void Infiltrator(CardContainer playedCard)
        {
            // ???
        }

        static public void Robber(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            // store a copy of the card, remove it from the board, place in robber, move the copy to hand
        }

        // more effects added here
        // static public void <cardName>() {   <effect>    }

    }



}