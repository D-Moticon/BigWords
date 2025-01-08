using UnityEngine;

public class CardHandler : MonoBehaviour
{
    Card currentlyHeldCard;
    Card currentlyHoveredTile;
    Slot currentlyHoveredSlot;


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
            if (currentlyHoveredTile != null)
            {
                currentlyHeldCard = currentlyHoveredTile;
                currentlyHeldCard.RemoveFromSlot();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentlyHeldCard != null && currentlyHoveredSlot != null)
            {
                currentlyHeldCard.MoveCardToSlot(currentlyHoveredSlot, 0.1f);
            }

            currentlyHeldCard = null;
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

    public void TileHovered(Card tile)
    {
        currentlyHoveredTile = tile;
    }

    public void TileNotHovered(Card tile)
    {
        if (tile == currentlyHoveredTile)
        {
            currentlyHoveredTile = null;
        }
    }
}
