using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveIce : Spell
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
    }

    public override void Cast(Player player, bool left)
    {
        //Get the position between the two hands
        Vector3 betweenHands = (player.rightHand.CenterPos() + player.leftHand.CenterPos()) / 2;
        transform.position += betweenHands;

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
