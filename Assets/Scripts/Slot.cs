using UnityEngine;

public class Slot : MonoBehaviour
{
    BoxCollider2D bc2d;

    public Card card;
    [HideInInspector]public Rack owningRack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bc2d.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            Singleton.Instance.selectionHandler.SlotHovered(this);
        }
    }

    public void SetCard(Card c)
    {
        card = c;
    }

    public void SetCardNull()
    {
        card = null;
    }

    public void RemoveCard()
    {
        if (card != null)
        {
            card.transform.SetParent(null);
        }

        card = null;
    }

    public Card GetCard()
    {
        if (card != null)
        {
            return card;
        }

        return null;
    }

    public int GetSlotIndex()
    {
        if (owningRack != null)
        {
            return (owningRack.GetIndexOfSlot(this));
        }

        else
        {
            return 0;
        }
    }
}
