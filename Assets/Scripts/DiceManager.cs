using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    public DiceRollController rollController;
    public ANIM currentAnimation;
    public Image dotAndNumber;
    public CanvasGroup uiGroup;

    public Sprite dot;
    public Sprite num;

    public void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0; // Disable VSync to ensure targetFrameRate works
    }

    public void Start()
    {
        UpdateDiceTypeUI();
        SetDiceRange(6);
        SetDiceAnimation(ANIM.SLIDE);
        OnDiceRoll();
    }
    
    public DiceCountSlider diceCountSlider;
    public RangeUiHandler rangeUiHandler;
    public ColorSelectionHandler colorSelectionHandler;
    public DiceRollAnimationSelection animationSelectionHandler;

    public void CloseAllPanelsExcept(MonoBehaviour activePanel)
    {
        if (rangeUiHandler != null && rangeUiHandler != activePanel && rangeUiHandler.gameObject.activeInHierarchy)
        {
            rangeUiHandler.OnHideBar();
        }
        if (colorSelectionHandler != null && colorSelectionHandler != activePanel && colorSelectionHandler.gameObject.activeInHierarchy)
        {
            colorSelectionHandler.OnHideBar();
        }
        if (animationSelectionHandler != null && animationSelectionHandler != activePanel && animationSelectionHandler.gameObject.activeInHierarchy)
        {
            animationSelectionHandler.OnHideBar();
        }
        if (diceCountSlider != null && diceCountSlider != activePanel && diceCountSlider.gameObject.activeInHierarchy)
        {
            diceCountSlider.OnHideBar();
        }
    }

    public void OpenRangePanel()
    {
        CloseAllPanelsExcept(rangeUiHandler);
        if (rangeUiHandler != null && !rangeUiHandler.gameObject.activeInHierarchy)
        {
            rangeUiHandler.gameObject.SetActive(true);
        }
    }

    public void OpenDiceCountPanel()
    {
        CloseAllPanelsExcept(diceCountSlider);
        if (diceCountSlider != null && !diceCountSlider.gameObject.activeInHierarchy)
        {
            diceCountSlider.gameObject.SetActive(true);
        }
    }

    public void OpenColorPanel()
    {
        CloseAllPanelsExcept(colorSelectionHandler);
        if (colorSelectionHandler != null && !colorSelectionHandler.gameObject.activeInHierarchy)
        {
            colorSelectionHandler.gameObject.SetActive(true);
        }
    }

    public void OpenAnimationPanel()
    {
        CloseAllPanelsExcept(animationSelectionHandler);
        if (animationSelectionHandler != null && !animationSelectionHandler.gameObject.activeInHierarchy)
        {
            animationSelectionHandler.gameObject.SetActive(true);
        }
    }

    public void AddDice()
    {
        rollController.AddDice();
        if (diceCountSlider != null && diceCountSlider.gameObject.activeInHierarchy)
        {
            diceCountSlider.slider.SetValueWithoutNotify(rollController.spawnedDice.Count);
            diceCountSlider.rangeText.text = rollController.spawnedDice.Count.ToString();
        }
    }
    public void RemoveDice()
    {
        rollController.RemoveDice();
        if (diceCountSlider != null && diceCountSlider.gameObject.activeInHierarchy)
        {
            diceCountSlider.slider.SetValueWithoutNotify(rollController.spawnedDice.Count);
            diceCountSlider.rangeText.text = rollController.spawnedDice.Count.ToString();
        }
    }
    public bool isSettingsOpen = false;

    public void OnDiceRoll()
    {
        if(!rollController.isDiceRolling)
        {
            HideUI();
            rollController.PlayRoll();
        }
    }

    public void SetDiceRange(int maxRange)
    {
        rollController.maxDiceRange= maxRange;
    }

    public void SetDiceOnlyNumaric()
    {
        rollController.SetDotOrNumber();
        UpdateDiceTypeUI();
    }

    public void UpdateDiceTypeUI()
    {
        if (!rollController.isOnlyNumber)
        {
            dotAndNumber.sprite = dot;
        }
        else
        {
            dotAndNumber.sprite = num;
        }
    }

    public void SetDiceAnimation(ANIM animation)
    {
        currentAnimation = animation;
    }

    public void HideUI()
    {
        CloseAllPanelsExcept(null);
        uiGroup.interactable = false;
        StartCoroutine(SetUIAlpha(1,0));
    }

    public void ShowUI()
    {
        uiGroup.interactable = true;
        StartCoroutine(SetUIAlpha(0,1));
    }

    IEnumerator SetUIAlpha(int Startvalue, int endValue)
    {
        float i = 0;
        while(i < 1)
        {
            i += Time.deltaTime * 5;
            uiGroup.alpha = Mathf.Lerp(Startvalue, endValue, i);
            yield return null;  
        }
    }

    public void VibrateMobile()
    {
        // Check if vibration is enabled in settings
        if (PlayerPrefs.GetInt("Vibration", 1) == 0) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                    {
                        if (vibrator.Call<bool>("hasVibrator"))
                        {
                            vibrator.Call("vibrate", 500L); // 500 milliseconds
                        }
                    }
                }
            }
        }
        catch (System.Exception)
        {
            Handheld.Vibrate(); // Fallback
        }
#else
        // Fallback for testing in editor or other platforms if needed, though usually minimal
        // Handheld.Vibrate(); 
#endif
    }

}

public enum ANIM
{
    SLIDE,
    SIMPLE,
    NONE
}
