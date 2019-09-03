using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuObject;
    public MainMenu mainMenu;
    public GameObject[] menuWalls;
    public GameObject arena;

    private RoundManager roundManager;
    private bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        roundManager = GetComponent<RoundManager>();

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

        arena.SetActive(true);

        gameStarted = true;

        roundManager.StartRounds(leftElement, rightElement);
    }

    public void EndGame()
    {
        gameStarted = false;

        if (roundManager.GameOngoing())
            roundManager.EndRounds();

        foreach (GameObject g in menuWalls)
            g.SetActive(true);

        arena.SetActive(false);
    }

    public Player GetPlayer()
    {
        return roundManager.GetPlayer();
    }
}
