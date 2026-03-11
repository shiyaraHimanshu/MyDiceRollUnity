using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DiceRollController : MonoBehaviour
{
    public int finalCount;
    [Header("Settings")]
    [SerializeField] private DiceHandler dicePrefab;
    [SerializeField] private RectTransform diceContainer;
    [SerializeField] private int minDice = 1;
    [SerializeField] private int maxDice = 12;
    public ResultViewAnimation viewAnimation;
    public int maxDiceRange = 6;
    public bool isOnlyNumber = false;
    public bool isBorderEnable = false;
    public bool isDiceRolling;
    public AudioSource diceRoll;
    public TextMeshProUGUI total;
    private int _rollCount = 0;
    private int _nextAdRollThreshold;
    private int _diceFinishedCount;


    [Header("Layout")]
    [SerializeField] private float spacing = 5f;

    public List<DiceHandler> spawnedDice = new List<DiceHandler>();
    private RectTransform _containerRect;

    private void Awake()
    {
        _containerRect = diceContainer;
        if (_containerRect == null)
            _containerRect = GetComponent<RectTransform>();
    }

    public Slider diceCountSlider;

    private void Start()
    {
        // Initialize with default or slider value
        int initialCount = 1;
        _nextAdRollThreshold = Random.Range(10, 16);

        if (diceCountSlider != null)
        {
            // Set slider min/max based on controller settings
            diceCountSlider.minValue = minDice;
            diceCountSlider.maxValue = maxDice;
            diceCountSlider.wholeNumbers = true;
            
            // Set initial slider value
            diceCountSlider.value = initialCount;
            
            // Add listener
            diceCountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        UpdateDiceCount(initialCount);
        GetFinalCount();
    }

    public void OnSliderValueChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        if (spawnedDice.Count != count)
        {
            UpdateDiceCount(count);
            // Recalculate scale based on new count
             SetScale(count);
        }
    }

    private Vector2 _lastSize;

    private void Update()
    {
        // Only update grid if screen/container size changes
        if (spawnedDice.Count > 0 && _containerRect != null)
        {
            Vector2 currentSize = _containerRect.rect.size;
            if (currentSize != _lastSize)
            {
                UpdateGrid();
                _lastSize = currentSize;
            }
        }
    }

    [ContextMenu("roll")]
    public void PlayRoll()
    {
        if(!isDiceRolling)
        {
            isDiceRolling = true;
            diceRoll.Play();
            for (int i = 0; i < spawnedDice.Count; i++)
            {
                spawnedDice[i].PlayDiceRoll();
            }

            // Ad Logic: Increment counter once per roll action
            _rollCount++;
            _diceFinishedCount = 0;

            if (FirebaseAnalyticsManager.instance != null)
                FirebaseAnalyticsManager.instance.LogEvent("dice_roll_start", "dice_count", spawnedDice.Count);
        }
       
    }

    public void SetDotOrNumber()
    {
        isOnlyNumber = !isOnlyNumber;
    }

    public void SetBorder()
    {
        isBorderEnable = !isBorderEnable;
        for (int i = 0; i < spawnedDice.Count; i++)
        {
            spawnedDice[i].mainDice.SetBorder(isBorderEnable);
            spawnedDice[i].CloneDice.SetBorder(isBorderEnable);
        }
    }

    public void GetFinalCount()
    {
        int totalcount = 0;
        for (int i = 0; i < spawnedDice.Count; i++)
        {
           totalcount += spawnedDice[i].mainDice.currentCount;
        }
        finalCount = totalcount;

        // If dice are rolling, wait for all to finish before processing side effects
        if (isDiceRolling)
        {
            _diceFinishedCount++;
            if (_diceFinishedCount < spawnedDice.Count)
            {
                return;
            }
        }

        DiceManager.instance.ShowUI();
        isDiceRolling = false;
        total.text = finalCount.ToString();
        
        if(DiceManager.instance != null)
        {
             DiceManager.instance.VibrateMobile();
        }

        if(spawnedDice.Count > 1)
        {
            viewAnimation.PlayResultAnimation(finalCount);
        }

        if (FirebaseAnalyticsManager.instance != null)
        {
            int diceRange = maxDiceRange;
            string animType = DiceManager.instance != null ? DiceManager.instance.currentAnimation.ToString() : "Unknown";
            string diceColor = DiceColor.Instance != null ? DiceColor.Instance.currentSelctedColor.ToString() : "Unknown";
            bool soundOn = diceRoll != null ? !diceRoll.mute : false;
            bool vibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;

            FirebaseAnalyticsManager.instance.LogDiceRoll(
                spawnedDice.Count, 
                finalCount, 
                diceRange, 
                animType, 
                diceColor, 
                soundOn, 
                vibrationOn
            );
        }

        // Ad Logic: Check counter and show ad if reached random threshold (10-15)
        if (_rollCount >= _nextAdRollThreshold)
        {
            _rollCount = 0;
            _nextAdRollThreshold = Random.Range(10, 16);

            if (GoogleAdsManager.instance != null)
            {
                GoogleAdsManager.instance.ShowInterstitialAd();
            }
        }
    }



    #region ScreenHandler

    [ContextMenu("Add Dice")]
    public void AddDice()
    {
        if (spawnedDice.Count < maxDice)
        {
            UpdateDiceCount(spawnedDice.Count + 1);
            SetScale(spawnedDice.Count);
        }
    }

    

    [ContextMenu("Remove Dice")]
    public void RemoveDice()
    {
        if (spawnedDice.Count > minDice)
        {
            UpdateDiceCount(spawnedDice.Count - 1);
            SetScale(spawnedDice.Count);
        }
    }

    public void SpawnAmount(int count)
    {
        UpdateDiceCount(count);
        SetScale(count);
    }


    private void SetScale(float input)
    {


        float t = Mathf.InverseLerp(3f, 25f, input);


        for (int i = 0; i < spawnedDice.Count; i++)
        {
            spawnedDice[i].mainDice.SetScale(Mathf.Lerp(0.7f, 0.3f, t));
            spawnedDice[i].CloneDice.SetScale(Mathf.Lerp(0.7f, 0.3f, t));

        }
    }

    public void UpdateDiceCount(int targetCount)
    {
        targetCount = Mathf.Clamp(targetCount, minDice, maxDice);

        // Sync slider if different to prevent loop
        if (diceCountSlider != null && (int)diceCountSlider.value != targetCount)
        {
            diceCountSlider.value = targetCount;
        }

        // Add needed
        while (spawnedDice.Count < targetCount)
        {
            DiceHandler newDice = Instantiate(dicePrefab, diceContainer);
            newDice.gameObject.SetActive(true);
            newDice.mainDice.gameObject.SetActive(true);
            newDice.CloneDice.gameObject.SetActive(false);
            newDice.mainDice.SetBorder(isBorderEnable);
            newDice.CloneDice.SetBorder(isBorderEnable);
            spawnedDice.Add(newDice);
        }

        // Remove excess
        while (spawnedDice.Count > targetCount)
        {
            DiceHandler diceToRemove = spawnedDice[spawnedDice.Count - 1];
            spawnedDice.RemoveAt(spawnedDice.Count - 1);
            Destroy(diceToRemove.gameObject);
        }

        UpdateGrid();
        SetScale(1);
    }

    public void RollAll()
    {
        foreach (var dice in spawnedDice)
        {
            // Trigger roll logic if exposed, or toggle enable to re-roll
            dice.gameObject.SetActive(true); 
        }
    }

    private void UpdateGrid()
    {
        if (spawnedDice.Count == 0 || _containerRect == null) return;

        int count = spawnedDice.Count;
        float width = _containerRect.rect.width;
        float height = _containerRect.rect.height;

        // 1. Calculate optimal standard column count to minimize deviation from squareness
        int bestCols = 1;
        float bestDev = float.MaxValue;

        for (int cols = 1; cols <= count; cols++)
        {
            int rows = Mathf.CeilToInt((float)count / cols);
            
            // Calculate theoretical cell size if grid was uniform
            float cellW = (width - (spacing * (cols - 1))) / cols;
            float cellH = (height - (spacing * (rows - 1))) / rows;

            if (cellW <= 0 || cellH <= 0) continue;

            float ratio = cellW / cellH;
            float deviation = Mathf.Abs(1f - ratio);

            if (deviation < bestDev)
            {
                bestDev = deviation;
                bestCols = cols;
            }
        }

        int finalCols = bestCols;
        int finalRows = Mathf.CeilToInt((float)count / finalCols);
        
        // Calculate row height (uniform for all rows)
        float rowHeight = (height - (spacing * (finalRows - 1))) / finalRows;

        int currentDiceIndex = 0;

        for (int r = 0; r < finalRows; r++)
        {
            // Determine how many items fit in this specific row
            // Usually 'finalCols', but limited by remaining count for the last row
            int itemsInRow = finalCols;
            int remaining = count - currentDiceIndex;
            if (remaining < finalCols)
                itemsInRow = remaining;

            if (itemsInRow <= 0) break;

            // Calculate width for items IN THIS ROW (stretches to fill space)
            float rowItemWidth = (width - (spacing * (itemsInRow - 1))) / itemsInRow;

            for (int c = 0; c < itemsInRow; c++)
            {
                if (currentDiceIndex >= count) break;

                DiceHandler dice = spawnedDice[currentDiceIndex];
                RectTransform rt = dice.transform as RectTransform;

                // Configure anchors to Top-Left
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);

                float xPos = c * (rowItemWidth + spacing);
                float yPos = -r * (rowHeight + spacing);

                rt.sizeDelta = new Vector2(rowItemWidth, rowHeight);
                rt.anchoredPosition = new Vector2(xPos, yPos);

                currentDiceIndex++;
            }
        }
    }
    #endregion
}
