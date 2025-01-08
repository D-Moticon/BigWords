using UnityEngine;
using System.Collections.Generic;

public class CardCreator : MonoBehaviour
{
    public Card cardPrefab;

    public Deck CreateDeckFromSO(DeckSO deckSO)
    {
        Deck deck = new GameObject().AddComponent<Deck>();
        deck.name = "NewDeck";

        for (int i = 0; i < deckSO.cardInfos.Count; i++)
        {
            for (int j = 0; j < deckSO.cardInfos[i].quantity; j++)
            {
                Card card = CreateCardFromSO(deckSO.cardInfos[i].card, deckSO.cardInfos[i].letter);
                card.MoveCardToDeck(deck, 0.01f, false);
            }
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
        newCard.SetCardImage(cardSO.cardArt);

        return newCard;
    }
}
