using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWater : Spell
{
    public float distanceAhead, pullSpeed;

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
        Vector3 forward = player.playerHead.transform.forward;
        forward.y = 0;

        transform.position += player.transform.position + forward.normalized * distanceAhead;
        transform.eulerAngles += new Vector3(0, player.playerHead.transform.eulerAngles.y, 0);
    }

    void OnCollisionStay(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            Vector3 toVortex = transform.position - enemy.transform.position;
            toVortex.y = 0;

            enemy.transform.position += toVortex * pullSpeed * Time.deltaTime;
        }
    }
}
