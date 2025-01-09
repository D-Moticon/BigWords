using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class Rack : MonoBehaviour
{
    public int slotNumber = 7;
    List<Slot> slots;
    public float slotSpacing = 1f;
    public Slot slotPrefab;

    [FoldoutGroup("SFX")]
    public SFXInfo slipCardsSFX;

    private void OnDrawGizmos()
    {
        Vector2 slotSize = slotPrefab.GetComponent<BoxCollider2D>().size;
        for (int i = 0; i < slotNumber; i++)
        {
            Gizmos.DrawWireCube(this.transform.position + new Vector3(i * slotSpacing, 0f, 0f), slotSize);
        }
    }

    private void Awake()
    {
        PopulateSlots();
    }

    void PopulateSlots()
    {
        slots = new List<Slot>();

        for (int i = 0; i < slotNumber; i++)
        {
            Slot s = Instantiate(slotPrefab, this.transform);
            s.transform.localPosition = new Vector3(i * slotSpacing, 0f, 0f);
            s.owningRack = this;
            slots.Add(s);
        }
    }

    public void MoveRackCardsToMakeRoom(Slot s, bool leftSide)
    {
        if (GetNumberOpenSlots() == 0)
        {
            return;
        }

        if (leftSide)
        {
            ShiftCardsRightToMakeRoom(s);
        }

        else
        {
            ShiftCardsLeftToMakeRoom(s);
        }

        slipCardsSFX.Play();
    }

    void ShiftCardsLeftToMakeRoom(Slot s)
    {
        int slotIndex = slots.IndexOf(s);

        int firstCardIndex = 0;
        for (int i = slotIndex; i >= 0; i--)
        {
            if (slots[i].GetCard() == null)
            {
                firstCardIndex = i + 1;
                break;
            }
        }

        if (firstCardIndex == 0)
        {
            ShiftCardsRightToMakeRoom(s);
            return;
        }

        for (int i = firstCardIndex; i <= slotIndex; i++)
        {
            if (slots[i].GetCard() != null)
            {
                slots[i].GetCard().MoveCardToSlot(slots[i - 1], 0.1f, false);
            }
        }
    }

    void ShiftCardsRightToMakeRoom(Slot s)
    {
        int slotIndex = slots.IndexOf(s);

        int lastCardIndex = slots.Count-1;
        for (int i = slotIndex; i < slots.Count; i++)
        {
            if (slots[i].GetCard() == null)
            {
                lastCardIndex = i - 1;
                break;
            }
        }

        if (lastCardIndex == slots.Count - 1)
        {
            ShiftCardsLeftToMakeRoom(s);
            return;
        }

        for (int i = lastCardIndex; i >= slotIndex; i--)
        {
            if (slots[i].GetCard() != null)
            {
                slots[i].GetCard().MoveCardToSlot(slots[i + 1], 0.1f, false);
            }
        }
    }

    public class TestResults
    {
        public bool isValidWord;
        public string word;
        public List<char> wordChars;
        public List<Card> cards;
    }

    public TestResults TestRack()
    {
        TestResults tr = new TestResults();

        bool validWord = false;

        string word = new string(GetRackLetters().ToArray()).ToLowerInvariant();

        if (WordChecker.CheckWord(word))
        {
            validWord = true;
        }

        else
        {
            validWord = false;
        }

        tr.isValidWord = validWord;
        tr.word = word;
        tr.wordChars = GetRackLetters();
        tr.cards = GetRackCards();

        return tr;
    }

    public void PrintDoesRackHaveValidWord()
    {
        bool valid = TestRack().isValidWord;
        print(valid);

    }

    public List<char> GetRackLetters()
    {
        List<char> chars = new List<char>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }

            Card t = slots[i].GetCard();

            if (t == null)
            {
                continue;
            }

            chars.Add(t.GetLetter());
        }

        return chars;
    }

    public List<Card> GetRackCards()
    {
        List<Card> cs = new List<Card>();
        for (int i = 0; i < slots.Count; i++)
        {
            Card c = slots[i].GetCard();
            if (c != null)
            {
                cs.Add(c);
            }
        }

        return cs;
    }

    public int GetNumberOpenSlots()
    {
        int openSlots = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetCard() == null)
            {
                openSlots++;
            }
        }

        return openSlots;
    }

    public void AddTileToEndOfRack(Card t)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                slots[i].SetCard(t);
            }
        }
    }

    public Slot GetFirstOpenSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetCard() == null)
            {
                return (slots[i]);
            }
        }

        print("Tried to get open slot but there were none");
        return null;
    }

    public int GetIndexOfSlot(Slot s)
    {
        return (slots.IndexOf(s));
    }

    public void Shuffle()
    {
        List<int> indexes = new List<int>();

        List<Card> cards = GetRackCards();

        for (int i = 0; i < cards.Count; i++)
        {
            indexes.Add(i);
        }

        indexes.Shuffle();

        for (int i = 0; i < cards.Count; i++)
        {
            int targetIndex = indexes[i];
            Slot s = slots[targetIndex];
            cards[i].MoveCardToSlot(s, 0.2f, false);
        }
    }
}
