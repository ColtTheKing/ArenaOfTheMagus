using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateButton : MenuButton
{
    private MainMenu mainMenu;

    public override void Press()
    {
        mainMenu.SwapMenu(1);
    }

    public void Init(int buttonId, MainMenu mainMenu)
    {
        this.buttonId = buttonId;
        this.mainMenu = mainMenu;
    }
}