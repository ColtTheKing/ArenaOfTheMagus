using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFire : Spell
{
    public float speed, dotDuration, particleStart;
    public int damage, dotDamage;

    private Vector3 velocity;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particles)
        {
            p.Simulate(particleStart);
            p.Play();
        }
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

        transform.position += hand.transform.position;
        velocity = new Vector3(hand.transform.forward.x, 0, hand.transform.forward.z);
        velocity = velocity.normalized * speed;

        transform.eulerAngles += new Vector3(0, hand.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(damage);

            enemy.Ignite(dotDamage, dotDuration);

            Destroy(gameObject);
        }
        else if(collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Destroy(gameObject);
        }
    }
}
