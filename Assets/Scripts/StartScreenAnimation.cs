using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartScreenAnimation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public ParticleSystem ps;
    public Image logo;
    public void Start()
    {
        ps.Play();
        StartCoroutine(ScaleLogo());
        StartCoroutine(PlayOutAnimation());
    }

    IEnumerator PlayOutAnimation()
    {
        yield return new WaitForSeconds(1f);
        ps.Stop();

        float i = 0;
        while(i < 1)
        {
            i += Time.deltaTime * 2f;
            canvasGroup.alpha = Mathf.Lerp(1, 0, i);
            yield return null;
        }
        ps.gameObject.SetActive(false);
    }


    public void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ScaleLogo()
    {
        float i = 0;
        while(i < 1)
        {
            i += Time.deltaTime * 0.2f;
            logo.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one *2.5f, i);
            yield return null;
        }
    }


}
