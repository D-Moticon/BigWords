using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PalindromeRelic", menuName = "Relics/Palindrome")]
public class PalindromeRelic : RelicSO
{
    public override void AttackCompleted(Relic relicInstance, ref List<IEnumerator> tasksToPerform, EffectParams effectParams)
    {
        base.AttackCompleted(relicInstance, ref tasksToPerform, effectParams);

        if (IsPalindrome(effectParams.attackingWordChars))
        {
            PlayFeelAndSFX();
            Debug.Log($"{relicName} activated on palindrome {effectParams.attackingWord}");
            tasksToPerform.Add(GameManager.TriggerRack(Phase.cardActivate, false));
        }
    }


    public bool IsPalindrome(List<char> chars)
    {
        if (chars == null || chars.Count == 0 || chars.Count == 1)
            return false; // You could decide whether an empty list is a palindrome

        int left = 0;
        int right = chars.Count - 1;

        while (left < right)
        {
            if (chars[left] != chars[right])
                return false;

            left++;
            right--;
        }

        return true;
    }
    
}
