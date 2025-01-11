using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuyableCollection", menuName = "Scriptable Objects/BuyableCollection")]
public class BuyableCollection : ScriptableObject
{
    public List<RelicSO> relics;
    public List<CardSO> cards;

    public List<IBuyableSO> GetBuyableSOs()
    {
        List<IBuyableSO> outputList = new List<IBuyableSO>();
        foreach (RelicSO rso in relics)
        {
            outputList.Add(rso);
        }

        foreach (CardSO cso in cards)
        {
            outputList.Add(cso);
        }

        return outputList;
    }
}
