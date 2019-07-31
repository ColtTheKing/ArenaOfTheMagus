using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MenuButton
{
    private MenuScreen thisMenu, previousMenu;

    public override void Press()
    {
        thisMenu.SwapMenu(0);
        previousMenu.ActivateMenu();
    }

    public void Init(int buttonId, MenuScreen thisMenu, MenuScreen previousMenu)
    {
        this.buttonId = buttonId;
        this.thisMenu = thisMenu;
        this.previousMenu = previousMenu;
    }
}
