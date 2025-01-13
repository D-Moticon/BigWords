using UnityEngine;

public abstract class State
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    public bool isComplete = false;

    public GameManager gameManager;
    public Deck playerDeck;
    public Rack playerRack;
    public Deck discardPile;
    public Actor player;

    public State()
    {
        gameManager = Singleton.Instance.gameManager;
        playerDeck = gameManager.playerDeck;
        playerRack = gameManager.playerRack;
        discardPile = gameManager.discardPile;
        player = gameManager.player;
    }
}
