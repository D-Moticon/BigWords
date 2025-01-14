using UnityEngine;
using System.Collections.Generic;

public class CardCreator : MonoBehaviour
{
    public Card cardPrefab;

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

    public Card CopyCard(Card c)
    {
        Card newCard = Instantiate(c);
        return newCard;
    }

    public Card CreateCardFromSOWithTraitsOfExistingCard(Card c, CardSO cso)
    {
        Card newCard = CreateCardFromSO(cso, c.letter, c.GetPower());
        return newCard;
    }
}
