using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public PriceTag priceTagPrefab;

    public BuyableCollection buyableCollection;
    public int quantity = 3;
    List<IBuyable> instantiatedBuyables;
    public Transform buyablesParent;

    private void OnEnable()
    {
        PopulateShop();
        BuyableEvents.BuyableClickedEvent += BuyableClickedListener;
    }

    private void OnDisable()
    {
        BuyableEvents.BuyableClickedEvent -= BuyableClickedListener;
    }

    void PopulateShop()
    {
        PopulateBuyables();
    }

    void PopulateBuyables()
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

        List<IBuyableSO> excludeList = new List<IBuyableSO>();

        //Exclude already owned relics
        for (int i = 0; i < Singleton.Instance.gameManager.playerRelics.Count; i++)
        {
            excludeList.Add(Singleton.Instance.gameManager.playerRelics[i].relicSO);
        }

        List<IBuyableSO> selectedBuyableSOs = Helpers.GetRandomElements(buyableCollection.GetBuyableSOs(), quantity, excludeList);

        instantiatedBuyables = new List<IBuyable>();

        for (int i = 0; i < selectedBuyableSOs.Count; i++)
        {
            IBuyable buyable = null;

            if (selectedBuyableSOs[i] is RelicSO)
            {
                Relic r = Relic.CreateRelicFromSO(selectedBuyableSOs[i] as RelicSO);
                buyable = r;
                r.transform.SetParent(buyablesParent);
                instantiatedBuyables.Add(r);
            }

            if (selectedBuyableSOs[i] is CardSO)
            {
                LetterData letterData = Singleton.Instance.letterPicker.GetRandomLetter();

                Card c = Card.CreateCardFromSO(selectedBuyableSOs[i] as CardSO, letterData.letterChar, letterData.score);
                buyable = c;
                c.transform.SetParent(buyablesParent);
                instantiatedBuyables.Add(c);
            }


            PriceTag.AddPriceTagToBuyable(buyable, (buyable as MonoBehaviour).gameObject);
        }
    }

    void BuyableClickedListener(IBuyable b)
    {
        if (instantiatedBuyables.Contains(b))
        {
            if (TryBuy(b))
            {
                instantiatedBuyables.Remove(b);
            }
        }
    }

    bool TryBuy(IBuyable buyable)
    {
        float playerCoins = Singleton.Instance.gameManager.playerCoins;
        float price = buyable.GetFinalPrice();

        if (playerCoins >= price)
        {
            buyable.BuySuccessful();
            BuyableEvents.BuyableBought(buyable, price);
            Singleton.Instance.gameManager.SubtractCoinsFromPlayer(price);
            return true;
        }

        return false;
    }
}
