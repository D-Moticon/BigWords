using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBuyable, IHoverable
{
    [SerializeField] private string cardName;
    [SerializeField] private TMP_Text cardNameText;
    private string cardDescription;
    public CardSO cardSO;
    private CardType cardType;
    [SerializeField] private Image cardImageRenderer;
    [SerializeField] private Vector2 cardSpriteSize = new Vector2(4f, 3f);
    [SerializeField] public char letter;
    [SerializeField] private float power = 1f;
    [SerializeField] private bool hasPower = false;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private GameObject powerParent;
    [SerializeField] private Transform descriptionTextParent;
    [SerializeField] private TMP_Text descriptionText;
    public List<Effect> cardEffects;

    Slot slotBeforePickup;
    Slot currentSlot;

    BoxCollider2D bc2d;

    Coroutine currentMoveCoroutine;

    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player multiplierCountFeel;
    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player hoverFeel;
    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player powerFeel;
    [FoldoutGroup("Feels")]
    [SerializeField] private MMF_Player standardBumpFeel;

    [FoldoutGroup("SFX")]
    public SFXInfo attachtoSlotSFX;

    public delegate void CardDelegate(Card c);
    public static event CardDelegate cardHoveredEvent;
    public static event CardDelegate cardDeHoveredEvent;
    public static event CardDelegate cardClickedEvent;

    public float basePrice = 0f;
    public bool clickable = true;

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

    public void SetCardDescription(string desc)
    {
        cardDescription = desc;
        descriptionText.text = desc;
    }

    public string GetCardDescription()
    {
        return (cardDescription);
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

    /*public void AddCardEffect(CardEffect cardEffect)
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
    }*/

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
            if (Singleton.Instance.selectionHandler.currentlyHoveredCard != this && Singleton.Instance.selectionHandler.currentlyHeldCard == null)
            {
                hoverFeel.PlayFeedbacks();
            }

            cardHoveredEvent?.Invoke(this);

            if (Input.GetMouseButtonDown(0))
            {
                CardClicked();
            }
        }

        else
        {
            cardDeHoveredEvent?.Invoke(this);
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

        d.AddCardTemporarily(this);
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

    public void MultiplierCountFeel()
    {
        multiplierCountFeel.PlayFeedbacks();
    }

    public void StandardBumpFeel()
    {
        standardBumpFeel.PlayFeedbacks();
    }

    public IEnumerator ActivateEffects(EffectParams effectParams)
    {
        EffectParams eParams = effectParams.Copy();
        eParams.sourceCard = this;

        for (int j = 0; j < cardEffects.Count; j++)
        {
            if(cardEffects[j].activationPhase != effectParams.phase)
            {
                continue;
            }

            Task effectTask = new Task(cardEffects[j].GetEffectTask(eParams));

            while (effectTask.Running)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public float GetPower()
    {
        return power;
    }

    public string GetPowerString()
    {
        return (Helpers.RoundToDecimal(power, 1).ToString());
    }

    public void SetPower(float newPower)
    {
        power = newPower;
        powerText.text = GetPowerString();
    }

    public IEnumerator AddPower(float powerAdd)
    {
        power += powerAdd;
        powerText.text = GetPowerString();
        powerFeel.PlayFeedbacks();

        Singleton.Instance.uiManager.SpawnGenericFloater(powerText.transform.position, $"+{Helpers.RoundToDecimal(powerAdd,1)}");

        yield break;
    }

    public void SetHasPower(bool val)
    {
        hasPower = val;

        if (val)
        {
            powerParent.SetActive(true);
        }

        else
        {
            powerParent.SetActive(false);
        }
    }

    public bool GetHasPower()
    {
        return hasPower;
    }

    public void SetCardType(CardType newType)
    {
        cardType = newType;
        TypeColorize[] tcs = GetComponentsInChildren<TypeColorize>();
        foreach (TypeColorize tc in tcs)
        {
            tc.ColorizeHue(newType.cardTypeColor);
        }
    }

    public void CardClicked()
    {
        if (!clickable)
        {
            return;
        }

        cardClickedEvent?.Invoke(this);
        BuyableEvents.BuyableClicked(this);
    }

    public float GetBasePrice()
    {
        return basePrice;
    }

    public float GetFinalPrice()
    {
        float finalPrice = basePrice;
        BuyableEvents.GetFinalPrice(this, ref finalPrice, basePrice);

        return finalPrice;
    }

    public void BuySuccessful()
    {
        Singleton.Instance.gameManager.playerDeck.AddCardPermanently(this);
    }

    public static Card CreateCardFromSO(CardSO cso, char letter = 'A', float power = 1f)
    {
        return(Singleton.Instance.cardCreator.CreateCardFromSO(cso, letter, power));
    }

    public Vector2 GetLocalPriceTagPosition()
    {
        return new Vector2(0f, 3.0f);
    }

    public Card GetPowerUpgradedCard(float powerGain)
    {
        if (!hasPower)
        {
            return null;
        }

        Card c = Singleton.Instance.cardCreator.CopyCard(this);
        c.SetPower(this.power + powerGain);
        return c;
    }

    public Card GetCardUpgradedCard()
    {
        if (cardSO.upgradeCards.Count == 0)
        {
            return null;
        }

        int rand = Random.Range(0, cardSO.upgradeCards.Count);
        CardSO cso = cardSO.upgradeCards[rand];
        Card c = Singleton.Instance.cardCreator.CreateCardFromSOWithTraitsOfExistingCard(this, cso);
        return c;
    }

    public Card GetRandomCardUpgrade(float powerGain, out float price)
    {
        List<Card> possibleUpgrades = new List<Card>();
        List<float> prices = new List<float>();

        // Reference to LetterPicker if needed for pricing logic (not used here)
        // LetterPicker letterPicker = FindObjectOfType<LetterPicker>();

        // Try the power upgrade
        Card powerUpgrade = GetPowerUpgradedCard(powerGain);
        if (powerUpgrade != null)
        {
            possibleUpgrades.Add(powerUpgrade);
            // Example pricing logic for power upgrade
            prices.Add(powerGain * 5f); // For example, price scales with powerGain
        }

        // Try the card-based upgrade
        Card cardUpgrade = GetCardUpgradedCard();
        if (cardUpgrade != null)
        {
            possibleUpgrades.Add(cardUpgrade);
            // Example pricing logic for card upgrade
            prices.Add(cardUpgrade.cardSO.basePrice); // Fixed price for a card-based upgrade, adjust as necessary
        }

        // Add additional upgrade function calls here as needed
        // ...

        // If no upgrades available, set price to 0 and return null
        if (possibleUpgrades.Count == 0)
        {
            price = 0f;
            return null;
        }

        // Randomly select one of the possible upgrades
        int randomIndex = Random.Range(0, possibleUpgrades.Count);
        Card chosenCard = possibleUpgrades[randomIndex];
        price = prices[randomIndex];  // Set output price based on selected upgrade

        // Destroy all non-selected cards
        for (int i = 0; i < possibleUpgrades.Count; i++)
        {
            if (i != randomIndex && possibleUpgrades[i] != null)
            {
                Destroy(possibleUpgrades[i].gameObject);
            }
        }

        return chosenCard;
    }

    public void DestroyCard()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveCard();
        }
        Destroy(this.gameObject);
    }

    public string GetHoverableName()
    {
        return GetCardName();
    }

    public string GetHoverableDescription()
    {
        return GetCardDescription();
    }

    public string GetHoverableType()
    {
        if (cardType == null)
        {
            return "";
        }

        return (cardType.cardTypeName);
    }

    public Sprite GetHoverableSprite()
    {
        return (cardImageRenderer.sprite);
    }

    public Color GetHoverableColor()
    {
        if (cardType == null)
        {
            return Color.white;
        }

        return (cardType.cardTypeColor);
    }

    public Vector2 GetHoverUIOffset()
    {
        return new Vector2(0f, 6.5f);
    }
}
