using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<Card> allCards;
    public List<Card> currentCards;

    public void AddCard(Card card)
    {
        if (allCards == null)
        {
            allCards = new List<Card>();
        }

        if (currentCards == null)
        {
            currentCards = new List<Card>();
        }

        if (!allCards.Contains(card))
        {
            allCards.Add(card);
        }

        if (!currentCards.Contains(card))
        {
            currentCards.Add(card);
        }

        MoveCardToDeck(card);
    }

    public void MoveCardToDeck(Card card)
    {
        card.transform.parent = this.transform;
        card.transform.localPosition = Vector3.zero;
    }

    public void MoveCardOffDeck(Card card)
    {
        card.transform.SetParent(null);
        currentCards.Remove(card);
    }

    public Card TakeCardFromTop()
    {

        //using for to prevent drawing nulls
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i] != null)
            {
                Card c = currentCards[i];
                MoveCardOffDeck(c);
                return c;
            }
        }

        print("Tried to take card from top but no card existed in deck");

        return null;
    }
}
