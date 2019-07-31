using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveLightning : Spell
{
    public float dps, particleStart;

    private Player player;

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
        Vector3 headPos = player.playerHead.transform.position;
        headPos.y = transform.position.y;

        transform.position = headPos;
    }

    public override void Cast(Player player, bool left)
    {
        Vector3 headPos = player.playerHead.transform.position;
        headPos.y = transform.position.y;

        transform.position = headPos;

        this.player = player;
    }

    void OnCollisionStay(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(dps * Time.deltaTime);
        }
    }
}
