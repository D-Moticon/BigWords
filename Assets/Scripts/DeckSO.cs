using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Objects/DeckSO")]
public class DeckSO : ScriptableObject
{

    public enum LetterAssignMode
    {
        ensureBalancedDeck,
        manual,
        weightedRandom,
        random
    }

    //This class contains the card, the letter, and any other modifiers
    [System.Serializable]
    public class CardInfo
    {
        public CardSO card;
        public int quantity;

        public LetterAssignMode letterAssignMode;
        [ShowIf("letterAssignMode", LetterAssignMode.manual)]
        public char letter;
        public bool overridePower = false;
        [ShowIf("@overridePower == true")]
        public float power = 1f;
        
    }

    public List<CardInfo> cardInfos;

    public Deck CreateDeckFromSO()
    {
        Deck deck = new GameObject().AddComponent<Deck>();
        deck.name = "NewDeck";

        // Reference LetterPicker (assumes one exists in scene)
        LetterPicker letterPicker = Singleton.Instance.letterPicker;

        // Calculate total letters needed for ensureBalancedDeck mode
        int totalEnsureCount = 0;
        foreach (var info in cardInfos)
        {
            if (info.letterAssignMode == LetterAssignMode.ensureBalancedDeck)
                totalEnsureCount += info.quantity;
        }

        // Generate balanced pool of letters ensuring at least one of each if possible
        List<char> balancedPool = new List<char>();

        if (totalEnsureCount >= 26)
        {
            // Add one of each letter to the pool
            for (char c = 'A'; c <= 'Z'; c++)
            {
                balancedPool.Add(c);
            }

            // Fill the rest of the pool with random letters
            for (int k = 26; k < totalEnsureCount; k++)
            {
                balancedPool.Add(letterPicker.GetRandomLetter().letterChar);
            }
        }
        else
        {
            // If we can't guarantee one of each letter, fill the pool normally
            for (int k = 0; k < totalEnsureCount; k++)
            {
                balancedPool.Add(letterPicker.GetRandomLetter().letterChar);
            }
        }

        // Shuffle the pool
        for (int m = 0; m < balancedPool.Count; m++)
        {
            int r = Random.Range(m, balancedPool.Count);
            char temp = balancedPool[m];
            balancedPool[m] = balancedPool[r];
            balancedPool[r] = temp;
        }

        // Now proceed with creating cards...
        for (int i = 0; i < cardInfos.Count; i++)
        {
            for (int j = 0; j < cardInfos[i].quantity; j++)
            {
                char letter = 'a';  // default

                switch (cardInfos[i].letterAssignMode)
                {
                    case LetterAssignMode.ensureBalancedDeck:
                        if (balancedPool.Count > 0)
                        {
                            letter = balancedPool[balancedPool.Count - 1];
                            balancedPool.RemoveAt(balancedPool.Count - 1);
                        }
                        else
                        {
                            // Fallback if pool is empty
                            letter = letterPicker.GetRandomLetter().letterChar;
                        }
                        break;

                    case LetterAssignMode.manual:
                        letter = cardInfos[i].letter;
                        break;

                    case LetterAssignMode.weightedRandom:
                        letter = letterPicker.GetRandomLetter().letterChar;
                        break;

                    case LetterAssignMode.random:
                        letter = (char)Random.Range('A', 'Z' + 1);
                        break;
                }

                float power = cardInfos[i].card.power;
                if (cardInfos[i].overridePower)
                {
                    power = cardInfos[i].power;
                }

                else
                {
                    power = Singleton.Instance.letterPicker.GetLetterScore(letter);
                }

                Card card = Card.CreateCardFromSO(cardInfos[i].card, letter, power);
                deck.AddCardPermanently(card);
            }
        }

        return deck;
    }

}
