using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveMagma : Spell
{
    public float speed, gravity, shoveForce, upwardsForce, explosionLifespan, explosionSpeed;
    public int damage;
    public Material explodedMaterial;

    private Vector3 velocity;
    private Light lavaLight;
    private bool exploded;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        exploded = false;

        lavaLight = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Adjust velocity for gravity (deal with derivatives for accurate motion later)
        velocity.y -= gravity * Time.deltaTime;

        //Move the spell
        transform.position += velocity * Time.deltaTime;

        if (transform.position.y <= 0)
            Destroy(gameObject);

        if(exploded)
        {
            float explosionIncrease = 1 + explosionSpeed * Time.deltaTime;

            transform.localScale = new Vector3(transform.localScale.x * explosionIncrease, transform.localScale.y * explosionIncrease, transform.localScale.z * explosionIncrease);
        }
    }

    public override void Cast(Player player, bool left)
    {
        //Get the position between the two hands
        Vector3 betweenHands = (player.rightHand.transform.position + player.leftHand.transform.position) / 2;

        transform.position += betweenHands;
        velocity = new Vector3(player.playerHead.transform.forward.x, 0, player.playerHead.transform.forward.z);
        velocity = velocity.normalized * speed;
        velocity.y = upwardsForce;
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

            if (!exploded)
            {
                exploded = true;
                GetComponent<MeshRenderer>().material = explodedMaterial;

                lifespan = explosionLifespan;
                gravity = 0;
                velocity = Vector3.zero;

                lavaLight.enabled = true;

                damage /= 2;
            }
        }
        else if (!exploded && collision.gameObject.GetComponent<Obstacle>() != null)
        {
            exploded = true;
            transform.localScale = new Vector3(2, 2, 2);
            GetComponent<MeshRenderer>().material = explodedMaterial;

            lifespan = explosionLifespan;
            gravity = 0;
            velocity = Vector3.zero;
        }
    }
}