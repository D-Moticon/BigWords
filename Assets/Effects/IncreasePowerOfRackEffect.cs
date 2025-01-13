using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class IncreasePowerOfRackEffect : Effect
{
    public float powerIncrease = 1f;
    public FXSO activationFX;
    public FXSO fxPerCard;

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        List<Card> cardsOnRack = effectParams.cardsOnRack;

        if (activationFX != null)
        {
            Task activationFXTask = new Task(activationFX.PlayFXSteps(effectParams.sourceCard.gameObject));
            while (activationFXTask.Running)
            {
                yield return null;
            }
        }

        for (int i = 0; i < cardsOnRack.Count; i++)
        {
            if (!cardsOnRack[i].GetHasPower())
            {
                continue;
            }

            Task perCardFXTask = new Task(fxPerCard.PlayFXSteps(effectParams.sourceCard.gameObject, cardsOnRack[i].gameObject));
            while (perCardFXTask.Running)
            {
                yield return null;
            }

            Task t = new Task(cardsOnRack[i].AddPower(powerIncrease));

            while (t.Running)
            {
                yield return null;
            }
        }
    }

    public override string GetEffectDescription()
    {
        return ($"All letters in rack gain +{powerIncrease} power until end of battle");
    }
}
