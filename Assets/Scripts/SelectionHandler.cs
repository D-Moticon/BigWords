using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public Card currentlyHeldCard;
    public Card currentlyHoveredCard;
    Slot currentlyHoveredSlot;
    Actor currentlyHoveredActor;
    Actor currentlyTargetedActor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
            if (currentlyHoveredCard != null)
            {
                currentlyHeldCard = currentlyHoveredCard;
                currentlyHeldCard.RemoveFromSlot();
            }

            if (currentlyHoveredActor != null)
            {
                TargetActor(currentlyHoveredActor);
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

    public void SlotHovered(Slot slot)
    {
        currentlyHoveredSlot = slot;
    }

    public void CardHovered(Card card)
    {
        currentlyHoveredCard = card;
    }

    public void CardNotHovered(Card card)
    {
        if (card == currentlyHoveredCard)
        {
            currentlyHoveredCard = null;
        }
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

    public void TargetActor(Actor actor)
    {
        if (actor != currentlyTargetedActor)
        {
            if (currentlyTargetedActor != null)
            {
                currentlyTargetedActor.DeSelectActor();
            }

            currentlyTargetedActor = actor;
        }

        actor.SelectActor();
        Singleton.Instance.gameManager.currentlyTargetedActor = actor;
    }
}
