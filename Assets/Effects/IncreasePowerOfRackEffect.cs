using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class IncreasePowerOfRackEffect : Effect
{
    public float powerIncrease = 1f;
    public SFXInfo powerIncreasePerCardSFX;

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        List<Card> cardsOnRack = effectParams.cardsOnRack;

        for (int i = 0; i < cardsOnRack.Count; i++)
        {
            if (!cardsOnRack[i].GetHasPower())
            {
                continue;
            }

            Task t = new Task(cardsOnRack[i].AddPower(powerIncrease));
            effectParams.sourceCard.StandardBumpFeel();
            powerIncreasePerCardSFX.Play();

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
