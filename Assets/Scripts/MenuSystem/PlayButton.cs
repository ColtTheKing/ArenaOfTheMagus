using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MenuButton
{
    private MainMenu mainMenu;

    public override void Press()
    {
        mainMenu.SwapMenu(0);
    }

    public void Init(int buttonId, MainMenu mainMenu)
    {
        this.buttonId = buttonId;
        this.mainMenu = mainMenu;
    }
}
