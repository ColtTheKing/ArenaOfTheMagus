using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlock
{
    public List<Enemy> flockMembers;
    public Player player;

    private float wanderTimer, wanderFrequency, wanderRadius, arenaRadius;
    private CollisionChecker collisionChecker;

    public EnemyFlock(List<Enemy> enemies, float wanderRadius, float arenaRadius, float wanderFrequency, Player player, float healthModifier, float damageModifier, CollisionChecker collisionChecker)
    {
        flockMembers = enemies;
        
        this.wanderRadius = wanderRadius;
        this.arenaRadius = arenaRadius;
        this.wanderFrequency = wanderFrequency;
        this.player = player;

        for(int i = 0; i < flockMembers.Count; i++)
        {
            flockMembers[i].Init(this, i, healthModifier, damageModifier, collisionChecker);
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = 0; i < flockMembers.Count; i++)
        {
            if (!flockMembers[i].GetAlive())
            {
                flockMembers[i].Kill();
                flockMembers.RemoveAt(i--);
            }
        }

        //When the time is up, pick another location to wander to
        if (wanderTimer <= 0)
        {
            ChangeWanderLocation();
        }

        wanderTimer -= deltaTime;
    }

    public void KillFlock()
    {
        foreach(Enemy e in flockMembers)
            e.Kill();

        flockMembers.Clear();
    }

    private void ChangeWanderLocation()
    {
        wanderTimer = wanderFrequency;

        Vector3 currentWander = flockMembers[0].GetWanderLocation();

        float lowX, highX, lowZ, highZ;

        if (currentWander.x - wanderRadius < -arenaRadius)
            lowX = -arenaRadius;
        else
            lowX = currentWander.x - wanderRadius;

        if (currentWander.x + wanderRadius > arenaRadius)
            highX = arenaRadius;
        else
            highX = currentWander.x + wanderRadius;

        if (currentWander.z - wanderRadius < -arenaRadius)
            lowZ = -arenaRadius;
        else
            lowZ = currentWander.z - wanderRadius;

        if (currentWander.z + wanderRadius > arenaRadius)
            highZ = arenaRadius;
        else
            highZ = currentWander.z + wanderRadius;

        float randX = Random.Range(lowX, highX);
        float randZ = Random.Range(lowZ, highZ);

        Vector3 wanderLocation = new Vector3(randX, 0, randZ);

        //Ensure the location is on the outside of the radius
        wanderLocation = wanderLocation.normalized * wanderRadius;

        for(int i = 0; i < flockMembers.Count; i++)
        {
            flockMembers[i].SetWanderLocation(wanderLocation);
        }
    }
}
