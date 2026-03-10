using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceAnimation : MonoBehaviour
{
    public Dice mainDice;
    public Dice CloneDice;

    public void SetfinalCount()
    {
        DiceManager.instance.rollController.GetFinalCount();
    }
}
