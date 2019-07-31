using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEarth : Spell
{
    public float speed, gravity, shoveForce, upwardsForce, rotationSpeed;
    public int damage;

    private Vector3 velocity;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Adjust velocity for gravity (deal with derivatives for accurate motion later)
        velocity.y -= gravity * Time.deltaTime;

        //Move the spell
        transform.position += velocity * Time.deltaTime;

        //Rotate the spell
        transform.eulerAngles += new Vector3(rotationSpeed * Time.deltaTime, 0, 0);

        if (transform.position.y <= 0)
            Destroy(gameObject);
    }

    public override void Cast(Player player, bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        transform.position += hand.transform.position;
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

            Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Destroy(gameObject);
        }
    }
}