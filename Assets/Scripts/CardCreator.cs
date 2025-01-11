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
                float power = deckSO.cardInfos[i].card.power;
                if (deckSO.cardInfos[i].overridePower)
                {
                    power = deckSO.cardInfos[i].power;
                }

                Card card = Card.CreateCardFromSO(deckSO.cardInfos[i].card, deckSO.cardInfos[i].letter, power);
                card.MoveCardToDeck(deck, 0.01f, false);
            }
        }

        return deck;
    }

    public Card CreateCardFromSO(CardSO cardSO, char letter = 'A', float power = 1)
    {
        Card newCard = Instantiate(cardPrefab, this.transform.position, Quaternion.identity);
        newCard.SetCardName(cardSO.cardName);
        newCard.SetCardType(cardSO.cardType);
        newCard.basePrice = cardSO.basePrice;
        newCard.cardSO = cardSO;
        newCard.cardEffects = cardSO.cardEffects;

        /*foreach (var effect in cardSO.cardEffects)
        {
            newCard.AddCardEffect(effect);
        }*/

        newCard.SetLetter(letter);
        newCard.SetCardImage(cardSO.cardArt);
        newCard.SetHasPower(cardSO.hasPower);
        newCard.SetPower(power);
        newCard.SetCardDescription(cardSO.GetDescription());

        return newCard;
    }
}
