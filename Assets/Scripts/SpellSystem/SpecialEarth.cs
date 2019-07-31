using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEarth : Spell
{
    public int shieldHp;

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
        player.AddShield(shieldHp, player.rockMaterial);
    }
}