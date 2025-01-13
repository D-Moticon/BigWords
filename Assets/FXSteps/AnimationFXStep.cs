using System.Collections;
using UnityEngine;

public class AnimationFXStep : FXStep
{
    public Animator animatorPrefab;
    public string stateName;
    public Sprite replaceSprite;
    public FXTarget fxTarget;

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        Animator animator = Animator.Instantiate(animatorPrefab);

        if (replaceSprite != null)
        {
            SpriteRenderer sr = animator.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = replaceSprite;
            }
        }

        switch (fxTarget)
        {
            case FXTarget.source:
                animator.transform.position = source.transform.position;
                break;
            case FXTarget.target:
                animator.transform.position = target.transform.position;
                break;
        }

        animator.Play(stateName);

        while(!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        GameObject.Destroy(animator.gameObject);
    }
}
