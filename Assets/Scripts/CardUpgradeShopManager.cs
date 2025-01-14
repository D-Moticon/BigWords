using UnityEngine;
using System.Collections.Generic;

public class CardUpgradeShopManager : ShopManager
{
    protected override void PopulateBuyables()
    {
        IBuyable[] existingBuyables = buyablesParent.GetComponentsInChildren<IBuyable>();
        for (int i = 0; i < existingBuyables.Length; i++)
        {
            MonoBehaviour mb = existingBuyables[i] as MonoBehaviour;
            if (mb != null)
            {
                Destroy(mb.gameObject);
            }
        }

        instantiatedBuyables = new List<IBuyable>();

        List<Card> cardsToUpgrade = Singleton.Instance.gameManager.playerDeck.GetRandomUpgradeableCardsFromPermanentDeck(quantity);

        for (int i = 0; i < cardsToUpgrade.Count; i++)
        {
            CardUpgrade cu = CardUpgrade.GenerateCardUpgradeFromCard(cardsToUpgrade[i]);

            cu.transform.SetParent(buyablesParent);
            instantiatedBuyables.Add(cu);
            PriceTag.AddPriceTagToBuyable(cu, cu.gameObject);
        }

    }
}
