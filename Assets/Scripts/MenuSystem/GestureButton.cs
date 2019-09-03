using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureButton : MenuButton
{
    private GestureCalibrationMenu gestureCalibrationMenu;
    private SpellHandler.SpellType type;

    public override void Press()
    {
        gestureCalibrationMenu.SetCurrentGesture(type);
    }

    public void Init(int buttonId, SpellHandler.SpellType type, GestureCalibrationMenu gestureCalibrationMenu)
    {
        this.gestureCalibrationMenu = gestureCalibrationMenu;
        this.buttonId = buttonId;
        this.type = type;
    }
}
