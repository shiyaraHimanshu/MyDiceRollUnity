using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public List<CanvasGroup> settingButtonOptions;
    public Button settingButton;
    public bool isSettingOpen = false;
    public GameObject buttonRotaion;
    public Image soundImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    public Image vibrateImage;
    public Sprite vibrateOnSprite;
    public Sprite vibrateOffSprite;

    public void Start()
    {
        isSettingOpen = false;
        // Initialize buttons to hidden/scaled down state
        if (settingButtonOptions != null)
        {
            foreach (var option in settingButtonOptions)
            {
                if (option != null)
                {
                    option.alpha = 0;
                    option.transform.localScale = Vector3.zero;
                    option.gameObject.SetActive(false);
                }
            }
        }

        if (DiceManager.instance != null && DiceManager.instance.rollController != null && DiceManager.instance.rollController.diceRoll != null)
        {
            UpdateSoundButton(DiceManager.instance.rollController.diceRoll.mute);
        }

        // Initialize Vibration Button
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        UpdateVibrationButton(isVibrationOn);
    }

    public void OnSoundButtonClick()
    {
        if (DiceManager.instance != null && DiceManager.instance.rollController != null && DiceManager.instance.rollController.diceRoll != null)
        {
            bool isMuted = !DiceManager.instance.rollController.diceRoll.mute;
            DiceManager.instance.rollController.diceRoll.mute = isMuted;
            UpdateSoundButton(isMuted);

            if (FirebaseAnalyticsManager.instance != null)
                FirebaseAnalyticsManager.instance.LogSettingChanged("sound", isMuted ? "off" : "on");
        }
    }

    public void UpdateSoundButton(bool isMuted)
    {
        if (soundImage != null)
        {
            if (isMuted)
            {
                soundImage.sprite = soundOffSprite;
            }
            else
            {
                soundImage.sprite = soundOnSprite;
            }
        }
    }

    public void OnVibrationButtonClick()
    {
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateVibrationButton(isVibrationOn);

        if (FirebaseAnalyticsManager.instance != null)
            FirebaseAnalyticsManager.instance.LogSettingChanged("vibration", isVibrationOn ? "on" : "off");
    }

    public void UpdateVibrationButton(bool isVibrationOn)
    {
        if (vibrateImage != null)
        {
            if (isVibrationOn)
            {
                vibrateImage.sprite = vibrateOnSprite;
            }
            else
            {
                vibrateImage.sprite = vibrateOffSprite;
            }
        }
    }

    public RangeUiHandler RangeUiHandler;
    public void OnRangeButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenRangePanel();
        }
    }


    public void OnDiceCountClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenDiceCountPanel();
        }
    }


    public ColorSelectionHandler ColorSelectionHandler;

    public void OnColorButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenColorPanel();
        }
    }

    public DiceRollAnimationSelection AnimationSelectionHandler;

    public void OnAnimationSelectButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenAnimationPanel();
        }
    }

    public void OnSettingButtonClick()
    {
        if (settingButton != null && !settingButton.interactable) return;

        if(isSettingOpen)
        {
            isSettingOpen = false;
            
            if(DiceManager.instance != null)
            {
                DiceManager.instance.isSettingsOpen = false;
                DiceManager.instance.CloseAllPanelsExcept(null);
            }
            StartCoroutine(ShowButtons(1, 0));
        }
        else
        {
            isSettingOpen = true;
            if(DiceManager.instance != null)
            {
                DiceManager.instance.isSettingsOpen = true;
            }

            for (int i = 0; i < settingButtonOptions.Count; i++)
            {
                settingButtonOptions[i].gameObject.SetActive(false);
            }
            StartCoroutine(ShowButtons(0, 1));
            StartCoroutine(PlaySettingOpenRotation());
        }
    }

    IEnumerator PlaySettingOpenRotation()
    {
        if (buttonRotaion == null) yield break;

        // Snap immediately to maxAngle to start the animation from there
        targetRotationAngle = maxAngle;
        buttonRotaion.transform.localRotation = Quaternion.Euler(0, 0, maxAngle);

        float duration = 0.4f; // Faster, more energetic rotation
        float elapsed = 0f;

        // Rotate from maxAngle to minAngle
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Using ease-out for a snappy finish at minAngle
            float easedT = 1f - Mathf.Pow(1f - t, 3f); 
            targetRotationAngle = Mathf.Lerp(maxAngle, minAngle, easedT);
            yield return null;
        }

        targetRotationAngle = minAngle;
    }

    IEnumerator ShowButtons(int start, int end)
    {
        if (settingButton != null) settingButton.interactable = false;

        float delay = (end == 1) ? 0.05f : 0.03f; // Snappier sequence

        // Disable interaction on options during animation
        for (int i = 0; i < settingButtonOptions.Count; i++)
        {
            settingButtonOptions[i].interactable = false;
        }

        for (int i = 0; i < settingButtonOptions.Count; i++)
        {
            settingButtonOptions[i].gameObject.SetActive(true);
            StartCoroutine(PlayOptionOpenAnimation(start, end, settingButtonOptions[i]));
            yield return new WaitForSeconds(delay);
        }

        // Wait for the last animation to complete (approx 0.15s duration with speed 8)
        yield return new WaitForSeconds(0.2f);

        if (settingButton != null) settingButton.interactable = true;

        // If we opened the menu, enable interaction on options
        if (end == 1)
        {
            for (int i = 0; i < settingButtonOptions.Count; i++)
            {
                settingButtonOptions[i].interactable = true;
            }
        }
    }

    IEnumerator PlayOptionOpenAnimation(float start, float end, CanvasGroup buttonObject)
    {
        float progress = 0;
        float speed = (end > start) ? 8f : 10f; // Fast open, even faster close

        while (progress < 1)
        {
            progress += Time.deltaTime * speed;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);
            float value = Mathf.Lerp(start, end, easedProgress);

            buttonObject.transform.localScale = Vector3.one * value;
            buttonObject.alpha = value; // Keep alpha for a cleaner look

            yield return null;
        }

        buttonObject.transform.localScale = Vector3.one * end;
        buttonObject.alpha = end;

        if (end == 0)
        {
            buttonObject.gameObject.SetActive(false);
        }
    }

    
    [Header("Rotation Settings")]
    public float minAngle = -30f;
    public float maxAngle = 30f;
    public float rotationStep = 30f; 
    
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float targetRotationAngle = 0f;
    private float rotationSpeed = 10f; 

    void Update()
    {
        if (isSettingOpen)
        {
            HandleSwipe();
            RotateButton();
        }
    }

    private float lastProcessedY;

    void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastProcessedY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            float currentY = Input.mousePosition.y;
            float diff = currentY - lastProcessedY;
            float swipeThreshold = 50f;

            if (Mathf.Abs(diff) >= swipeThreshold)
            {
                int steps = (int)(diff / swipeThreshold); 
                
                // steps preserves sign. If diff is positive (Swipe Up), steps is positive.
                // We want Swipe Up -> Decrease Angle (based on previous logic)
                targetRotationAngle -= steps * rotationStep;
                
                targetRotationAngle = Mathf.Clamp(targetRotationAngle, minAngle, maxAngle);
                
                lastProcessedY += steps * swipeThreshold;
            }
        }
    }

    void DetectSwipe()
    {
        // Logic moved to HandleSwipe for continuous interaction
    }

    void RotateButton()
    {
        if(buttonRotaion != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
            buttonRotaion.transform.localRotation = Quaternion.Lerp(buttonRotaion.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

}
