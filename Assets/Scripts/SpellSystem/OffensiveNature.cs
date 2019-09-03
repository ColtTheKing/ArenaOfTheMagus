using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveNature : Spell
{
    public SpellEffect spellVisualization;
    public float rootDuration;
    public int damage;

    private List<Enemy> enemies;
    private Vector3 startingPos;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        enemies = new List<Enemy>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Pick the closest enemy after allowing collisions to happen
        if (lifespan < 0.5f && enemies.Count > 0 && startingPos != null)
            EffectEnemy();
    }

    public override void Cast(Player player, bool left)
    {
        //Get the position between the two hands
        Vector3 betweenHands = player.rightHand.CenterPos() + (player.leftHand.CenterPos() - player.rightHand.CenterPos());
        betweenHands.y = 0;

        transform.position += betweenHands;

        startingPos = betweenHands;

        transform.eulerAngles += new Vector3(0, player.playerHead.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemies.Add(enemy);
        }
    }

    private void EffectEnemy()
    {
        Enemy e = enemies[0];
        float closestEnemyDist = (e.transform.position - startingPos).magnitude;

        for (int i = 1; i < enemies.Count; i++)
        {
            float enemyDist = (enemies[i].transform.position - startingPos).magnitude;

            if (enemyDist < closestEnemyDist)
            {
                closestEnemyDist = enemyDist;
                e = enemies[i];
            }
        }

        e.Stun(rootDuration);
        e.Damage(damage);

        SpellEffect effect = Instantiate(spellVisualization);
        effect.EffectTarget(e, rootDuration);

        Destroy(gameObject);
    }
}
