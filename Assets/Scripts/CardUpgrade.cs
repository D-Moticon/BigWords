using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CardUpgrade : MonoBehaviour, IBuyable
{
    Card originalCard;

    Card displayedOriginalCard;
    Card displayedUpgradedCard;

    public Transform originalCardTransform;
    public Transform upgradedCardTransform;

    public float price;

    public BoxCollider2D bc2d;

    public static CardUpgrade GenerateCardUpgradeFromCard(Card card)
    {
        CardSO originalCSO = card.cardSO;

        CardUpgrade cu = Instantiate(Singleton.Instance.gameManager.cardUpgradePrefab);

        Card[] oldCards = cu.GetComponentsInChildren<Card>();
        foreach (Card c in oldCards)
        {
            Destroy(c.gameObject);
        }

        cu.originalCard = card;
        cu.displayedOriginalCard = Singleton.Instance.cardCreator.CopyCard(card);
        cu.displayedUpgradedCard = card.GetRandomCardUpgrade(1f, out cu.price);

        cu.displayedOriginalCard.transform.SetParent(cu.originalCardTransform);
        cu.displayedOriginalCard.transform.localPosition = Vector2.zero;

        cu.displayedUpgradedCard.transform.SetParent(cu.upgradedCardTransform);
        cu.displayedUpgradedCard.transform.localPosition = Vector2.zero;

        cu.displayedOriginalCard.clickable = false;
        cu.displayedUpgradedCard.clickable = false;

        return cu;
    }

    public void BuySuccessful()
    {
        Singleton.Instance.gameManager.playerDeck.RemoveCardPermanently(originalCard);
        displayedUpgradedCard.clickable = true;
        displayedUpgradedCard.transform.SetParent(null);
        Singleton.Instance.gameManager.playerDeck.AddCardPermanently(displayedUpgradedCard);

        Destroy(this.gameObject);
    }

    public float GetBasePrice()
    {
        return price;
    }

    public float GetFinalPrice()
    {
        float finalPrice = price;
        BuyableEvents.GetFinalPrice(this, ref finalPrice, price);

        return finalPrice;
    }

    public Vector2 GetLocalPriceTagPosition()
    {
        return new Vector2(0f, 5.75f);
    }

    void Update()
    {
        if (bc2d.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            /*if (Singleton.Instance.selectionHandler.currentlyHoveredCard != this && Singleton.Instance.selectionHandler.currentlyHeldCard == null)
            {
                hoverFeel.PlayFeedbacks();
            }

            cardHoveredEvent?.Invoke(this);*/

            if (Input.GetMouseButtonDown(0))
            {
                BuyableEvents.BuyableClicked(this);
            }
        }
    }
}
