using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite cardArt;
    [SerializeReference]
    public List<CardEffect> cardEffects;
}
