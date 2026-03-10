using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceHandler : DiceAnimation
{
    public DiceSlideAnimationController slideAnimation;
    public DiceSimpleRollAnimation simpleRollAnimation;


    public void PlayDiceRoll()
    {
        if(DiceManager.instance.currentAnimation == ANIM.SLIDE)
        {
            DiceRollWithslide();
        }
        else if(DiceManager.instance.currentAnimation == ANIM.SIMPLE)
        {
            DiceRollWithSimple();
        }
        else
        {
            RollDiceWithoutAnimation();
        }
    }

    public void DiceRollWithslide()
    {
        slideAnimation.PlayRollAnimation();
    }

    public void DiceRollWithSimple()
    {
        simpleRollAnimation.PlaySimpleDiceAnimation();
    }

    public void RollDiceWithoutAnimation()
    {
        StartCoroutine(RollDiceWiothoutAnim());
    }

    IEnumerator RollDiceWiothoutAnim()
    {
        yield return new WaitForSeconds(1f);
        mainDice.Roll();
        SetfinalCount();
    }

}
