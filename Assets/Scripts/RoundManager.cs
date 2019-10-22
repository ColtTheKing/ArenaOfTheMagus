using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public Text roundText;
    public Enemy enemy;
    public BigSkeleton harold;
    public List<Vector3> enemyFlockOffsets;
    public List<Vector3> flockSpawns;
    public List<int> increaseFlockRounds;
    public float timeBetweenSpawns, wanderFrequency, wanderRadius, arenaRadius, spawnDistMin, spawnDistMax, enemyHpPerRound, enemyDmgPerRound;
    public bool spawnEnemies;

    private bool inGame;
    private int currentRound, flocksKilledThisRound, flocksPerRound;
    private float spawnTimer;
    private List<EnemyFlock> flocks;
    private Player player;
    private GameManager gameManager;
    private CollisionChecker collisionChecker;
    private List<bool> spawnsOccupied;

    void Start()
    {
        currentRound = 1;
        spawnTimer = 0;
        flocksKilledThisRound = 0;
        flocksPerRound = 1;

        flocks = new List<EnemyFlock>();
        player = GetComponent<Player>();
        gameManager = GetComponent<GameManager>();
        collisionChecker = GetComponent<CollisionChecker>();

        spawnsOccupied = new List<bool>();

        for (int i = 0; i < flockSpawns.Count; i++)
            spawnsOccupied.Add(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame || !spawnEnemies)
            return;

        spawnTimer += Time.deltaTime;

        for (int i = 0; i < flocks.Count; i++)
        {
            //If the flock has been killed remove it from the list
            if (flocks[i].flockMembers.Count == 0)
            {
                flocks.RemoveAt(i--);
                flocksKilledThisRound++;
            }
        }

        //If all the flocks are dead start the next round
        if (flocksKilledThisRound == flocksPerRound)
            NextRound();

        //When time has passed make a flock
        if (spawnTimer >= timeBetweenSpawns && flocks.Count < flocksPerRound)
            SpawnFlock();

        //Do flock things
        for (int i = 0; i < flocks.Count; i++)
        {
            //Handle this flock
            flocks[i].Update(Time.deltaTime);
        }
    }

    public void StartRounds(SpellHandler.SpellElement leftElement, SpellHandler.SpellElement rightElement)
    {
        inGame = true;
        player.ToggleMenuPointer(false);
        player.SetElements(leftElement, rightElement);

        harold.PlayEnter();
    }

    public void NextRound()
    {
        spawnTimer = 0;
        flocksKilledThisRound = 0;

        //Clear the spawns
        for (int i = 0; i < spawnsOccupied.Count; i++)
            spawnsOccupied[i] = false;

        if (++currentRound == increaseFlockRounds[flocksPerRound - 1])
        {
            harold.PlayAdvance(flocksPerRound - 1);
            flocksPerRound++;
        }
        
        roundText.text = "Round " + currentRound;
    }

    public void EndRounds()
    {
        foreach (EnemyFlock f in flocks)
            f.KillFlock();

        //Clear the spawns
        for (int i = 0; i < spawnsOccupied.Count; i++)
            spawnsOccupied[i] = false;

        flocks.Clear();
        
        roundText.text = "Round 1";

        currentRound = 1;
        spawnTimer = 0;
        flocksKilledThisRound = 0;
        flocksPerRound = 1;

        inGame = false;
        player.ToggleMenuPointer(true);
        player.transform.position = new Vector3(0, 0, 0);
        player.ResetStuff();

        harold.PlayLose();

        gameManager.EndGame();
    }

    public bool GameOngoing()
    {
        return inGame;
    }

    private void SpawnFlock()
    {
        Vector3 flockPosition;

        //Place the flock in an open spawn position
        int rand = Random.Range(0, flockSpawns.Count);

        while(spawnsOccupied[rand])
        {
            if (++rand == flockSpawns.Count)
                rand = 0;
        }

        flockPosition = flockSpawns[rand];

        //Mark that spawn as occupied
        spawnsOccupied[rand] = true;

        //Create the flock based on that position
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemyFlockOffsets.Count; i++)
        {
            Enemy e = Instantiate(enemy);

            e.transform.position += flockPosition + enemyFlockOffsets[i];

            enemies.Add(e);
        }

        flocks.Add(new EnemyFlock(enemies, wanderRadius, arenaRadius, wanderFrequency, player, enemyHpPerRound * currentRound, enemyDmgPerRound * currentRound, collisionChecker));

        spawnTimer = 0;
    }

    public Player GetPlayer()
    {
        return player;
    }
}
