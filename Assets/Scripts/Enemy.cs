using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int startingHP, attackDamage;
    public float baseSpeed, attackCooldown, attackRange, resistForce;
    //AI Movement Variables
    public float pursueDuration, pursueAheadDist, pursueRange, avoidDist, separationRadius, pursueCoefficient, avoidCoefficient, separationCoefficient, cohesionCoefficient, wanderCoefficient;
    public Material baseMaterial, weakenMaterial;

    private Health healthComp;
    private float fireTimer, slowTimer, weakTimer, stunTimer, blindTimer, pursueTimer, attacking, speed, damageTaken;
    private int flockId;
    private EnemyFlock flock;
    private Vector3 wanderLocation, pushVelocity;
    private bool alive;
    private ParticleSystem fireParticles;

    void Awake()
    {
        alive = true;
        speed = baseSpeed;
        damageTaken = 1;
    }

    void Start()
    {
        healthComp = new Health(startingHP);
        fireParticles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //Have the AI figure out what to do
        Move(DecideBehaviour());

        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0)
            {
                fireParticles.Stop();
                fireTimer = 0;
            }
        }

        if (weakTimer > 0)
        {
            weakTimer -= Time.deltaTime;

            if (weakTimer <= 0)
            {
                weakTimer = 0;
                damageTaken = 1;

                SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < meshes.Length; i++)
                    meshes[i].material = baseMaterial;
            }
        }

        if (blindTimer > 0)
        {
            blindTimer -= Time.deltaTime;
        }

        if (!healthComp.Update(Time.deltaTime))
            MarkToKill();
    }

    public Vector3 DecideBehaviour()
    {
        Vector3 move = new Vector3(0, 0, 0);

        RaycastHit objectSeen;

        int layerMask = 1 << 8;

        //If the player is in sight the enemy will pursue them for a while even after losing sight
        if (Physics.Raycast(transform.position, flock.player.GetComponentInChildren<Camera>().transform.position - transform.position, out objectSeen, pursueRange, layerMask)
            && objectSeen.transform.gameObject.GetComponentInParent<Player>() != null)
                pursueTimer = pursueDuration;
        
        //If they are still pursuing
        if(pursueTimer > 0 && blindTimer <= 0)
        {
            pursueTimer -= Time.deltaTime;

            //Attack the player if close enough and not already attacking
            if(attacking > 0)
            {
                attacking -= Time.deltaTime;
            }
            else
            {
                Vector2 playerDist = new Vector2(flock.player.GetPos().x, flock.player.GetPos().z)
                    - new Vector2(transform.position.x, transform.position.z);

                if (playerDist.magnitude <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    move = PursuePlayer();
                }
            }
        }
        else
        {
            move = Flock();
        }

        return move;
    }

    public void Init(EnemyFlock flock, int id)
    {
        this.flock = flock;
        flockId = id;
    }

    public void Damage(float damage)
    {
        Debug.Log("Enemy " + flockId + " hit for " + damage * damageTaken + " damage");

        healthComp.TakeDamage(damage * damageTaken);

        //Maybe add a hurt sound effect
    }

    public void Ignite(float dps, float duration)
    {
        healthComp.AddDOTEffect(dps, duration);

        //Add some fire effects
        fireTimer = duration;

        if(fireParticles.isStopped)
            fireParticles.Play();
    }

    public void Slow(float speedPenalty, float duration)
    {
        slowTimer = duration;
        speed = baseSpeed * speedPenalty;
    }

    public void Push(Vector3 force)
    {
        pushVelocity = force;
    }

    public void Weaken(float damageBoost, float duration)
    {
        weakTimer = duration;

        damageTaken = damageBoost;

        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material = weakenMaterial;
    }

    public void Stun(float duration)
    {
        stunTimer = duration;
    }

    public void Blind(float duration)
    {
        blindTimer = duration;
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

    private Vector3 PursuePlayer()
    {
        //get velocity of player and move towards a point in front of them
        Vector3 target = flock.player.GetPos() + flock.player.GetVelocity() * pursueAheadDist;
        target.y = 0;

        //find the direction towards that point
        Vector3 playerPursue = (target - transform.position).normalized;

        //also take into account obstacle avoidance
        Vector3 obstacleAvoid = AvoidObstacles();

        //Make the pursue be a unit vector
        Vector3 moveDir = playerPursue * pursueCoefficient + obstacleAvoid * avoidCoefficient;
        moveDir.y = 0;
        moveDir = moveDir.normalized;

        return moveDir;
    }

    private Vector3 Flock()
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

        return separation + cohesion + obstacleAvoid + wander;
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

    public void MarkToKill()
    {
        alive = false;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public bool GetAlive()
    {
        return alive;
    }

    private void Move(Vector3 moveDir)
    {
        if(stunTimer <= 0)
            transform.position += (moveDir * speed + pushVelocity) * Time.deltaTime;
        else
            transform.position += pushVelocity * Time.deltaTime;

        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer < 0)
                speed = baseSpeed;
        }

        if(stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }

        if (pushVelocity.magnitude > 0)
        {
            if (pushVelocity.magnitude < resistForce)
                pushVelocity = new Vector3(0, 0, 0);
            else
                pushVelocity -= pushVelocity.normalized * resistForce * Time.deltaTime;
        }
    }
}
