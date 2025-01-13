using UnityEngine;
using System.Collections.Generic;

public class BoardState
{
    public Rack rack;
    public Rack playerHand;
    public List<Card> cardsOnRack;
    public string wordOnRack;
    public bool wordOnRackValid = false;
    public List<Actor> enemies;
    public List<Actor> allies;
    public Actor targetedActor;
    public List<Card> cardsInHand;
    public Deck playerDeck;
    public Deck discardPile;
}
