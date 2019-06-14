using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHandler
{
    public enum SpellType
    {
        ONE_QUICK,
        ONE_HEAVY,
        ONE_SPECIAL,
        TWO_DEFENSE,
        TWO_OFFENSE,
        NONE
    };

    public enum SpellElement
    {
        FIRE,
        WATER,
        EARTH,
        AIR
    };

    private SpellElement element;

    public SpellHandler(SpellElement element)
    {
        this.element = element;
    }

    public void CastSpell(SpellType type, Player player)
    {
        Debug.Log("Cast Spell: " + type);

        switch(type)
        {
            case SpellType.NONE:
                return;
        }
    }

    //Have a bunch of functions for each spell
}
