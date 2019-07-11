using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int startingHP, attackDamage;
    public float speed, attackCooldown, attackRange;
    //AI Movement Variables
    public float pursueDuration, pursueAheadDist, pursueRange, avoidDist, separationRadius, pursueCoefficient, avoidCoefficient, separationCoefficient, cohesionCoefficient, wanderCoefficient;
    public Material baseMaterial, onFireMaterial;

    private Health healthComp;
    private float onFire, pursueTimer, attacking;
    private int flockId;
    private EnemyFlock flock;
    private Vector3 velocity, wanderLocation;

    void Start()
    {
        healthComp = new Health(startingHP);
    }

    // Update is called once per frame
    void Update()
    {
        //Have the AI figure out what to do
        DecideBehaviour(Time.deltaTime);

        if (!healthComp.Update(Time.deltaTime))
        {
            Destroy(gameObject);
        }

        if (onFire > 0)
        {
            onFire -= Time.deltaTime;

            if (onFire < 0)
            {
                gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
                onFire = 0;
            }
        }
    }

    public void DecideBehaviour(float deltaTime)
    {
        RaycastHit objectSeen;

        int layerMask = 1 << 8;

        //If the player is in sight the enemy will pursue them for a while even after losing sight
        if (Physics.Raycast(transform.position, flock.player.GetComponentInChildren<Camera>().transform.position - transform.position, out objectSeen, pursueRange, layerMask)
            && objectSeen.transform.gameObject.GetComponentInParent<Player>() != null)
                pursueTimer = pursueDuration;
        
        //If they are still pursuing
        if(pursueTimer > 0)
        {
            pursueTimer -= deltaTime;

            //Attack the player if close enough and not already attacking
            if(attacking > 0)
            {
                attacking -= deltaTime;
            }
            else
            {
                if ((flock.player.transform.position - transform.position).magnitude <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    PursuePlayer(deltaTime);
                }
            }
        }
        else
        {
            Flock(deltaTime);
        }
    }

    public void Init(EnemyFlock flock, int id)
    {
        this.flock = flock;
        flockId = id;
    }

    public void Damage(int damage)
    {
        healthComp.TakeDamage(damage);

        //Maybe add a hurt sound effect
    }

    public void Ignite()
    {
        healthComp.AddDOTEffect(SpellHandler.FIRE_DOT_DURATION, SpellHandler.FIRE_DOT_DAMAGE);

        //Add some fire effects
        onFire = SpellHandler.FIRE_DOT_DURATION;
        gameObject.GetComponent<MeshRenderer>().material = onFireMaterial;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetWanderLocation(Vector3 location)
    {
        wanderLocation = location;
    }

    private void AttackPlayer()
    {
        //do attack
        flock.player.Damage(attackDamage);

        //set the attack cooldown
        attacking = attackCooldown;
    }

    private void PursuePlayer(float deltaTime)
    {
        //get velocity of player and move towards a point in front of them
        Vector3 target = flock.player.transform.position + flock.player.GetVelocity() * pursueAheadDist;
        target.y = 0;

        //find the direction towards that point
        Vector3 playerPursue = (target - transform.position).normalized;

        //also take into account obstacle avoidance
        Vector3 obstacleAvoid = AvoidObstacles();

        //Make the pursue be a unit vector
        Vector3 moveDir = playerPursue * pursueCoefficient + obstacleAvoid * avoidCoefficient;
        moveDir.y = 0;
        moveDir = moveDir.normalized;

        transform.position += moveDir * speed * deltaTime;
    }

    private void Flock(float deltaTime)
    {
        //Stay separated from other flock members
        Vector3 separation = Vector3.zero;

        for(int i = 0; i < flock.flockMembers.Count; i++)
        {
            if (i == flockId)
                continue;

            Vector3 fromFlockMember = transform.position - flock.flockMembers[i].transform.position;

            if (fromFlockMember.magnitude < separationRadius)
            {
                //Make force decrease from 1 to zero from the current flock member to the separation radius
                float separationForce = Mathf.Sqrt(separationRadius - fromFlockMember.magnitude);
                Vector3 hi = fromFlockMember.normalized * separationForce;
                separation += hi;
            }
        }

        //Keeps the value reasonable while allowing it to be smaller than 1
        if (separation.magnitude > 1)
            separation = separation.normalized;

        separation *= separationCoefficient;

        //Stay cohesive with the flock
        Vector3 avgFlockPos = Vector3.zero;

        for (int i = 0; i < flock.flockMembers.Count; i++)
        {
            avgFlockPos += flock.flockMembers[i].transform.position;
        }

        avgFlockPos /= flock.flockMembers.Count;

        //Move towards the average position of the flock
        Vector3 cohesion = (avgFlockPos - transform.position).normalized * cohesionCoefficient;

        //also take into account obstacle avoidance
        Vector3 obstacleAvoid = AvoidObstacles() * avoidCoefficient;

        //Wander towards a point
        Vector3 wander = (wanderLocation - transform.position).normalized * wanderCoefficient;

        Vector3 move = separation + cohesion + obstacleAvoid + wander;

        transform.position += move * speed * deltaTime;
    }

    private Vector3 AvoidObstacles()
    {
        Vector3 avoid = Vector3.zero;

        for (int i = 0; i < flock.obstacles.Count; i++)
        {
            Vector3 fromObstacle = transform.position - flock.obstacles[i].transform.position;
            float avoidRadius = flock.obstacles[i].radius + avoidDist;

            if (fromObstacle.magnitude < avoidRadius)
            {
                //Make force decrease from 1 to zero from the current flock member to the separation radius
                float avoidForce = (avoidRadius - fromObstacle.magnitude) / avoidRadius;

                avoid += fromObstacle.normalized * avoidForce;
            }
        }

        //Keeps the value reasonable while allowing it to be smaller than 1
        if (avoid.magnitude > 1)
            avoid = avoid.normalized;

        return avoid;
    }
}
