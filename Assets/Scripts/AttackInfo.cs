using UnityEngine;
using System.Collections.Generic;

public class AttackInfo
{
    public Actor source;
    public Actor target;
    public Vector2 sourcePosition;
    public Vector2 targetPosition;
    public string word;
    public List<char> wordChars;
    public List<Card> cards;
    public int cardCount;
    public float cardCountMultiplier;
}
