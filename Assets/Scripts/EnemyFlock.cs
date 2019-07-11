using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlock
{
    public List<Enemy> flockMembers;
    public Player player;
    public List<Obstacle> obstacles;

    private Vector3 origin;
    private float wanderTimer, wanderFrequency, wanderRadius;

    public EnemyFlock(List<Enemy> enemies, Vector3 origin, float wanderRadius, float wanderFrequency, Player player, List<Obstacle> obstacles)
    {
        flockMembers = enemies;

        this.origin = origin;
        this.wanderRadius = wanderRadius;
        this.wanderFrequency = wanderFrequency;
        this.player = player;
        this.obstacles = obstacles;

        for(int i = 0; i < flockMembers.Count; i++)
        {
            flockMembers[i].Init(this, i);
        }
    }

    public void Update(float deltaTime)
    {
        //When the time is up, pick another location to wander to
        if (wanderTimer <= 0)
        {
            ChangeWanderLocation();
        }

        wanderTimer -= deltaTime;
    }

    private void ChangeWanderLocation()
    {
        wanderTimer = wanderFrequency;

        float randX = Random.Range(origin.x - wanderRadius, origin.x + wanderRadius);
        float randZ = Random.Range(origin.z - wanderRadius, origin.z + wanderRadius);

        Vector3 wanderLocation = new Vector3(randX, 0, randZ);

        //Ensure the location is on the outside of the radius
        wanderLocation = wanderLocation.normalized * wanderRadius;
        wanderLocation.y = 1;

        for(int i = 0; i < flockMembers.Count; i++)
        {
            flockMembers[i].SetWanderLocation(wanderLocation);
        }
    }
}
