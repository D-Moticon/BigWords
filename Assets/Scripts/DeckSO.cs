using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Objects/DeckSO")]
public class DeckSO : ScriptableObject
{

    //This class contains the card, the letter, and any other modifiers
    [System.Serializable]
    public class CardInfo
    {
        public CardSO card;
        public char letter;
        public int quantity;
    }

    public List<CardInfo> cardInfos;

}
