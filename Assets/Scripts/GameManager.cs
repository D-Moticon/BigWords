using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using MoreMountains.Feedbacks;

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

    private List<State> states = new List<State>();
    private int currentStateIndex = 0;
    public State currentState;
    public EnemyEncounterSO currentEncounter;
    public Rack playerHand;
    public Rack playerRack;
    public Deck discardPile;
    public Actor player;
    public Actor actorPrefab;
    public DeckSO playerStartingDeck;
    public Transform playerDeckLocation;
    public Transform enemyCenterLocation;
    Deck playerDeck;

    [FoldoutGroup("Rules")]
    public float smartShuffleFactor = 0f;
    public int startingDiscards = 5;
    public float multAddPerCardCount = 0.2f;

    bool playButtonPressed = false;

    [FoldoutGroup("UI")]
    public TMP_Text discardsText;
    [FoldoutGroup("UI")]
    public TMP_Text cardCountMultiplierText;

    [FoldoutGroup("SFX")]
    public SFXInfo cardCountMultiplierSFX;

    [FoldoutGroup("UpdatedAtRuntime")]
    public Actor currentlyTargetedActor;
    [FoldoutGroup("UpdatedAtRuntime")]
    public List<Actor> enemies;
    [FoldoutGroup("UpdatedAtRuntime")]
    public int discardsRemaining;

    public void Start()
    {
        playerDeck = Singleton.Instance.cardCreator.CreateDeckFromSO(playerStartingDeck);
        playerDeck.name = "PlayerDeck";
        playerDeck.transform.position = playerDeckLocation.position;
        playerDeck.SmartShuffle(smartShuffleFactor);

        states.Add(new DrawState());
        states.Add(new PlayState());
        states.Add(new ProcessAttackState());

        CreateEncounter(currentEncounter);

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
                currentState.isComplete = false;
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

    public void CreateEncounter(EnemyEncounterSO enemyEncounterSO)
    {
        if (enemies != null)
        {
            foreach (Actor a in enemies)
            {
                Destroy(a.gameObject);
            }
        }

        enemies = new List<Actor>();

        for (int i = 0; i < enemyEncounterSO.enemyInfos.Count; i++)
        {
            EnemyEncounterSO.EnemyInfo ei = enemyEncounterSO.enemyInfos[i];

            Actor enemy = ei.enemy.CreateEnemyFromSO();
            enemy.transform.position = enemyCenterLocation.position + (Vector3)ei.position;
            enemy.SetHealthAndMaxHealth(ei.startingHP);
            enemies.Add(enemy);
        }

        discardsRemaining = startingDiscards;
        UpdateUI();
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

            int drawCount = Mathf.Min(gameManager.playerHand.GetNumberOpenSlots(), playerDeck.currentCards.Count);

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

        c.MoveCardToSlot(s, 1f);
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
            if (gameManager.currentlyTargetedActor == null)
            {
                if (gameManager.enemies == null || gameManager.enemies.Count == 0)
                {
                    Singleton.Instance.selectionHandler.TargetActor(player);
                }

                else
                {
                    Singleton.Instance.selectionHandler.TargetActor(gameManager.enemies[0]);
                }
                
                return;
            }

            if (gameManager.playButtonPressed)
            {
                Rack.TestResults rackTest = playerRack.TestRack();

                if (!rackTest.isValidWord)
                {
                    print("NOT VALID WORD");
                    return;
                }

                isComplete = true;
                
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
            attackInfo.target = gameManager.currentlyTargetedActor;
            attackInfo.cards = rackTest.cards;
            attackInfo.word = rackTest.word;
            attackInfo.cardCount = rackTest.cards.Count;

            //Card Count Multiplier
            float cardCountMultiplier = 1f;
            gameManager.cardCountMultiplierText.text = "";
            for (int i = 0; i < attackInfo.cards.Count; i++)
            {
                cardCountMultiplier += gameManager.multAddPerCardCount;
                gameManager.cardCountMultiplierText.text = $"x{cardCountMultiplier}";
                gameManager.cardCountMultiplierText.GetComponentInChildren<MMF_Player>().PlayFeedbacks();
                attackInfo.cards[i].SimpleBumpFeel();
                gameManager.cardCountMultiplierSFX.Play();

                yield return new WaitForSeconds(0.1f);
            }

            attackInfo.cardCountMultiplier = cardCountMultiplier;

            yield return new WaitForSeconds(0.5f);

            //Card Effects
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

            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < attackInfo.cards.Count; i++)
            {
                attackInfo.cards[i].MoveCardToDeck(discardPile);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.5f);

            isComplete = true;
        }

        
    }

    public void ShufflePlayerHand()
    {
        playerHand.Shuffle();
    }

    public bool TryDiscard(Card c)
    {
        if (discardsRemaining > 0)
        {
            c.MoveCardToDeck(discardPile);
            discardsRemaining--;
            DrawCard();
            UpdateUI();
            return true;
        }

        return false;
    }

    void UpdateUI()
    {
        discardsText.text = discardsRemaining.ToString();
    }
}
