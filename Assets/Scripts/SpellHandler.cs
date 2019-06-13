using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHandler
{
    public enum SpellType
    {
        ONE_SPELL1,
        ONE_SPELL2,
        TWO_SPELL1,
        TWO_SPELL2,
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

    public void CastSpell(SpellType type)
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
