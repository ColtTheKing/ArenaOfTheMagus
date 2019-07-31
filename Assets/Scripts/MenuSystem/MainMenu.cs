using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuScreen
{
    public ElementSelectMenu elementSelectMenu;
    //PUT OTHER SUBMENUS HERE

    public PlayButton playButton;
    public CalibrateButton calibrateButton;
    public SettingsButton settingsButton;
    public QuitButton quitButton;

    private GameManager gameManager;
    private int curMenu;

    public override void Start()
    {
        base.Start();
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        curMenu = 0;

        //Initialize the sub-menus
        elementSelectMenu.Init(gameManager, this);

        //Initialize the buttons
        playButton.Init(0, this, elementSelectMenu);
        calibrateButton.Init(1);
        settingsButton.Init(2);
        quitButton.Init(3);
    }

    public override void PressButton(int buttonId)
    {
        if (curMenu == 0)
        {
            //This is the currently displayed menu
            switch (buttonId)
            {
                case 0:
                    playButton.Press();
                    break;
                case 1:
                    calibrateButton.Press();
                    break;
                case 2:
                    settingsButton.Press();
                    break;
                case 3:
                    quitButton.Press();
                    break;
                default:
                    break;
            }
        }
        else
        {
            //The currently displayed menu is in one of the submenus so pass the call down
            switch (curMenu)
            {
                case 1:
                    elementSelectMenu.PressButton(buttonId);
                    break;
                case 2:
                    //Calibrate menu
                    break;
                case 3:
                    //Settings menu
                    break;
                default:
                    break;
            }
        }
    }

    public override void ActivateMenu()
    {
        base.ActivateMenu();

        curMenu = 0;
    }

    public override void SwapMenu(int menuId)
    {
        base.SwapMenu(menuId);

        switch(menuId)
        {
            case 0:
                elementSelectMenu.ActivateMenu();
                curMenu = 1;
                break;
            case 1:
                //Calibrate menu
                break;
            case 2:
                //Settings menu
                break;
            default:
                break;
        }
    }
}