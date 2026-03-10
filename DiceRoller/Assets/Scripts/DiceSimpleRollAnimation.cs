using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSimpleRollAnimation : DiceAnimation
{
    public float speed;
  //  public Dice mainDice;
    public void PlaySimpleDiceAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PlaySimpleRoll());
    }

    IEnumerator PlaySimpleRoll()
    {
        for(int i = 0; i < 5;i++)
        {
            mainDice.Roll();
            yield return new WaitForSeconds(speed);
        }
        SetfinalCount();
    }
}
