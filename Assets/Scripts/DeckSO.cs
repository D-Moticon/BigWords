using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Objects/DeckSO")]
public class DeckSO : ScriptableObject
{

    //This class contains the card, the letter, and any other modifiers
    [System.Serializable]
    public class CardInfo
    {
        public CardSO card;
        public char letter;
        public bool overridePower = false;
        [ShowIf("@overridePower == true")]
        public float power = 1f;
        public int quantity;
    }

    public List<CardInfo> cardInfos;

}
