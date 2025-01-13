using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Deck : MonoBehaviour
{
    public List<Card> permanentCards;
    public List<Card> currentCards;

    public void AddCardPermanently(Card card)
    {
        if (permanentCards == null)
        {
            permanentCards = new List<Card>();
        }

        if (currentCards == null)
        {
            currentCards = new List<Card>();
        }

        if (!permanentCards.Contains(card))
        {
            permanentCards.Add(card);
        }

        if (!currentCards.Contains(card))
        {
            currentCards.Add(card);
        }

        card.MoveCardToDeck(this, 1f, true);
    }

    public void AddCardTemporarily(Card card)
    {
        if (currentCards == null)
        {
            currentCards = new List<Card>();
        }

        if (!currentCards.Contains(card))
        {
            currentCards.Add(card);
        }
    }

    public void SnapCardToDeck(Card card)
    {
        card.transform.parent = this.transform;
        card.transform.localPosition = Vector3.zero;
    }

    public void MoveCardOffDeck(Card card)
    {
        card.transform.SetParent(null);
        currentCards.Remove(card);
    }

    public Card TakeCardFromTop()
    {

        //using for to prevent drawing nulls
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i] != null)
            {
                Card c = currentCards[i];
                MoveCardOffDeck(c);
                return c;
            }
        }

        print("Tried to take card from top but no card existed in deck");

        return null;
    }

    public void ShuffleDeck()
    {
        currentCards.Shuffle();
    }

    public void SmartShuffle(float alternationFactor)
    {
        // Clamp factor to [0, 1]
        alternationFactor = Mathf.Clamp01(alternationFactor);

        // If factor is very close to 0, we can just do a normal random shuffle and return
        if (Mathf.Approximately(alternationFactor, 0f))
        {
            currentCards.Shuffle();
            return;
        }

        // Separate cards into vowels and consonants
        List<Card> vowels = currentCards.Where(c => IsVowel(c.letter)).ToList();
        List<Card> consonants = currentCards.Where(c => !IsVowel(c.letter)).ToList();

        // Shuffle each subgroup
        vowels.Shuffle();
        consonants.Shuffle();

        var finalDeck = new List<Card>(currentCards.Count);

        // Randomly decide if we start by *trying* to pick a vowel or consonant
        bool lastWasVowel = Random.value > 0.5f;

        // While we still have cards in either list, choose next card
        while (vowels.Count > 0 || consonants.Count > 0)
        {
            // Probability check: if "wantAlternate" is true, 
            // we try to pick the "other type" from whatever we used last time.
            bool wantAlternate = Random.value < alternationFactor;

            bool pickVowel;
            if (lastWasVowel)
            {
                // If we want to alternate, pick a consonant; otherwise pick a vowel
                pickVowel = !wantAlternate;
            }
            else
            {
                // If we want to alternate, pick a vowel; otherwise pick a consonant
                pickVowel = wantAlternate;
            }

            if (pickVowel)
            {
                if (vowels.Count > 0)
                {
                    // Take from the end for O(1) removal (or pick from front if you prefer)
                    var card = vowels[vowels.Count - 1];
                    vowels.RemoveAt(vowels.Count - 1);
                    finalDeck.Add(card);
                    lastWasVowel = true;
                }
                else
                {
                    // No vowels left, must pick a consonant
                    if (consonants.Count > 0)
                    {
                        var card = consonants[consonants.Count - 1];
                        consonants.RemoveAt(consonants.Count - 1);
                        finalDeck.Add(card);
                        lastWasVowel = false;
                    }
                    else
                    {
                        // Nothing left in either list
                        break;
                    }
                }
            }
            else
            {
                // We want a consonant
                if (consonants.Count > 0)
                {
                    var card = consonants[consonants.Count - 1];
                    consonants.RemoveAt(consonants.Count - 1);
                    finalDeck.Add(card);
                    lastWasVowel = false;
                }
                else
                {
                    // No consonants left, must pick a vowel
                    if (vowels.Count > 0)
                    {
                        var card = vowels[vowels.Count - 1];
                        vowels.RemoveAt(vowels.Count - 1);
                        finalDeck.Add(card);
                        lastWasVowel = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // If one list still has leftovers, just dump them in random order at the end
        // (already shuffled individually, so no extra re-shuffle needed)
        while (vowels.Count > 0)
        {
            var card = vowels[vowels.Count - 1];
            vowels.RemoveAt(vowels.Count - 1);
            finalDeck.Add(card);
        }
        while (consonants.Count > 0)
        {
            var card = consonants[consonants.Count - 1];
            consonants.RemoveAt(consonants.Count - 1);
            finalDeck.Add(card);
        }

        // Now we have our "smart shuffled" deck
        currentCards = finalDeck;
    }

    private bool IsVowel(char c)
    {
        c = char.ToUpperInvariant(c);
        return (c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U');
    }

    public void MoveCardsToOtherDeck(Deck d)
    {
        List<Card> cs = currentCards;

        for (int i = currentCards.Count - 1; i >= 0; i--)
        {
            currentCards[i].MoveCardToDeck(d);
            MoveCardOffDeck(currentCards[i]);
        }

    }
}
