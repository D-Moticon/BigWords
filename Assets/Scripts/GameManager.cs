using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        public bool isComplete = false;

        public GameManager gameManager;
        public Deck playerDeck;
        public Rack playerRack;
        public Actor player;

        public State()
        {
            gameManager = Singleton.Instance.gameManager;
            playerDeck = gameManager.playerDeck;
            playerRack = gameManager.playerRack;
            player = gameManager.player;
        }
    }

    private List<State> states = new List<State>();
    private int currentStateIndex = 0;
    public State currentState;
    public Rack playerHand;
    public Rack playerRack;
    public Actor player;
    public Actor testEnemy;
    public DeckSO playerStartingDeck;
    public Transform playerDeckLocation;
    Deck playerDeck;

    [FoldoutGroup("Rules")]
    public int drawsPerTurn = 7;

    public bool playButtonPressed = false;

    public void Start()
    {
        playerDeck = Singleton.Instance.cardCreator.CreateDeckFromSO(playerStartingDeck);
        playerDeck.name = "PlayerDeck";
        playerDeck.transform.position = playerDeckLocation.position;

        states.Add(new DrawState());
        states.Add(new PlayState());
        states.Add(new ProcessAttackState());

        if (states.Count > 0)
        {
            currentStateIndex = 0;
            currentState = states[currentStateIndex];
            currentState.Enter();
        }
    }

    private void Update()
    {
        if (currentState != null)
        {
            // 1) Run the current state’s Update
            currentState.Update();

            // 2) Check if it’s done
            if (currentState.isComplete)
            {
                // Exit the old state
                currentState.Exit();

                // Move to the next state
                currentStateIndex++;

                if (currentStateIndex > states.Count - 1)
                {
                    currentStateIndex = 0;
                }

                currentState = states[currentStateIndex];
                currentState.Enter();
            }
        }
    }

    public void PressPlayButton()
    {
        playButtonPressed = true;
    }

    private void LateUpdate()
    {
        playButtonPressed = false;
    }

    public class DrawState : State
    {
        public override void Enter()
        {
            print("Entering Draw Phase");
            gameManager.StartCoroutine(DrawCards());
        }

        public override void Exit()
        {
            print("Exiting Draw Phase");
        }

        public override void Update()
        {
            
        }

        IEnumerator DrawCards()
        {
            yield return new WaitForSeconds(0.1f);

            int drawCount = Mathf.Min(gameManager.drawsPerTurn, playerDeck.currentCards.Count);

            for (int i = 0; i < drawCount; i++)
            {
                gameManager.DrawCard();
                yield return new WaitForSeconds(0.1f);
            }

            isComplete = true;
        }
    }

    public void DrawCard()
    {
        Card c = playerDeck.TakeCardFromTop();
        Slot s = playerHand.GetFirstOpenSlot();

        if (c == null)
        {
            return;
        }

        if (s == null)
        {
            return;
        }

        c.MoveCardLerp(s.transform.position, 1f, s);
    }

    public class PlayState : State
    {
        public override void Enter()
        {
            print("Entering Play Phase");
        }

        public override void Exit()
        {
            print("Exiting Play Phase");
        }

        public override void Update()
        {
            if (gameManager.playButtonPressed)
            {
                Rack.TestResults rackTest = playerRack.TestRack();
                if (rackTest.isValidWord)
                {
                    isComplete = true;
                }

                else
                {
                    print("NOT VALID WORD");
                }
            }
        }
    }

    public class ProcessAttackState : State
    {
        public override void Enter()
        {
            gameManager.StartCoroutine(AttackProcessCoroutine());
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            
        }

        IEnumerator AttackProcessCoroutine()
        {
            AttackInfo attackInfo = new AttackInfo();

            Rack.TestResults rackTest = playerRack.TestRack();
            if (!rackTest.isValidWord)
            {
                print("NOT VALID WORD");
                yield break;
            }

            attackInfo.source = player;
            attackInfo.target = gameManager.testEnemy;
            attackInfo.cards = rackTest.cards;
            attackInfo.word = rackTest.word;

            for (int i = 0; i < attackInfo.cards.Count; i++)
            {
                List<CardEffect> cardEffects = attackInfo.cards[i].cardEffects;
                for (int j = 0; j < cardEffects.Count; j++)
                {
                    Task effectTask = new Task(cardEffects[j].CardActivatedEffect(attackInfo));

                    while (effectTask.Running)
                    {
                        yield return null;
                    }

                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(5f);
            isComplete = true;
        }

        
    }
}
