using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using MoreMountains.Feedbacks;

public class Card : MonoBehaviour
{
    [SerializeField] private string cardName;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private char letter;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private Transform effectTextParent;
    [SerializeField] private TMP_Text effectTextPrefab;

    [SerializeReference]
    public List<CardEffect> cardEffects;

    Slot currentSlot;

    BoxCollider2D bc2d;

    Coroutine currentMoveCoroutine;

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
            currentSlot.RemoveTile();
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
            Singleton.Instance.tileHandler.TileHovered(this);
        }

        else
        {
            Singleton.Instance.tileHandler.TileNotHovered(this);
        }
    }

    public void MoveCardLerp(Vector3 newPosition, float duration = 1f, Slot parentSlotAtEnd = null)
    {
        transform.SetParent(null);

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(MoveCardCoroutine(newPosition, duration, parentSlotAtEnd));
    }

    public void MoveCardToSlot(Slot s, float duration = 1f)
    {
        Vector3 pos = s.transform.position;

        MoveCardLerp(pos, duration, s);
    }

    public void SnapIntoSlot(Slot s)
    {
        currentSlot = s;
        transform.parent = s.transform;
        transform.localPosition = Vector3.zero;
    }

    IEnumerator MoveCardCoroutine(Vector3 newPosition, float duration = 1f, Slot parentSlotAtEnd = null)
    {
        float elapsedTime = 0f;

        if (currentSlot != null)
        {
            currentSlot.SetCard(null);
        }

        parentSlotAtEnd.SetCard(this);
        SetSlot(parentSlotAtEnd);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(transform.position, newPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        if (parentSlotAtEnd != null)
        {
            SnapIntoSlot(parentSlotAtEnd);
        }
    }
}
