using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureCalibrationMenu : MenuScreen
{
    public Button gestureQuickUIButton, gestureHeavyUIButton, gestureSpecialUIButton, gestureOffensiveUIButton, gestureDefensiveUIButton;
    public Image gestureQuickImage, gestureHeavyImage, gestureSpecialImage, gestureOffensiveImage, gestureDefensiveImage;
    public Sprite checkSprite, xSprite;
    public Text gestureNameText, timesToPerformText;

    public GestureButton gestureQuickButton, gestureHeavyButton, gestureSpecialButton, gestureOffensiveButton, gestureDefensiveButton;
    public StartGestureButton startButton;
    public RestartGestureButton restartButton;
    public BackButton backButton;

    public GameObject quickDisplay, heavyDisplay, specialDisplay, offensiveDisplay, defensiveDisplay;

    public int maxTimesToPerform;

    private GameManager gameManager;
    private MainMenu mainMenu;
    private Player player;
    private GestureManager gestureManager;
    private SpellHandler.SpellType currentGestureType;
    private int timesToPerform;
    private GameObject currentDisplay;
    private bool[] gestureSet;

    public override void Awake()
    {
        base.Awake();

        currentGestureType = SpellHandler.SpellType.NONE;
    }

    public override void Start()
    {
        base.Start();
    }

    public void Init(GameManager gameManager, MainMenu mainMenu)
    {
        this.gameManager = gameManager;
        this.mainMenu = mainMenu;
        player = gameManager.GetPlayer();
        gestureManager = player.GetGestureManager();
        gestureManager.SetCalibrationMenu(this);

        gestureSet = new bool[5];

        //Initialize the buttons
        gestureQuickButton.Init(0, SpellHandler.SpellType.ONE_QUICK, this);
        gestureHeavyButton.Init(1, SpellHandler.SpellType.ONE_HEAVY, this);
        gestureSpecialButton.Init(2, SpellHandler.SpellType.ONE_SPECIAL, this);
        gestureOffensiveButton.Init(3, SpellHandler.SpellType.TWO_OFFENSE, this);
        gestureDefensiveButton.Init(4, SpellHandler.SpellType.TWO_DEFENSE, this);
        startButton.Init(5, this);
        restartButton.Init(6, this);
        backButton.Init(7, this, mainMenu);
    }

    public override void PressButton(int buttonId)
    {
        //Press the corresponding button
        switch (buttonId)
        {
            case 0:
                if (gestureQuickUIButton.interactable)
                    gestureQuickButton.Press();
                break;
            case 1:
                if (gestureHeavyUIButton.interactable)
                    gestureHeavyButton.Press();
                break;
            case 2:
                if (gestureSpecialUIButton.interactable)
                    gestureSpecialButton.Press();
                break;
            case 3:
                if (gestureOffensiveUIButton.interactable)
                    gestureOffensiveButton.Press();
                break;
            case 4:
                if (gestureDefensiveUIButton.interactable)
                    gestureDefensiveButton.Press();
                break;
            case 5:
                startButton.Press();
                break;
            case 6:
                restartButton.Press();
                break;
            case 7:
                backButton.Press();
                break;
        }
    }

    public void SetCurrentGesture(SpellHandler.SpellType type)
    {
        //Update the currently shown gesture display
        if (currentDisplay != null)
            Destroy(currentDisplay);

        //Make the pressed button and it's mirror uninteractable and update the element sprite
        switch (type)
        {
            case SpellHandler.SpellType.ONE_QUICK:
                gestureQuickUIButton.interactable = false;
                currentDisplay = Instantiate(quickDisplay);
                gestureNameText.text = "Quick";
                break;
            case SpellHandler.SpellType.ONE_HEAVY:
                gestureHeavyUIButton.interactable = false;
                currentDisplay = Instantiate(heavyDisplay);
                gestureNameText.text = "Heavy";
                break;
            case SpellHandler.SpellType.ONE_SPECIAL:
                gestureSpecialUIButton.interactable = false;
                currentDisplay = Instantiate(specialDisplay);
                gestureNameText.text = "Special";
                break;
            case SpellHandler.SpellType.TWO_OFFENSE:
                gestureOffensiveUIButton.interactable = false;
                currentDisplay = Instantiate(offensiveDisplay);
                gestureNameText.text = "Offense";
                break;
            case SpellHandler.SpellType.TWO_DEFENSE:
                gestureDefensiveUIButton.interactable = false;
                currentDisplay = Instantiate(defensiveDisplay);
                gestureNameText.text = "Defense";
                break;
        }
        
        //Make the previously pressed button interactable again
        switch (currentGestureType)
        {
            case SpellHandler.SpellType.ONE_QUICK:
                gestureQuickUIButton.interactable = true;
                break;
            case SpellHandler.SpellType.ONE_HEAVY:
                gestureHeavyUIButton.interactable = true;
                break;
            case SpellHandler.SpellType.ONE_SPECIAL:
                gestureSpecialUIButton.interactable = true;
                break;
            case SpellHandler.SpellType.TWO_OFFENSE:
                gestureOffensiveUIButton.interactable = true;
                break;
            case SpellHandler.SpellType.TWO_DEFENSE:
                gestureDefensiveUIButton.interactable = true;
                break;
        }

        currentGestureType = type;

        gestureManager.ChangeCalibrationType(type);

        timesToPerform = maxTimesToPerform;
        timesToPerformText.text = timesToPerform.ToString();
    }

    public override void ActivateMenu()
    {
        base.ActivateMenu();

        SetCurrentGesture(SpellHandler.SpellType.ONE_QUICK);
    }

    public override void SwapMenu(int menuId)
    {
        base.SwapMenu(menuId);

        gestureQuickImage.sprite = xSprite;
        gestureHeavyImage.sprite = xSprite;
        gestureSpecialImage.sprite = xSprite;
        gestureOffensiveImage.sprite = xSprite;
        gestureDefensiveImage.sprite = xSprite;

        for (int i = 0; i < gestureSet.Length; i++)
            gestureSet[i] = false;

        SetCurrentGesture(SpellHandler.SpellType.ONE_QUICK);

        gestureManager.ResetCalibration();

        if (currentDisplay != null)
            Destroy(currentDisplay);
    }

    public void StartGesture()
    {
        gestureManager.StartCalibrating();

        player.SetPointerEnabled(false);
    }

    public void RestartGesture()
    {
        timesToPerform = maxTimesToPerform;
        timesToPerformText.text = timesToPerform.ToString();

        gestureManager.ResetCalibration();
    }

    public void DecreaseRemainingGestures()
    {
        timesToPerform--;
        timesToPerformText.text = timesToPerform.ToString();

        player.SetPointerEnabled(true);
    }

    public void FinishedGesture()
    {
        //Update the completed icon and cycle to the next unfinished gesture
        switch(currentGestureType)
        {
            case SpellHandler.SpellType.ONE_QUICK:
                gestureSet[0] = true;
                gestureQuickImage.sprite = checkSprite;
                NextGesture(0);
                break;
            case SpellHandler.SpellType.ONE_HEAVY:
                gestureSet[1] = true;
                gestureHeavyImage.sprite = checkSprite;
                NextGesture(1);
                break;
            case SpellHandler.SpellType.ONE_SPECIAL:
                gestureSet[2] = true;
                gestureSpecialImage.sprite = checkSprite;
                NextGesture(2);
                break;
            case SpellHandler.SpellType.TWO_OFFENSE:
                gestureSet[3] = true;
                gestureOffensiveImage.sprite = checkSprite;
                NextGesture(3);
                break;
            case SpellHandler.SpellType.TWO_DEFENSE:
                gestureSet[4] = true;
                gestureDefensiveImage.sprite = checkSprite;
                NextGesture(4);
                break;
        }

        player.SetPointerEnabled(true);
    }

    private void NextGesture(int curGestureInd)
    {
        int i = curGestureInd == gestureSet.Length-1 ? 0 : curGestureInd;

        do //Loop through until the next unfinished gesture
        {
            if(!gestureSet[i])
            {
                //Swap to this gesture and return
                switch(i)
                {
                    case 0:
                        SetCurrentGesture(SpellHandler.SpellType.ONE_QUICK);
                        break;
                    case 1:
                        SetCurrentGesture(SpellHandler.SpellType.ONE_HEAVY);
                        break;
                    case 2:
                        SetCurrentGesture(SpellHandler.SpellType.ONE_SPECIAL);
                        break;
                    case 3:
                        SetCurrentGesture(SpellHandler.SpellType.TWO_OFFENSE);
                        break;
                    case 4:
                        SetCurrentGesture(SpellHandler.SpellType.TWO_DEFENSE);
                        break;
                }

                return;
            }

            //Increment the index
            if(++i == gestureSet.Length)
                i = 0;

        } while (i != curGestureInd);

        //If all gestures are finished exit the menu
        this.SwapMenu(0);
        mainMenu.ActivateMenu();
    }
}
