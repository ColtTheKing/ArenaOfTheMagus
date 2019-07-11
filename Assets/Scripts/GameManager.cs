using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuObject;
    public MainMenu mainMenu;
    public GameObject[] menuWalls;

    private RoundManager roundManager;
    private SpellHandler spellHandler;
    private bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        roundManager = GetComponent<RoundManager>();
        spellHandler = GetComponent<SpellHandler>();

        mainMenu.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressMenuButton(int buttonId)
    {
        mainMenu.PressButton(buttonId);
    }

    public bool InGame()
    {
        return gameStarted;
    }

    public void StartGame(SpellHandler.SpellElement leftElement, SpellHandler.SpellElement rightElement)
    {
        foreach (GameObject g in menuWalls)
            g.SetActive(false);

        gameStarted = true;

        spellHandler.elementLeft = leftElement;
        spellHandler.elementRight = rightElement;

        roundManager.StartRound();
    }
}
