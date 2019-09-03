using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyFire : Spell
{
    public float speed, dotDuration;
    public int damage, dotDamage;

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
    }

    public override void Cast(Player player, bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        transform.position += hand.CenterPos();
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

            enemy.Ignite(dotDamage, dotDuration);
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Destroy(gameObject);
        }
    }
}
