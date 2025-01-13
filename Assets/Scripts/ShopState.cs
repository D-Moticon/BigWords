using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShopState : State
{
    public static event System.Action ShopStateEnteredEvent;

    public override void Enter()
    {
        Singleton.Instance.gameManager.MoveHandToDeck();
        Singleton.Instance.gameManager.MoveRackToDeck();
        Singleton.Instance.gameManager.MoveDiscardPileToDeckAndShuffle();

        //Ensure only one shop open
        // Verify Shop scene is loaded
        Scene shopScene = SceneManager.GetSceneByName("Shop");

        if (shopScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(shopScene);
        }

        SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive);

        GameManager.CloseShopButtonPressedEvent += ShopButtonClosedPressedListener;
        ShopStateEnteredEvent?.Invoke();
        
    }

    public override void Exit()
    {
        
        GameManager.CloseShopButtonPressedEvent -= ShopButtonClosedPressedListener;
        gameManager.currentEncounterIndex++;
    }

    public override void Update()
    {
        
    }

    IEnumerator UnloadShopSceneCoroutine()
    {
        // Verify Shop scene is loaded
        Scene shopScene = SceneManager.GetSceneByName("Shop");

        AsyncOperation closeShopOp = SceneManager.UnloadSceneAsync(shopScene);
        yield return closeShopOp;  // Wait for unload to complete

        Singleton.Instance.gameManager.ChangeState(new StartEncounterState());
    }

    void ShopButtonClosedPressedListener()
    {
        Singleton.Instance.gameManager.StartCoroutine(UnloadShopSceneCoroutine());
    }
}
