using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveIce : Spell
{
    public float shoveForce, shoveCutoff, distanceAhead;
    public int damage;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Cast(Player player, bool left)
    {
        Vector3 positionInFront = player.transform.position + player.playerHead.transform.forward * distanceAhead;
        positionInFront.y = 0;

        transform.position += positionInFront;
        transform.eulerAngles += new Vector3(0, player.playerHead.transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Damage(damage);

            Vector3 pushDir = enemy.transform.position - transform.position;
            pushDir.y = 0;
            pushDir.Normalize();

            enemy.Push(pushDir * shoveForce);
        }
    }

    private Vector3 CalculatePush(Vector3 enemyPos)
    {
        //Get an vector perpendicular to the spell
        float perpRot = transform.eulerAngles.y + 90;

        Vector3 perpendicular = new Vector3(Mathf.Sin(perpRot * Mathf.Deg2Rad), 0, Mathf.Cos(perpRot * Mathf.Deg2Rad));
        perpendicular.x *= Mathf.Sin(perpRot * Mathf.Deg2Rad);
        perpendicular.z *= Mathf.Cos(perpRot * Mathf.Deg2Rad);

        //Get the vector from the enemy to the spell's center
        Vector3 toEnemy = enemyPos - transform.position;

        //Find the amount that the toEnemy vector goes in the direction of the perpendicular
        float distFromCenter = Vector3.Dot(toEnemy, perpendicular);

        //Make the perpendicular point to the side of the spell the enemy is on
        if(distFromCenter < 0)
            perpendicular *= -1;

        //Determine how hard to push based on how close they are to the center
        float magnitude = shoveCutoff - Mathf.Abs(distFromCenter);

        return perpendicular * magnitude;
    }
}