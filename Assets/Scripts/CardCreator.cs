using UnityEngine;
using System.Collections.Generic;

public class CardCreator : MonoBehaviour
{
    public Card cardPrefab;

    public Deck CreateDeckFromSO(DeckSO deckSO)
    {
        Deck deck = new GameObject().AddComponent<Deck>();
        deck.name = "NewDeck";

        for (int i = 0; i < deckSO.cards.Count; i++)
        {
            Card card = CreateCardFromSO(deckSO.cards[i].card, deckSO.cards[i].letter);
            //print(card.name);
            deck.AddCard(card);
        }

        return deck;
    }

    public Card CreateCardFromSO(CardSO cardSO, char letter)
    {
        Card newCard = Instantiate(cardPrefab, this.transform.position, Quaternion.identity);
        newCard.SetCardName(cardSO.cardName);
        newCard.ClearEffectTexts();

        foreach (var effect in cardSO.cardEffects)
        {
            newCard.AddCardEffect(effect);
        }

        newCard.SetLetter(letter);

        return newCard;
    }
}
