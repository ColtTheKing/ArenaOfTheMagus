using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Enemy enemy;
    public List<Vector3> enemyFlockOffsets;
    public List<int> flocksPerRound;
    public float timeBetweenSpawns, wanderFrequency, wanderRadius, spawnDistMin, spawnDistMax;
    public bool spawnEnemies;
    public List<Obstacle> obstacles;

    private bool inGame;
    private int currentRound, flocksKilled;
    private float spawnTimer;
    private List<EnemyFlock> flocks;
    private Player player;
    private GameManager gameManager;

    void Start()
    {
        currentRound = 0;
        spawnTimer = 0;
        flocksKilled = 0;

        flocks = new List<EnemyFlock>();
        player = GetComponent<Player>();
        gameManager = GetComponent<GameManager>();
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
                flocksKilled++;
            }
        }

        //If all the flocks are dead start the next round
        if (flocksKilled == flocksPerRound[currentRound])
            NextRound();

        //When time has passed make a flock
        if (spawnTimer >= timeBetweenSpawns && flocks.Count < flocksPerRound[currentRound])
        {
            SpawnFlock();
        }

        //Do flock things
        for (int i = 0; i < flocks.Count; i++)
        {
            //Handle this flock
            flocks[i].Update(Time.deltaTime);
        }
    }

    public void StartRounds()
    {
        inGame = true;
        player.ToggleMenuPointer(false);
    }

    public void NextRound()
    {
        if (++currentRound >= flocksPerRound.Count)
        {
            EndRounds();
        }
        else
        {
            spawnTimer = 0;
            flocksKilled = 0;
        }
    }

    public void EndRounds()
    {
        foreach (EnemyFlock f in flocks)
            f.KillFlock();

        flocks.Clear();

        currentRound = 0;
        spawnTimer = 0;

        inGame = false;
        player.ToggleMenuPointer(true);
        player.transform.position = new Vector3(0, 0, 0);
        player.ResetStuff();

        gameManager.EndGame();
    }

    public bool GameOngoing()
    {
        return inGame;
    }

    private void SpawnFlock()
    {
        //Determine where to spawn the flock based on player position and the positions of the other flocks
        Vector3 flockPosition;

        float spawnDist = Random.Range(spawnDistMin, spawnDistMax);
        float relativeRot = Random.Range(0, 359);
        float theta;

        if(relativeRot < 90)
        {
            theta = relativeRot;
            flockPosition = new Vector3(1, 0, 1);
        }
        else if(relativeRot < 180)
        {
            theta = 180 - relativeRot;
            flockPosition = new Vector3(1, 0, -1);
        }
        else if(relativeRot < 270)
        {
            theta = 180 + relativeRot;
            flockPosition = new Vector3(-1, 0, -1);
        }
        else
        {
            theta = 360 - relativeRot;
            flockPosition = new Vector3(-1, 0, 1);
        }

        flockPosition.x *= Mathf.Sin(theta * Mathf.Deg2Rad) * spawnDist;
        flockPosition.z *= Mathf.Cos(theta * Mathf.Deg2Rad) * spawnDist;

        //Create the flock based on that position
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemyFlockOffsets.Count; i++)
        {
            Enemy e = Instantiate(enemy);

            e.transform.position += flockPosition + enemyFlockOffsets[i];

            enemies.Add(e);
        }

        flocks.Add(new EnemyFlock(enemies, flockPosition, wanderRadius, wanderFrequency, player, obstacles));

        spawnTimer = 0;
    }
}
