using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DiceCountSlider : MonoBehaviour
{
    public int minRange;
    public int maxRange;

    public int currentRange;

    public Slider slider;
    public TextMeshProUGUI rangeText;
    public CanvasGroup rangeUIGroup;

    public void OnEnable()
    {
        rangeUIGroup.interactable = true;
        StartCoroutine(PlayUiShowAnimation(0, 1));
        if (DiceManager.instance != null && DiceManager.instance.rollController != null)
        {
            currentRange = DiceManager.instance.rollController.spawnedDice.Count;
            if (slider != null)
            {
                slider.SetValueWithoutNotify(currentRange);
            }
            if (rangeText != null)
            {
                rangeText.text = currentRange.ToString();
            }
        }
    }

    IEnumerator PlayUiShowAnimation(int start, int end)
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 3;
            rangeUIGroup.alpha = Mathf.Lerp(start, end, i);
            yield return null;
        }
        if (start == 1)
        {
            this.gameObject.SetActive(false);
        }
    }
    public void OnHideBar()
    {
        rangeUIGroup.interactable = false;
        StartCoroutine(PlayUiShowAnimation(1, 0));
    }


    private void Start()
    {
        if (slider != null)
        {
            slider.minValue = minRange;
            slider.maxValue = maxRange;
            slider.wholeNumbers = true;
            slider.value = currentRange;

            // Add listener
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            // Initial update
            UpdateUI(slider.value);
        }
    }

    public void OnSliderValueChanged(float value)
    {
        UpdateUI(value);
    }

    private void UpdateUI(float value)
    {
        currentRange = Mathf.RoundToInt(value);
        if (rangeText != null)
        {
            rangeText.text = currentRange.ToString();
        }

        DiceManager.instance.rollController.SpawnAmount(currentRange);
    }
}
