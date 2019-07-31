using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveDust : Spell
{
    public float wanderDuration, particleStart;
    public int damage;

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
    }

    public override void Cast(Player player, bool left)
    {
        //Get the position between the two hands
        Vector3 betweenHands = (player.rightHand.transform.position + player.leftHand.transform.position) / 2;
        betweenHands.y = 0;

        transform.position += betweenHands;

        transform.eulerAngles += new Vector3(0, player.playerHead.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(damage);

            float duration = wanderDuration > lifespan ? wanderDuration : lifespan;

            enemy.Blind(duration);
        }
    }
}
