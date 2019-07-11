using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightButton : MenuButton
{
    private GameManager gameManager;
    private ElementSelectMenu elementSelectMenu;

    public override void Press()
    {
        gameManager.StartGame(elementSelectMenu.GetElement(true), elementSelectMenu.GetElement(false));
    }

    public void Init(int buttonId, GameManager gameManager, ElementSelectMenu elementSelectMenu)
    {
        this.buttonId = buttonId;
        this.gameManager = gameManager;
        this.elementSelectMenu = elementSelectMenu;
    } 
}
