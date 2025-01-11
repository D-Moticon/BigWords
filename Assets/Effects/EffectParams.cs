using UnityEngine;
using System.Collections.Generic;

public class EffectParams
{
    public Phase phase;
    public Actor source;
    public Vector2 sourcePos;
    public Actor target;
    public List<Actor> allActorsOnTargetTeam;
    public Vector2 targetPos;
    public Card sourceCard;
    public Card targetCard;
    public List<Card> cardsOnRack;
    public string attackingWord;
    public List<char> attackingWordChars;
    public int attackingWordLetterCount;
    public float cardCountMult = 1f;
    public bool targetKilled = false;
    public bool overrideDamage = false;
    public float damageOverride = 0f;

    public EffectParams Copy()
    {
        EffectParams eParams = new EffectParams();
        eParams.phase = phase;
        eParams.source = source;
        eParams.sourcePos = sourcePos;
        eParams.target = target;
        if (allActorsOnTargetTeam != null)
        {
            eParams.allActorsOnTargetTeam = allActorsOnTargetTeam;
        }
        eParams.targetPos = targetPos;
        eParams.sourceCard = sourceCard;
        eParams.targetCard = targetCard;
        if (cardsOnRack != null)
        {
            eParams.cardsOnRack = new List<Card>(cardsOnRack);
        }
        eParams.attackingWord = attackingWord;
        if (attackingWordChars != null)
        {
            eParams.attackingWordChars = new List<char>(attackingWordChars);
        }
        
        eParams.attackingWordLetterCount = attackingWordLetterCount;
        eParams.cardCountMult = cardCountMult;
        eParams.targetKilled = targetKilled;
        eParams.overrideDamage = overrideDamage;
        eParams.damageOverride = damageOverride;

        return eParams;
    }
}
