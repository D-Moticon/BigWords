using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class PriceTag : MonoBehaviour
{
    public TMP_Text priceText;
    IBuyable buyable;
    float storedPrice;
    public MMF_Player discountFeel;

    public void SetPrice(float price)
    {
        priceText.text = $"${Mathf.RoundToInt(price)}";
        storedPrice = price;
    }

    private void OnEnable()
    {
        BuyableEvents.BuyableBoughtEvent += BuyableBoughtListener;
        Relic.relicAddedEvent += RelicAddedListener;
    }

    private void OnDisable()
    {
        BuyableEvents.BuyableBoughtEvent -= BuyableBoughtListener;
        Relic.relicAddedEvent -= RelicAddedListener;
    }

    public void BuyableBoughtListener(IBuyable ib, float price)
    {
        if (ib == buyable)
        {
            Destroy(this.gameObject);
        }

    }

    public static PriceTag AddPriceTagToBuyable(IBuyable ib, GameObject go)
    {
        PriceTag pt = Instantiate(Singleton.Instance.gameManager.priceTagPrefab, go.transform);
        pt.transform.localPosition = ib.GetLocalPriceTagPosition();
        pt.buyable = ib;
        pt.SetPrice(ib.GetFinalPrice());

        return pt;
    }

    public void RelicAddedListener(Relic r)
    {
        //Update price tag if prices were changed through a relic, etc.
        float p = buyable.GetFinalPrice();

        if (p < storedPrice)
        {
            float diff = Mathf.Abs(storedPrice - p);
            if (diff > 0.01f)
            {
                Singleton.Instance.uiManager.SpawnGenericFloater(priceText.transform.position, $"-${diff}");
                if (discountFeel != null)
                {
                    discountFeel.PlayFeedbacks();
                }
            }
        }

        SetPrice(p);
    }
}
