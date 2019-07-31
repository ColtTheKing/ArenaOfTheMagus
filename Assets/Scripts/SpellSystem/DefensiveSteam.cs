using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveSteam : Spell
{
    public float damageBoost, weakenDuration, distanceAhead;

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

    void OnCollisionStay(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Weaken(damageBoost, weakenDuration);
        }
    }
}
