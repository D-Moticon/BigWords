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
    public HeroSO startingPlayer;
    
    public Actor actorPrefab;
    public Relic relicPrefab;
    public PriceTag priceTagPrefab;

    public DeckSO playerStartingDeck;
    public Transform playerDeckLocation;

    public float playerStartingCoins = 0f;
    public List<RelicSO> playerStartingRelics;
    public Transform relicLocation;
    public float relicSpacing = 1f;
    public float playerCoins = 0f;

    public Transform playerCenterLocation;
    public Transform enemyCenterLocation;
    

    [FoldoutGroup("Rules")]
    public float smartShuffleFactor = 0f;
    public int startingDiscards = 5;
    public float multAddPerCardCount = 0.2f;

    bool playButtonPressed = false;

    [FoldoutGroup("UI")]
    public TMP_Text discardsText;
    [FoldoutGroup("UI")]
    public TMP_Text cardCountMultiplierText;
    [FoldoutGroup("UI")]
    public TMP_Text playerCoinsText;

    [FoldoutGroup("SFX")]
    public SFXInfo[] cardCountMultiplierSFX;
    [FoldoutGroup("SFX")]
    public SFXInfo shuffleSFX;

    [FoldoutGroup("Prefabs")]
    public PooledObjectData coinPickup;

    [FoldoutGroup("UpdatedAtRuntime")]
    public Actor player;
    [FoldoutGroup("UpdatedAtRuntime")]
    public Deck playerDeck;
    [FoldoutGroup("UpdatedAtRuntime")]
    public Actor currentlyTargetedActor;
    [FoldoutGroup("UpdatedAtRuntime")]
    public Vector2 targetedActorPosition;
    [FoldoutGroup("UpdatedAtRuntime")]
    public List<Actor> enemies;
    [FoldoutGroup("UpdatedAtRuntime")]
    public int discardsRemaining;
    [FoldoutGroup("UpdatedAtRuntime")]
    public List<Relic> playerRelics;

    //When certain events happen (like entering the draw phase, doing damage), the game manager will send out this
    //list, which relics or cards can add their tasks to and will be performed before proceeding

    public delegate void TaskDelegate(ref List<IEnumerator> tasksToPerform);
    public static event TaskDelegate DrawPhaseEnteredEvent;
    public delegate void CardTaskDelegate(ref List<IEnumerator> tasksToPerform, Card c);
    public static event CardTaskDelegate CardTriggeredEvent;
    public delegate void EffectParamsDelegate(ref List<IEnumerator> tasksToPerform, EffectParams effectParams);
    public static event EffectParamsDelegate AttackCompletedEvent;
    public static event EffectParamsDelegate PostAttackDiscardCompletedEvent;

    public delegate void AddedFloatDelegate(float valueAdded, float totalValue);
    public static event AddedFloatDelegate CoinsAddedEvent;
    public static event AddedFloatDelegate CoinsSubtractedEvent;

    AttackInfo currentAttackInfo;
    List<Actor> doomedActors = new List<Actor>(); //actors to destroy at end of frame

    public class TaskParams
    {
        public Relic sourceRelic;
        public Relic targetRelic;
    }

    private void OnEnable()
    {
        Actor.ActorDiedEvent += ActorDiedListener;
        Relic.relicAddedEvent += RelicAddedListener;
        
    }

    private void OnDisable()
    {
        Actor.ActorDiedEvent -= ActorDiedListener;
        Relic.relicAddedEvent -= RelicAddedListener;
    }

    public void Start()
    {
        player = startingPlayer.CreateActorFromHeroSO(actorPrefab);
        player.transform.position = playerCenterLocation.position;

        playerDeck = Singleton.Instance.cardCreator.CreateDeckFromSO(playerStartingDeck);
        playerDeck.name = "PlayerDeck";
        playerDeck.transform.position = playerDeckLocation.position;
        playerDeck.SmartShuffle(smartShuffleFactor);

        states.Add(new DrawState());
        states.Add(new PlayState());
        states.Add(new ProcessAttackState());
        states.Add(new EnemyAttackState());

        AddCoinsToPlayer((int)playerStartingCoins);

        for (int i = 0; i < playerStartingRelics.Count; i++)
        {
            Relic r = Relic.CreateRelicFromSO(playerStartingRelics[i]);
            r.AddRelic();
        }

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

        /*for (int i = 0; i < doomedActors.Count; i++)
        {
            if (doomedActors != null)
            {
                print("DOOMED ACTOR KILLED");
                Destroy(doomedActors[i].gameObject);
            }
        }

        doomedActors.Clear();*/
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

            gameManager.cardCountMultiplierText.text = $"x1";

            if (gameManager.currentlyTargetedActor == null)
            {
                gameManager.TargetFirstValidEnemy();
            }
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
            gameManager.currentAttackInfo = new AttackInfo();

            Rack.TestResults rackTest = playerRack.TestRack();
            if (!rackTest.isValidWord)
            {
                print("NOT VALID WORD");
                yield break;
            }

            //Early Trigger Cards
            EffectParams effectParams = Singleton.Instance.gameManager.CreateEffectParamsFromBoardState();
            effectParams.phase = Phase.earlyCardActivate;
            for (int i = 0; i < effectParams.cardsOnRack.Count; i++)
            {
                Task cardTask = new Task(effectParams.cardsOnRack[i].ActivateEffects(effectParams));
                while (cardTask.Running)
                {
                    yield return null;
                }
            }

            //Trigger Cards
            Task triggerRackTask = new Task(TriggerRack(Phase.cardActivate, true));
            while (triggerRackTask.Running)
            {
                yield return null;
            }

            effectParams = Singleton.Instance.gameManager.CreateEffectParamsFromBoardState();
            effectParams.phase = Phase.postAttack;

            //Attack Completed Tasks
            List<IEnumerator> attackCompletedTasks = new List<IEnumerator>();
            AttackCompletedEvent?.Invoke(ref attackCompletedTasks, effectParams);
            for (int j = 0; j < attackCompletedTasks.Count; j++)
            {
                Task t = new Task(attackCompletedTasks[j]);
                while (t.Running)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.4f);

            List<Card> cardsOnRack = playerRack.GetRackCards();
            for (int i = 0; i < cardsOnRack.Count; i++)
            {
                cardsOnRack[i].MoveCardToDeck(discardPile);
                yield return new WaitForSeconds(0.1f);
            }

            //Post Discard Tasks
            effectParams = Singleton.Instance.gameManager.CreateEffectParamsFromBoardState();
            effectParams.phase = Phase.postDiscard;

            List<IEnumerator> discardCompletedTasks = new List<IEnumerator>();
            PostAttackDiscardCompletedEvent?.Invoke(ref discardCompletedTasks, effectParams);
            for (int j = 0; j < discardCompletedTasks.Count; j++)
            {
                Task t = new Task(discardCompletedTasks[j]);
                while (t.Running)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(1.0f);

            isComplete = true;
        }

        
    }

    public EffectParams CreateEffectParamsFromBoardState()
    {
        Rack.TestResults rackTest = playerRack.TestRack();

        EffectParams effectParams = new EffectParams();
        effectParams.source = player;
        effectParams.sourcePos = player.transform.position;
        effectParams.target = currentlyTargetedActor;
        if (currentlyTargetedActor != null)
        {
            effectParams.targetPos = currentlyTargetedActor.transform.position;
        }

        else
        {
            effectParams.targetPos = targetedActorPosition;
        }

        if (enemies != null)
        {
            effectParams.allActorsOnTargetTeam = enemies;
        }

        effectParams.cardsOnRack = rackTest.cards;
        effectParams.attackingWord = rackTest.word;
        effectParams.attackingWordChars = rackTest.wordChars;
        effectParams.attackingWordLetterCount = rackTest.cards.Count;

        return effectParams;
    }


    public class EnemyAttackState : State
    {
        public override void Enter()
        {
            gameManager.StartCoroutine(EnemyTurnCoroutine());
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            
        }

        IEnumerator EnemyTurnCoroutine()
        {
            for (int i = 0; i < gameManager.enemies.Count; i++)
            {
                Actor enemy = gameManager.enemies[i];

                if (enemy.enemyActions != null && enemy.enemyActions.Count > 0)
                {
                    Task t = new Task(enemy.enemyActions[enemy.currentActionIndex].ActionTask(enemy));
                    while (t.Running)
                    {
                        yield return null;
                    }

                    enemy.currentActionIndex++;

                    if (enemy.currentActionIndex > enemy.enemyActions.Count - 1)
                    {
                        enemy.currentActionIndex = 0;
                    }
                }
            }

            isComplete = true;
        }
    }

    public void ShufflePlayerHand()
    {
        playerHand.Shuffle();
        shuffleSFX.Play();
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
        playerCoinsText.text = Mathf.FloorToInt(playerCoins).ToString();
    }

    public static IEnumerator TriggerRack(Phase phase, bool runForward = true)
    {
        GameManager gameManager = Singleton.Instance.gameManager;
        EffectParams effectParams = Singleton.Instance.gameManager.CreateEffectParamsFromBoardState();
        effectParams.phase = phase;

        //Card Count Multiplier
        float cardCountMultiplier = 1f;
        gameManager.cardCountMultiplierText.text = "";

        for (int i = 0; i < effectParams.cardsOnRack.Count; i++)
        {
            cardCountMultiplier += gameManager.multAddPerCardCount;
            gameManager.cardCountMultiplierText.text = $"x{cardCountMultiplier}";
            gameManager.cardCountMultiplierText.GetComponentInChildren<MMF_Player>().PlayFeedbacks();
            effectParams.cardsOnRack[i].MultiplierCountFeel();

            if (i < gameManager.cardCountMultiplierSFX.Length)
            {
                gameManager.cardCountMultiplierSFX[i].Play();
            }

            else
            {
                gameManager.cardCountMultiplierSFX[gameManager.cardCountMultiplierSFX.Length - 1].Play();
            }

            yield return new WaitForSeconds(0.1f);
        }

        //Trigger Cards
        effectParams.cardCountMult = cardCountMultiplier;
        

        yield return new WaitForSeconds(0.5f);

        int start, end, step;
        int length = effectParams.cardsOnRack.Count;

        if (runForward)
        {
            start = 0;
            end = length;
            step = 1;
        }
        else
        {
            start = length - 1;
            end = -1;       // one less than the lowest index
            step = -1;
        }


        for (int i = start; i != end; i+=step)
        {
            Task cardTask = new Task(effectParams.cardsOnRack[i].ActivateEffects(effectParams));
            while (cardTask.Running)
            {
                yield return null;
            }

            //Call for any relics/cards that need to be activated on Card Trigger
            List<IEnumerator> cardTriggeredTasks = new List<IEnumerator>();
            CardTriggeredEvent?.Invoke(ref cardTriggeredTasks, effectParams.cardsOnRack[i]);

            for (int j = 0; j < cardTriggeredTasks.Count; j++)
            {
                Task t = new Task(cardTriggeredTasks[j]);
                while (t.Running)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ActorDiedListener(Actor actor, ref List<IEnumerator> tasksToPerform)
    {
        /*if (actor == currentlyTargetedActor)
        {
            TargetFirstValidEnemy();
        }*/

        if (enemies.Contains(actor))
        {
            enemies.Remove(actor);
        }

        Destroy(actor.gameObject);
        //doomedActors.Add(actor);
    }

    public void TargetFirstValidEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isDying)
            {
                continue;
            }

            Singleton.Instance.selectionHandler.TargetActor(enemies[i]);

            if (currentAttackInfo != null)
            {
                currentAttackInfo.target = currentlyTargetedActor;
                currentAttackInfo.targetPosition = currentlyTargetedActor.transform.position;
            }

            return;
        }

        currentlyTargetedActor = null;
    }

    public void AddCoinsToPlayer(float coinAmount)
    {
        playerCoins += coinAmount;
        CoinsAddedEvent?.Invoke(coinAmount, playerCoins);
    }

    public void SubtractCoinsFromPlayer(float coinAmount)
    {
        playerCoins -= coinAmount;
        CoinsSubtractedEvent?.Invoke(coinAmount, playerCoins);
    }

    public void RelicAddedListener(Relic r)
    {
        r.transform.SetParent(relicLocation);

        if (playerRelics == null)
        {
            playerRelics = new List<Relic>();
        }
        playerRelics.Add(r);
    }

    public List<Actor> GetLivingEnemies()
    {
        List<Actor> es = new List<Actor>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && !enemies[i].isDying)
            {
                es.Add(enemies[i]);
            }
        }

        return es;
    }
}
