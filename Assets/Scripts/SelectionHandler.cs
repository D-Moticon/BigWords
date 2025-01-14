using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public enum Mode
    {
        play,
        shop
    }

    [HideInInspector]
    public Mode mode;

    public Card currentlyHeldCard;
    public Card currentlyHoveredCard;
    public Relic currentlyHoveredRelic;
    public Slot currentlyHoveredSlot;
    public Actor currentlyHoveredActor;
    public Actor currentlyTargetedActor;
    public IHoverable currentHoverable;

    private void OnEnable()
    {
        Card.cardHoveredEvent += CardHoveredListener;
        Card.cardDeHoveredEvent += CardDeHoveredListener;
        Card.cardClickedEvent += CardClickedListener;
        Relic.relicHoveredEvent += RelicHoveredListener;
        Relic.relicDeHoveredEvent += RelicDeHoveredListener;
        Relic.relicClickedEvent += RelicClickedListener;
        ShopState.ShopStateEnteredEvent += ShopStateEnteredListener;
    }

    private void OnDisable()
    {
        Card.cardHoveredEvent -= CardHoveredListener;
        Card.cardDeHoveredEvent -= CardDeHoveredListener;
        Card.cardClickedEvent -= CardClickedListener;
        Relic.relicHoveredEvent -= RelicHoveredListener;
        Relic.relicDeHoveredEvent -= RelicDeHoveredListener;
        Relic.relicClickedEvent -= RelicClickedListener;
        ShopState.ShopStateEnteredEvent -= ShopStateEnteredListener;
    }

    Vector2 mousePos;

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (mode)
        {
            case Mode.play:
                UpdatePlayMode();
                break;
            case Mode.shop:
                UpdateShopMode();
                break;
        }

        UpdateHoverable();
    }

    void UpdatePlayMode()
    {
        if (currentlyHeldCard != null && currentlyHoveredSlot != null && currentlyHoveredSlot.GetCard() != null)
        {
            bool leftSide = false;

            if (mousePos.x > currentlyHoveredSlot.transform.position.x)
            {
                leftSide = true;
            }

            currentlyHoveredSlot.owningRack.MoveRackCardsToMakeRoom(currentlyHoveredSlot, leftSide);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyHoveredActor != null)
            {
                Singleton.Instance.gameManager.TargetActor(currentlyHoveredActor);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentlyHeldCard != null && currentlyHoveredSlot != null && currentlyHoveredSlot.GetCard() == null)
            {
                currentlyHeldCard.MoveCardToSlot(currentlyHoveredSlot, 0.1f);
            }

            currentlyHeldCard = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentlyHoveredCard != null)
            {
                if (Singleton.Instance.gameManager.TryDiscard(currentlyHoveredCard))
                {
                    currentlyHoveredCard = null;
                }
            }
        }

        if (currentlyHeldCard != null)
        {
            currentlyHeldCard.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }
    }

    void UpdateShopMode()
    {

    }

    void UpdateHoverable()
    {
        if (currentlyHoveredCard != null)
        {
            currentHoverable = currentlyHoveredCard;
            return;
        }

        if (currentlyHoveredRelic != null)
        {
            currentHoverable = currentlyHoveredRelic;
            return;
        }

        currentHoverable = null;
    }

    public void SlotHovered(Slot slot)
    {
        currentlyHoveredSlot = slot;
    }

    public void CardHoveredListener(Card card)
    {
        currentlyHoveredCard = card;
    }

    public void CardDeHoveredListener(Card card)
    {
        if (card == currentlyHoveredCard)
        {
            currentlyHoveredCard = null;
        }
    }

    public void CardClickedListener(Card card)
    {
        if (mode == Mode.play)
        {
            currentlyHeldCard = card;
            currentlyHeldCard.RemoveFromSlot();
        }

    }

    public void RelicHoveredListener(Relic relic)
    {
        currentlyHoveredRelic = relic;
    }

    public void RelicDeHoveredListener(Relic relic)
    {
        if (relic == currentlyHoveredRelic)
        {
            currentlyHoveredRelic = null;
        }
    }

    public void RelicClickedListener(Relic relic)
    {

    }

    public void ActorHovered(Actor actor)
    {
        currentlyHoveredActor = actor;
    }

    public void ActorNotHovered(Actor actor)
    {
        if (actor == currentlyHoveredActor)
        {
            currentlyHoveredActor = null;
        }
    }

    void ShopStateEnteredListener()
    {
        mode = Mode.shop;
    }

}
