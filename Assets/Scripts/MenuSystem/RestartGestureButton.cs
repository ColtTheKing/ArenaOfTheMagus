using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGestureButton : MenuButton
{
    private GestureCalibrationMenu gestureCalibrationMenu;

    public override void Press()
    {
        gestureCalibrationMenu.RestartGesture();
    }

    public void Init(int buttonId, GestureCalibrationMenu gestureCalibrationMenu)
    {
        this.buttonId = buttonId;
        this.gestureCalibrationMenu = gestureCalibrationMenu;
    }
}
