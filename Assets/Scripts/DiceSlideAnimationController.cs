using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSlideAnimationController : DiceAnimation
{
   // public Dice mainDice;

    public RectTransform mainDiceRect;
    public RectTransform CloneDiceRect;

    public float rollSpeed;

    [ContextMenu("Play")]
    public void PlayRollAnimation()
    {
        //mainDiceRect.pivot = new Vector2(0, 0.5f);
        //CloneDiceRect.pivot = new Vector2(1, 0.5f);
        //mainDiceRect.transform.localScale = new Vector3(1, 1, 1);
        //CloneDiceRect.transform.localScale = new Vector3(0, 1, 1);
        StopAllCoroutines();
        StartCoroutine(PlayDiceRollAnimation());
    }

    private IEnumerator PlayDiceRollAnimation()
    {
        for(int i = 0; i < 2; i++)
        {
            yield return StartCoroutine(PlaySlideAnimation());
        }

        mainDice.gameObject.SetActive(true);
        mainDiceRect.pivot = new Vector2(0.5f, 0.5f);
        mainDiceRect.transform.localScale = Vector3.one;
        CloneDice.gameObject.SetActive(false);

        SetfinalCount();
    }

    IEnumerator PlaySlideAnimation()
    {
        float i = 0;
        mainDiceRect.pivot = new Vector2(0, 0.5f);
        CloneDiceRect.pivot = new Vector2(1, 0.5f);
        CloneDiceRect.offsetMin = Vector2.zero;
        CloneDiceRect.offsetMax = Vector2.zero;
        mainDiceRect.transform.localScale = new Vector3(1, 1, 1);
        CloneDiceRect.transform.localScale = new Vector3(0, 1, 1);
        CloneDice.gameObject.SetActive(true);
        CloneDice.Roll();
        Vector3 MDiceCurrentScale = mainDiceRect.transform.localScale;
        Vector3 CDiecCurrentScale = CloneDiceRect.transform.localScale;
        while (i < 1)
        {
            i += Time.deltaTime * rollSpeed;
            mainDiceRect.transform.localScale = Vector3.Lerp(MDiceCurrentScale, CDiecCurrentScale, i);
            CloneDiceRect.transform.localScale = Vector3.Lerp(CDiecCurrentScale, MDiceCurrentScale, i);
            yield return null;
        }
        mainDice.gameObject.SetActive(false);
        mainDiceRect.pivot = new Vector2(1, 0.5f);
        mainDiceRect.offsetMin = Vector2.zero;
        mainDiceRect.offsetMax = Vector2.zero;
        CloneDiceRect.pivot = new Vector2(0, 0.5f);
        mainDice.gameObject.SetActive(true);
        mainDice.Roll();
        i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * rollSpeed;
            mainDiceRect.transform.localScale = Vector3.Lerp(CDiecCurrentScale, MDiceCurrentScale, i);
            CloneDiceRect.transform.localScale = Vector3.Lerp(MDiceCurrentScale, CDiecCurrentScale, i);
            yield return null;
        }
        CloneDice.gameObject.SetActive(false);
        yield return null;
    }




}
