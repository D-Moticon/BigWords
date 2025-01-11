using UnityEngine;

[CreateAssetMenu(fileName = "DiscountRelic", menuName = "Relics/DiscountRelic")]
public class DiscountRelic : RelicSO
{
    public enum DiscountType
    {
        percentOffBase,
        stackingMult,
        subtractDollars
    }

    public DiscountType discountType;

    public float discountAmount = .10f;

    public override void GetBuyablePrice(Relic relicInstance, IBuyable buyable, ref float price, float bPrice)
    {
        base.GetBuyablePrice(relicInstance, buyable, ref price, bPrice);

        switch (discountType)
        {
            case DiscountType.percentOffBase:
                float dollarsOff = Mathf.CeilToInt(bPrice * discountAmount);
                price -= dollarsOff;
                break;
            case DiscountType.stackingMult:
                price *= discountAmount;
                break;
            case DiscountType.subtractDollars:
                price -= discountAmount;
                break;
        }

        if (price < 0f)
        {
            price = 0f;
        }
    }
}
