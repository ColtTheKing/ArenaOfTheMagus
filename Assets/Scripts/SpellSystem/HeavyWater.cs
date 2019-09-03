using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyWater : Spell
{
    public float speed, slowAmount, slowDuration;
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

        //Move the spell
        transform.position += velocity * Time.deltaTime;

        //If wave is nearing the end of its lifespan make it recede into the ground
        if (lifespan <= 1)
            transform.position += new Vector3(0, -1, 0) * Time.deltaTime;
    }

    public override void Cast(Player player, bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        transform.position += new Vector3(hand.CenterPos().x, 0, hand.CenterPos().z);
        velocity = new Vector3(player.playerHead.transform.forward.x, 0, player.playerHead.transform.forward.z);
        velocity = velocity.normalized * speed;

        transform.eulerAngles += new Vector3(0, player.playerHead.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(damage);

            enemy.Slow(slowAmount, slowDuration);
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Destroy(gameObject);
        }
    }
}