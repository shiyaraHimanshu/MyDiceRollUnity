using UnityEngine;
using System.Collections.Generic;

public class DiceColor : MonoBehaviour
{
    public BGCOLOR currentSelctedColor;
    public List<Color> diceColors;

    public static DiceColor Instance;
    public void Awake()
    {
        Instance = this;
    }

    public Color GetRandomColor()
    {
        switch (currentSelctedColor)
        {
            case BGCOLOR.RANDOM:
                return diceColors[Random.Range(0, diceColors.Count)];
            case BGCOLOR.GREEN:
                return diceColors[0];
            case BGCOLOR.ORANGE:
                return diceColors[1];
            case BGCOLOR.PINK:
                return diceColors[2];
            case BGCOLOR.BLUE:
                return diceColors[3];
            case BGCOLOR.PURPAL:
                return diceColors[4];
            case BGCOLOR.YELLOW:
                return diceColors[5];
            default:
                return diceColors[0];
        }
    }
}

public enum BGCOLOR
{
    RANDOM,
    YELLOW,
    GREEN,
    BLUE,
    ORANGE,
    PINK,
    PURPAL
}
