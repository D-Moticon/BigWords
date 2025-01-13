using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class PlayerCoinsUI : MonoBehaviour
{
    public TMP_Text coinsText;
    float targetCoins;
    float displayedCoins;

    public MMF_Player coinsAddedFeel;

    public float coinAddPerSecond = 10f;

    private void OnEnable()
    {
        GameManager.CoinsAddedEvent += CoinsAddedListener;
        GameManager.CoinsSubtractedEvent += CoinsSubtractedListener;

        targetCoins = Singleton.Instance.gameManager.playerCoins; //for when a new scene loads in, etc
    }

    private void OnDisable()
    {
        GameManager.CoinsAddedEvent -= CoinsAddedListener;
        GameManager.CoinsSubtractedEvent -= CoinsSubtractedListener;
    }

    void CoinsAddedListener(float coinsAdded, float totalCoins)
    {
        targetCoins = totalCoins;
        coinsAddedFeel.PlayFeedbacks();
    }

    void CoinsSubtractedListener(float coinsSubtracted, float totalCoins)
    {
        targetCoins = totalCoins;
        coinsAddedFeel.PlayFeedbacks();
    }

    private void Update()
    {
        if (Mathf.Abs(displayedCoins - targetCoins) < 1f)
        {
            displayedCoins = targetCoins;
        }

        else if (displayedCoins > targetCoins)
        {
            displayedCoins -= Time.deltaTime * coinAddPerSecond;
        }

        else if (displayedCoins < targetCoins)
        {
            displayedCoins += Time.deltaTime * coinAddPerSecond;
        }

        coinsText.text = $"{Mathf.Floor(displayedCoins)}";
    }
}
