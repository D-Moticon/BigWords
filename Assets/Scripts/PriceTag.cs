using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class PriceTag : MonoBehaviour
{
    public TMP_Text priceText;
    public Image priceTagFrame;

    public Color affordableColor = Color.green;
    public Color unaffordableColor = Color.red;

    IBuyable buyable;
    float storedPrice;
    public MMF_Player discountFeel;

    public void SetPrice(float price)
    {
        float playerCoins = Singleton.Instance.gameManager.playerCoins;

        // Choose color based on affordability
        if (playerCoins >= price)
        {
            priceText.color = Helpers.WithReferenceHue(priceText.color, affordableColor);
            priceTagFrame.color = affordableColor;
        }

        else
        {
            priceText.color = Helpers.WithReferenceHue(priceText.color, unaffordableColor);
            priceTagFrame.color = unaffordableColor;
        }

        // Apply the rich text with the selected color
        priceText.text = $"${Mathf.RoundToInt(price)}";
        storedPrice = price;
    }

    private void OnEnable()
    {
        BuyableEvents.BuyableBoughtEvent += BuyableBoughtListener;
        Relic.relicAddedEvent += RelicAddedListener;
        GameManager.CoinsChangedEvent += PlayerCoinsChangedListener;
    }

    private void OnDisable()
    {
        BuyableEvents.BuyableBoughtEvent -= BuyableBoughtListener;
        Relic.relicAddedEvent -= RelicAddedListener;
        GameManager.CoinsChangedEvent -= PlayerCoinsChangedListener;
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

    public void PlayerCoinsChangedListener(float newCoins)
    {
        SetPrice(storedPrice);
    }
}
