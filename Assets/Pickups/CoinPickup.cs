using UnityEngine;

public class CoinPickup : Pickup
{
    public int coinAmount = 1;
    public override void AddPickupToPlayer()
    {
        Singleton.Instance.gameManager.AddCoinsToPlayer(coinAmount);
    }

    
}
