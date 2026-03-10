using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Dice : MonoBehaviour
{
    public Image diceBg;
    public List<GameObject> diceImages;
    public TextMeshProUGUI diceCount;
    public int currentCount;
    public GameObject diceView;
    public GameObject border;

    private static HashSet<TMP_FontAsset> s_WarmedFonts = new HashSet<TMP_FontAsset>();

    [ContextMenu("test")]
    public void Roll()
    {
        Color newColor = DiceColor.Instance.GetRandomColor();
        int count = Random.Range(1, DiceManager.instance.rollController.maxDiceRange + 1);
        currentCount = count;
        SetDice(count, newColor);
    }

    public void OnEnable()
    {
        // Pre-warm the font with numbers to avoid race conditions in Unity 6 TextCore/UITK
        if (diceCount != null && diceCount.font != null && !s_WarmedFonts.Contains(diceCount.font))
        {
            diceCount.font.TryAddCharacters("0123456789");
            s_WarmedFonts.Add(diceCount.font);
        }

        Color newColor = Color.white;
        if (DiceColor.Instance != null)
        {
            newColor = DiceColor.Instance.GetRandomColor();
        }
        else
        {
        }
        int count = Random.Range(1, DiceManager.instance.rollController.maxDiceRange + 1);
        currentCount = count;
        SetDice(count, newColor);
    }

    public void SetScale(float value)
    {
        diceView.transform.localScale = Vector3.one * value;
       
        diceCount.transform.localScale = (Vector3.one * 2.3f) * value;
       
    }

    public void SetBorder(bool isEnable)
    {
        if (border != null)
        {
            border.SetActive(isEnable);
        }
    }

    public void SetDice(int count, Color Bgcolor)
    {
        // usage: dots if count is small AND we are not in "Only Number" mode
        bool showDots = (count < 10) && (!DiceManager.instance.rollController.isOnlyNumber);

        if (showDots)
        {
            // Show Dots, Hide Text
            SetDiceImage(count - 1); // maps 1->0, 2->1 ...
            diceCount.gameObject.SetActive(false);
        }
        else
        {
            // Show Text, Hide Dots
            diceCount.gameObject.SetActive(true);
            diceCount.text = count.ToString();
            SetDiceImage(-1); // Disable all dice images
        }
        diceBg.color = Bgcolor;
    }


    public void SetDiceImage(int count)
    {
        for (int i = 0; i < diceImages.Count; i++)
        {
            if (i == count)
            {
                diceImages[i].SetActive(true);
            }
            else
            {
                diceImages[i].SetActive(false);
            }
        }
    }
}
