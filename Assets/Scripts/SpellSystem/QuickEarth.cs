using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickEarth : Spell
{
    public float horizontalSpeed, verticalSpeed, shoveForce, maxDist;
    public int damage;

    private Vector3 velocity;
    private float distTravelled;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Keep track of how far it has moved and don't move if it has reached its max distance
        if(distTravelled < maxDist)
        {
            float moveDist = velocity.magnitude * Time.deltaTime;

            if(distTravelled + moveDist > maxDist)
            {
                //If this movement would push it too far, only move it part of the way
                transform.position += velocity.normalized * (maxDist - distTravelled);
            }
            else
            {
                //Move the spell normally
                transform.position += velocity * Time.deltaTime;
            }

            distTravelled += moveDist;
        }
    }

    public override void Cast(Player player, bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        transform.position = new Vector3(hand.transform.position.x, 0, hand.transform.position.z);
        velocity = new Vector3(hand.transform.forward.x, 0, hand.transform.forward.z);
        velocity = velocity.normalized * horizontalSpeed;
        velocity.y += verticalSpeed;

        transform.eulerAngles += new Vector3(0, hand.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(damage);

            Vector3 shove = velocity;
            shove.y = 0;

            enemy.Push(shove.normalized * shoveForce);

            distTravelled = maxDist;

            lifespan = 0.5f;
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            distTravelled = maxDist;
        }
    }
}
