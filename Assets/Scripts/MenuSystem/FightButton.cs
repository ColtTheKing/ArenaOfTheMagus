using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightButton : MenuButton
{
    private GameManager gameManager;
    private ElementSelectMenu elementSelectMenu;
    private MainMenu mainMenu;

    public override void Press()
    {
        elementSelectMenu.SwapMenu(0);
        mainMenu.ActivateMenu();
        gameManager.StartGame(elementSelectMenu.GetElement(true), elementSelectMenu.GetElement(false));
    }

    public void Init(int buttonId, GameManager gameManager, ElementSelectMenu elementSelectMenu, MainMenu mainMenu)
    {
        this.buttonId = buttonId;
        this.gameManager = gameManager;
        this.elementSelectMenu = elementSelectMenu;
        this.mainMenu = mainMenu;
    } 
}
