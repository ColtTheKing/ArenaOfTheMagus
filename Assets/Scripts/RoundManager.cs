using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Enemy enemy;
    public List<Vector3> enemyFlockOffsets;
    public Vector3 flockPosition; //will be generated later
    public List<int> flocksPerRound;
    public float timeBetweenSpawns, wanderFrequency, wanderRadius;
    public bool spawnEnemies;
    public List<Obstacle> obstacles;

    private bool inGame;
    private int currentRound, flocksKilled;
    private float spawnTimer;
    private List<EnemyFlock> flocks;
    private Player player;

    void Start()
    {
        currentRound = 0;
        spawnTimer = 0;
        flocksKilled = 0;

        flocks = new List<EnemyFlock>();
        player = GetComponent<Player>();
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
            List<Enemy> enemies = new List<Enemy>();

            for (int i = 0; i < enemyFlockOffsets.Count; i++)
            {
                Enemy e = Instantiate(enemy);

                //Flock starting position currently hardcoded, will be generated later
                e.gameObject.transform.eulerAngles = new Vector3(0, -90, 0);
                e.gameObject.transform.position = flockPosition + enemyFlockOffsets[i];

                enemies.Add(e);
            }

            flocks.Add(new EnemyFlock(enemies, flockPosition, wanderRadius, wanderFrequency, player, obstacles));

            spawnTimer = 0;
        }

        //Do flock things
        for(int i = 0; i < flocks.Count; i++)
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
        player.ResetHealth();
    }

    public bool GameOngoing()
    {
        return inGame;
    }
}
