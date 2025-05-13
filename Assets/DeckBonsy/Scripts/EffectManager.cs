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
        effects[1] = (playedCard, chosenCard) => Effects.Emperor();
        effects[2] = (playedCard, chosenCard) => Effects.Kaeso(playedCard);
        effects[3] = (playedCard, chosenCard) => Debug.Log("3");
        effects[4] = (playedCard, chosenCard) => Debug.Log("4");
        effects[5] = (playedCard, chosenCard) => Debug.Log("5");
        effects[6] = (playedCard, chosenCard) => Effects.Domina(playedCard);
        effects[7] = (playedCard, chosenCard) => Debug.Log("7");
        effects[8] = (playedCard, chosenCard) => Debug.Log("8");
        effects[9] = (playedCard, chosenCard) => Debug.Log("9");
        effects[10] = (playedCard, chosenCard) => Debug.Log("10");
        effects[11] = (playedCard, chosenCard) => Debug.Log("11");
        effects[12] = (playedCard, chosenCard) => Debug.Log("12");
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

        static public void Emperor()
        {
            // something something, can't be stolen
        }

        static public void Kaeso(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            int dec = GameManager.gameManager.CountTypeOfCardOnBoard(CardType.Politician, playedCard.GetIsPlayerCard());
            int inc = GameManager.gameManager.CountTypeOfCardOnBoard(CardType.Slave, playedCard.GetIsPlayerCard());

            Debug.Log("Kaeso effect trigger! (" + (1 + inc - dec) + ")");
            cardInfo.IncreasePoints(1 + inc - dec); // despite it saying "Increase" it can decrease the value :*
            // 1+ is there because this fella counts himself :D
        }

        static public void Helion(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            int inc = GameManager.gameManager.CountTypeOfCardInColumn(CardType.Citizen, playedCard.GetIsPlayerCard(), playedCard.GetColumnIndex());

            Debug.Log("Helion effect trigger! (" + inc + ")");
            cardInfo.IncreasePoints(inc);
            // notice no more +1
        }

        static public void Domina(CardContainer playedCard)
        {
            Card cardInfo = playedCard.GetCardInfo();

            int inc = GameManager.gameManager.CountTypeOfCardInColumn(CardType.Soldier, playedCard.GetIsPlayerCard(), playedCard.GetColumnIndex());

            Debug.Log("Domina effect trigger! (" + inc + ")");
            cardInfo.IncreasePoints(inc);
        }

        // more effects added here
        // static public void <cardName>() {   <effect>    }

    }



}