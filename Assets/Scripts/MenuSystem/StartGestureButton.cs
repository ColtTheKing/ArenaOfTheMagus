using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGestureButton : MenuButton
{
    private GestureCalibrationMenu gestureCalibrationMenu;

    public override void Press()
    {
        gestureCalibrationMenu.StartGesture();
    }

    public void Init(int buttonId, GestureCalibrationMenu gestureCalibrationMenu)
    {
        this.buttonId = buttonId;
        this.gestureCalibrationMenu = gestureCalibrationMenu;
    }
}
