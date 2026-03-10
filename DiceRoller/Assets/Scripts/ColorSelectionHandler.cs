using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectionHandler : MonoBehaviour
{
    public BGCOLOR currentSelectedColor;
    public CanvasGroup colorSelctionUIGroup;

    public void OnEnable()
    {
        colorSelctionUIGroup.interactable = true;
        currentSelectedColor = DiceColor.Instance.currentSelctedColor;
        StartCoroutine(PlayUiShowAnimation(0, 1));
    }

    IEnumerator PlayUiShowAnimation(int start, int end)
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 3;
            colorSelctionUIGroup.alpha = Mathf.Lerp(start, end, i);
            yield return null;
        }
        if (start == 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnHideBar()
    {
        colorSelctionUIGroup.interactable = false;
        StartCoroutine(PlayUiShowAnimation(1, 0));
    }
    public void OnColorSelcted(int color)
    {
        switch (color)
        {
            case 0:
                SetColor(BGCOLOR.RANDOM);
                break;
            case 1:
                SetColor(BGCOLOR.GREEN);
                break;
            case 2:
                SetColor(BGCOLOR.ORANGE);
                break;
            case 3:
                SetColor(BGCOLOR.PINK);
                break;
            case 4:
                SetColor(BGCOLOR.BLUE);
                break;
            case 5:
                SetColor(BGCOLOR.PURPAL);
                break;
            case 6:
                SetColor(BGCOLOR.YELLOW);
                break;
            default:
                break;

        }
    }

    public void SetColor(BGCOLOR color)
    {
        currentSelectedColor = color;
        DiceColor.Instance.currentSelctedColor = color;

        if (FirebaseAnalyticsManager.instance != null)
            FirebaseAnalyticsManager.instance.LogSettingChanged("dice_color", color.ToString());
    }
}
