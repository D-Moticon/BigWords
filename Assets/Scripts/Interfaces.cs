using UnityEngine;

public interface IDamagable
{
    public void Damage(float damage);
}

public interface IBuyable
{
    public float GetBasePrice();
    public float GetFinalPrice();
    public void BuySuccessful();

    public Vector2 GetLocalPriceTagPosition();
}

public interface IBuyableSO
{
}

public static class BuyableEvents
{
    public delegate void RefFloatBuyableDelegate(IBuyable buyable, ref float price, float basePrice);
    public static event RefFloatBuyableDelegate GetFinalPriceEvent;
    public delegate void FloatBuyableDelegate(IBuyable buyable, float price);
    public static event FloatBuyableDelegate BuyableBoughtEvent;
    public delegate void EmptyDelegate();
    public delegate void BuyableDelegate(IBuyable buyable);
    public static event BuyableDelegate BuyableClickedEvent;

    public static void GetFinalPrice(IBuyable buyable, ref float price, float basePrice)
    {
        GetFinalPriceEvent?.Invoke(buyable, ref price, basePrice);
    }

    public static void BuyableBought(IBuyable buyable, float price)
    {
        BuyableBoughtEvent?.Invoke(buyable, price);
    }

    public static void BuyableClicked(IBuyable buyable)
    {
        BuyableClickedEvent?.Invoke(buyable);
    }
}
