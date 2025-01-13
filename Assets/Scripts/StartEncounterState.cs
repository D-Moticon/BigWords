using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEncounterState : State
{
    public override void Enter()
    {
        Scene shopScene = SceneManager.GetSceneByName("Shop");

        if (shopScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(shopScene);
        }

        gameManager.CreateEncounter(gameManager.encounters[gameManager.currentEncounterIndex]);
        Singleton.Instance.selectionHandler.mode = SelectionHandler.Mode.play;
        gameManager.ChangeState(new GameManager.DrawState());
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        
    }
}
