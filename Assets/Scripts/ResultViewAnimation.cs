using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultViewAnimation : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public Animator anim;
    public string inanimation;

    public void PlayResultAnimation(int value)
    {
        resultText.text = value.ToString();
        anim.Play(inanimation);
    }
}
