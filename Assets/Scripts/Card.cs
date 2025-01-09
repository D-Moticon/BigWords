using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private string cardName;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private Image cardImageRenderer;
    [SerializeField] private Vector2 cardSpriteSize = new Vector2(4f, 3f);
    [SerializeField] public char letter;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private Transform effectTextParent;
    [SerializeField] private TMP_Text effectTextPrefab;

    [SerializeReference]
    public List<CardEffect> cardEffects;

    Slot slotBeforePickup;
    Slot currentSlot;

    BoxCollider2D bc2d;

    Coroutine currentMoveCoroutine;

    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player simpleBumpFeel;
    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player hoverFeel;

    [FoldoutGroup("SFX")]
    public SFXInfo attachtoSlotSFX;

    public char GetLetter()
    {
        return letter;
    }

    public void SetLetter(char l)
    {
        letter = l;
        letterText.text = l.ToString();
    }

    public void SetSlot(Slot s)
    {
        currentSlot = s;
        s.SetCard(this);
    }

    public void SetCardName(string newName)
    {
        cardName = newName;
        cardNameText.text = newName;
    }

    public void SetCardImage(Sprite newIMG)
    {
        cardImageRenderer.sprite = newIMG;

        //Helpers.SetSpriteSize(cardImageRenderer, cardSpriteSize.x, cardSpriteSize.y);
    }

    public string GetCardName()
    {
        return cardName;
    }

    public void AddCardEffect(CardEffect cardEffect)
    {
        // effect is of type CardEffect, so we need the concrete type
        // If CardEffect is not a ScriptableObject, this can work directly:
        if (cardEffects == null)
        {
            cardEffects = new List<CardEffect>();
        }
        var copiedEffect = cardEffect.Clone();
        cardEffects.Add(copiedEffect);

        TMP_Text effectText = Instantiate(effectTextPrefab, effectTextParent);
        effectText.text = copiedEffect.GetEffectDescription();

        if (cardEffect.feelPlayerPrefab != null)
        {
            MMF_Player mmfPlayer = Instantiate(cardEffect.feelPlayerPrefab, this.transform);
            mmfPlayer.transform.localPosition = Vector3.zero;
            mmfPlayer.transform.localRotation = Quaternion.identity;
            copiedEffect.feelPlayer = mmfPlayer;
        }

        copiedEffect.owningCard = this;
    }

    public void ClearEffectTexts()
    {
        TMP_Text[] existingTexts = effectTextParent.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in existingTexts)
        {
            Destroy(t.gameObject);
        }
    }

    public void RemoveFromSlot()
    {
        if (currentSlot != null)
        {
            slotBeforePickup = currentSlot;
            currentSlot.RemoveCard();
        }

        currentSlot = null;
    }

    private void Awake()
    {
        letterText.text = letter.ToString();
        bc2d = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (bc2d.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Singleton.Instance.selectionHandler.currentlyHoveredCard != this)
            {
                hoverFeel.PlayFeedbacks();
            }

            Singleton.Instance.selectionHandler.CardHovered(this);
        }

        else
        {
            Singleton.Instance.selectionHandler.CardNotHovered(this);
        }
    }

    public void MoveCardLerp(Vector3 newPosition, float duration = 1f, Slot parentSlotAtEnd = null, Deck parentDeckAtEnd = null, bool playSFX = true)
    {
        transform.SetParent(null);

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(MoveCardCoroutine(newPosition, duration, parentSlotAtEnd, parentDeckAtEnd, playSFX));
    }

    public void MoveCardToSlot(Slot s, float duration = 1f, bool playSFX = true)
    {
        Vector3 pos = s.transform.position;

        if (s.card != null)
        {
            s.card.RemoveFromSlot();
        }

        s.SetCard(this);
        MoveCardLerp(pos, duration, s, null, playSFX);
    }

    public void MoveCardToDeck(Deck d, float duration = 1f, bool playSFX = true)
    {
        Vector3 pos = d.transform.position;

        d.AddCard(this);
        MoveCardLerp(pos, duration, null, d, playSFX);
    }

    public void SnapIntoSlot(Slot s, bool playSFX = true)
    {
        currentSlot = s;

        transform.parent = s.transform;
        transform.localPosition = Vector3.zero;

        if (playSFX)
        {
            attachtoSlotSFX.Play();
        }
    }

    public void SnapIntoDeck(Deck d, bool playSFX = true)
    {
        transform.parent = d.transform;
        transform.localPosition = Vector3.zero;

        if (playSFX)
        {
            attachtoSlotSFX.Play();
        }
    }

    IEnumerator MoveCardCoroutine(Vector3 newPosition, float duration = 1f, Slot parentSlotAtEnd = null, Deck parentDeckAtEnd = null, bool playSFX = true)
    {
        float elapsedTime = 0f;

        if (currentSlot != null)
        {
            currentSlot.SetCard(null);
        }

        if (parentSlotAtEnd != null)
        {
            parentSlotAtEnd.SetCard(this);
            SetSlot(parentSlotAtEnd);
        }

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(transform.position, newPosition, t);

            if (Vector3.Distance(transform.position, newPosition) < 0.05f)
            {
                elapsedTime = duration;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        if (parentSlotAtEnd != null)
        {
            SnapIntoSlot(parentSlotAtEnd, playSFX);
        }

        if (parentDeckAtEnd != null)
        {
            SnapIntoDeck(parentDeckAtEnd, playSFX);
        }
    }

    public void SimpleBumpFeel()
    {
        simpleBumpFeel.PlayFeedbacks();
    }

    public IEnumerator TriggerCard(AttackInfo attackInfo)
    {
        for (int j = 0; j < cardEffects.Count; j++)
        {
            Task effectTask = new Task(cardEffects[j].CardActivatedEffect(attackInfo));

            while (effectTask.Running)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
