using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public AnimationCurve animShape;
    public float speed;

    private Vector3 defaultScale;

    public void Start()
    {
        defaultScale = Vector3.one;
    }

    public void OnPress()
    {
        this.gameObject.transform.localScale = defaultScale;
        StopAllCoroutines();
        StartCoroutine(PlayPressAnimation());
    }

    IEnumerator PlayPressAnimation()
    {
        float  i = 0;
        float scalevalue = 0;
        while(i  <1)
        {
            i += Time.deltaTime * speed;

            scalevalue = animShape.Evaluate(i);
            this.gameObject.transform.localScale = Vector3.one*scalevalue;
            yield return null;
        }
        this.gameObject.transform.localScale = defaultScale;
    }
}
