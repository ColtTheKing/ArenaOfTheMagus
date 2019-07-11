using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSelectMenu : MenuScreen
{
    public Button leftFireUIButton, leftWaterUIButton, leftEarthUIButton, leftAirUIButton;
    public Button rightFireUIButton, rightWaterUIButton, rightEarthUIButton, rightAirUIButton, fightUIButton;
    public Image leftElementImage, rightElementImage;
    public Sprite fireSprite, waterSprite, earthSprite, airSprite;

    public ElementButton leftFireButton, leftWaterButton, leftEarthButton, leftAirButton;
    public ElementButton rightFireButton, rightWaterButton, rightEarthButton, rightAirButton;
    public FightButton fightButton;
    public BackButton backButton;

    private GameManager gameManager;
    private MainMenu mainMenu;
    private SpellHandler.SpellElement leftElement, rightElement;
    private bool leftSelected, rightSelected;

    public override void Start()
    {
        base.Start();
    }

    public void Init(GameManager gameManager, MainMenu mainMenu)
    {
        this.gameManager = gameManager;
        this.mainMenu = mainMenu;

        //Initialize the buttons
        leftFireButton.Init(0, SpellHandler.SpellElement.FIRE, true, this);
        leftWaterButton.Init(1, SpellHandler.SpellElement.WATER, true, this);
        leftEarthButton.Init(2, SpellHandler.SpellElement.EARTH, true, this);
        leftAirButton.Init(3, SpellHandler.SpellElement.AIR, true, this);
        rightFireButton.Init(4, SpellHandler.SpellElement.FIRE, false, this);
        rightWaterButton.Init(5, SpellHandler.SpellElement.WATER, false, this);
        rightEarthButton.Init(6, SpellHandler.SpellElement.EARTH, false, this);
        rightAirButton.Init(7, SpellHandler.SpellElement.AIR, false, this);
        fightButton.Init(8, gameManager, this);
        backButton.Init(9, this, mainMenu);
    }

    public override void PressButton(int buttonId)
    {
        //Press the corresponding button
        switch (buttonId)
        {
            case 0:
                if(leftFireUIButton.interactable)
                    leftFireButton.Press();
                break;
            case 1:
                if (leftWaterUIButton.interactable)
                    leftWaterButton.Press();
                break;
            case 2:
                if (leftEarthUIButton.interactable)
                    leftEarthButton.Press();
                break;
            case 3:
                if (leftAirUIButton.interactable)
                    leftAirButton.Press();
                break;
            case 4:
                if (rightFireUIButton.interactable)
                    rightFireButton.Press();
                break;
            case 5:
                if (rightWaterUIButton.interactable)
                    rightWaterButton.Press();
                break;
            case 6:
                if (rightEarthUIButton.interactable)
                    rightEarthButton.Press();
                break;
            case 7:
                if (rightAirUIButton.interactable)
                    rightAirButton.Press();
                break;
            case 8:
                if (fightUIButton.interactable)
                    fightButton.Press();
                break;
            case 9:
                backButton.Press();
                break;
            default:
                break;
        }
    }

    public void SetElement(SpellHandler.SpellElement element, bool left)
    {
        if (left)
        {
            //Make the pressed button and it's mirror uninteractable and update the element sprite
            switch (element)
            {
                case SpellHandler.SpellElement.FIRE:
                    leftElementImage.sprite = fireSprite;
                    leftFireUIButton.interactable = false;
                    rightFireUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.WATER:
                    leftElementImage.sprite = waterSprite;
                    leftWaterUIButton.interactable = false;
                    rightWaterUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.EARTH:
                    leftElementImage.sprite = earthSprite;
                    leftEarthUIButton.interactable = false;
                    rightEarthUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.AIR:
                    leftElementImage.sprite = airSprite;
                    leftAirUIButton.interactable = false;
                    rightAirUIButton.interactable = false;
                    break;
                default:
                    break;
            }

            //Make the previously pressed button interactable again unless the element hadn't been set
            if (leftSelected)
            {
                switch (leftElement)
                {
                    case SpellHandler.SpellElement.FIRE:
                        leftFireUIButton.interactable = true;
                        rightFireUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.WATER:
                        leftWaterUIButton.interactable = true;
                        rightWaterUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.EARTH:
                        leftEarthUIButton.interactable = true;
                        rightEarthUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.AIR:
                        leftAirUIButton.interactable = true;
                        rightAirUIButton.interactable = true;
                        break;
                    default:
                        break;
                }
            }

            leftElement = element;
            leftSelected = true;
        }
        else
        {
            //Make the pressed button and it's mirror uninteractable and update the element sprite
            switch (element)
            {
                case SpellHandler.SpellElement.FIRE:
                    rightElementImage.sprite = fireSprite;
                    leftFireUIButton.interactable = false;
                    rightFireUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.WATER:
                    rightElementImage.sprite = waterSprite;
                    leftWaterUIButton.interactable = false;
                    rightWaterUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.EARTH:
                    rightElementImage.sprite = earthSprite;
                    leftEarthUIButton.interactable = false;
                    rightEarthUIButton.interactable = false;
                    break;
                case SpellHandler.SpellElement.AIR:
                    rightElementImage.sprite = airSprite;
                    leftAirUIButton.interactable = false;
                    rightAirUIButton.interactable = false;
                    break;
                default:
                    break;
            }

            //Make the previously pressed button interactable again unless the element hadn't been set
            if (rightSelected)
            {
                switch (rightElement)
                {
                    case SpellHandler.SpellElement.FIRE:
                        leftFireUIButton.interactable = true;
                        rightFireUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.WATER:
                        leftWaterUIButton.interactable = true;
                        rightWaterUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.EARTH:
                        leftEarthUIButton.interactable = true;
                        rightEarthUIButton.interactable = true;
                        break;
                    case SpellHandler.SpellElement.AIR:
                        leftAirUIButton.interactable = true;
                        rightAirUIButton.interactable = true;
                        break;
                    default:
                        break;
                }
            }

            rightElement = element;
            rightSelected = true;
        }

        if (leftSelected && rightSelected)
            fightUIButton.interactable = true;
    }

    public SpellHandler.SpellElement GetElement(bool left)
    {
        return left ? leftElement : rightElement;
    }
}
