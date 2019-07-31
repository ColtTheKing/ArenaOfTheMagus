using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAir : Spell
{
    public float shoveForce, particleStart;

    private List<Enemy> enemies;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        enemies = new List<Enemy>();

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
        transform.position += new Vector3(player.playerHead.transform.position.x, 0, player.playerHead.transform.position.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == enemy)
                    return;
            }

            //Mark the enemy as having already been hit by the pulse
            enemies.Add(enemy);

            Vector3 pushDir = enemy.transform.position - transform.position;
            pushDir.y = 0;

            enemy.Push(pushDir.normalized * shoveForce);
        }
    }
}