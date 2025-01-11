using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardPackSO", menuName = "Scriptable Objects/CardCollectionSO")]
public class CardCollectionSO : ScriptableObject
{
    [System.Serializable]
    public class CardInfo
    {
        public CardSO cardSO;
    }

    public List<CardInfo> cardInfos;
}
