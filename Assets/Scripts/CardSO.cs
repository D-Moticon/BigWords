using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject, IBuyableSO
{
    public string cardName;
    public CardType cardType;
    public Sprite cardArt;
    public float basePrice = 5f;
    public bool hasPower = true;
    [ShowIf("hasPower", true)]
    public float power = 1f;
    [SerializeReference]
    public List<Effect> cardEffects;
    public string descriptionOverride = "";
    public List<CardSO> upgradeCards;

    public string GetDescription()
    {
        if (!string.IsNullOrEmpty(descriptionOverride))
        {
            return descriptionOverride;
        }

        string s = "";

        foreach (Effect e in cardEffects)
        {
            s += e.GetEffectDescription();
            s += "\n";
        }

        return s;
    }
}
