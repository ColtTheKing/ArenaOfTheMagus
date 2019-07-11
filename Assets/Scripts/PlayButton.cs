using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MenuButton
{
    private MainMenu mainMenu;
    private ElementSelectMenu elementSelectMenu;

    public override void Press()
    {
        mainMenu.SwapMenu(0);
        elementSelectMenu.ActivateMenu();
    }

    public void Init(int buttonId, MainMenu mainMenu, ElementSelectMenu elementSelectMenu)
    {
        this.buttonId = buttonId;
        this.mainMenu = mainMenu;
        this.elementSelectMenu = elementSelectMenu;
    }
}
