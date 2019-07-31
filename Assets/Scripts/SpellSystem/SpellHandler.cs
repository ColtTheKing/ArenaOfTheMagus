using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHandler : MonoBehaviour
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

    public SpellElement elementLeft, elementRight;
    public QuickFire quickFire;
    public HeavyFire heavyFire;
    public SpecialFire specialFire;
    public QuickWater quickWater;
    public HeavyWater heavyWater;
    public SpecialWater specialWater;
    public QuickEarth quickEarth;
    public HeavyEarth heavyEarth;
    public SpecialEarth specialEarth;
    public QuickAir quickAir;
    public HeavyAir heavyAir;
    public SpecialAir specialAir;
    public DefensiveSteam defensiveSteam;
    public OffensiveSteam offensiveSteam;
    public DefensiveMagma defensiveMagma;
    public OffensiveMagma offensiveMagma;
    public DefensiveLightning defensiveLightning;
    public OffensiveLightning offensiveLightning;
    public DefensiveNature defensiveNature;
    public OffensiveNature offensiveNature;
    public DefensiveIce defensiveIce;
    public OffensiveIce offensiveIce;
    public DefensiveDust defensiveDust;
    public OffensiveDust offensiveDust;

    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public void CastSpell(SpellType type, Gesture.GestureHand hand)
    {
        bool left = false;

        if(hand == Gesture.GestureHand.LEFT)
            left = true;

        Spell spell;

        switch(type)
        {
            case SpellType.ONE_QUICK:
                spell = QuickSpell(left);
                break;
            case SpellType.ONE_HEAVY:
                spell = HeavySpell(left);
                break;
            case SpellType.ONE_SPECIAL:
                spell = SpecialSpell(left);
                break;
            case SpellType.TWO_DEFENSE:
                spell = DefenseSpell();
                break;
            case SpellType.TWO_OFFENSE:
                spell = OffenseSpell();
                break;
            default:
                return;
        }

        spell.Cast(player, left);
    }

    //Functions for dealing with the spell types
    private Spell QuickSpell(bool left)
    {
        Spell spell;

        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting QUICK Spell");

        switch(element)
        {
            case SpellElement.FIRE:
                spell = Instantiate(quickFire);
                break;
            case SpellElement.WATER:
                spell = Instantiate(quickWater);
                break;
            case SpellElement.EARTH:
                spell = Instantiate(quickEarth);
                break;
            case SpellElement.AIR:
                spell = Instantiate(quickAir);
                break;
            default:
                spell = Instantiate(quickFire);
                break;
        }

        return spell;
    }

    private Spell HeavySpell(bool left)
    {
        Spell spell;

        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting HEAVY Spell");

        switch (element)
        {
            case SpellElement.FIRE:
                spell = Instantiate(heavyFire);
                break;
            case SpellElement.WATER:
                spell = Instantiate(heavyWater);
                break;
            case SpellElement.EARTH:
                spell = Instantiate(heavyEarth);
                break;
            case SpellElement.AIR:
                spell = Instantiate(heavyAir);
                break;
            default:
                spell = Instantiate(quickFire);
                break;
        }

        return spell;
    }

    private Spell SpecialSpell(bool left)
    {
        Spell spell;

        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting SPECIAL Spell");

        switch (element)
        {
            case SpellElement.FIRE:
                spell = Instantiate(specialFire);
                break;
            case SpellElement.WATER:
                spell = Instantiate(specialWater);
                break;
            case SpellElement.EARTH:
                spell = Instantiate(specialEarth);
                break;
            case SpellElement.AIR:
                spell = Instantiate(specialAir);
                break;
            default:
                spell = Instantiate(quickFire);
                break;
        }

        return spell;
    }
    
    private Spell DefenseSpell()
    {
        Spell spell;

        Debug.Log("Casting DEFENSE Spell");

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        spell = Instantiate(defensiveSteam);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(defensiveMagma);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(defensiveLightning);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(defensiveSteam);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(defensiveNature);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(defensiveIce);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(defensiveMagma);
                        break;
                    case SpellElement.WATER:
                        spell = Instantiate(defensiveNature);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(defensiveDust);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(defensiveLightning);
                        break;
                    case SpellElement.WATER:
                        spell = Instantiate(defensiveIce);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(defensiveDust);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            default:
                spell = Instantiate(quickFire);
                break;
        }

        return spell;
    }

    private Spell OffenseSpell()
    {
        Spell spell;

        Debug.Log("Casting OFFENSE Spell");

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        spell = Instantiate(offensiveSteam);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(offensiveMagma);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(offensiveLightning);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(offensiveSteam);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(offensiveNature);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(offensiveIce);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(offensiveMagma);
                        break;
                    case SpellElement.WATER:
                        spell = Instantiate(offensiveNature);
                        break;
                    case SpellElement.AIR:
                        spell = Instantiate(offensiveDust);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        spell = Instantiate(offensiveLightning);
                        break;
                    case SpellElement.WATER:
                        spell = Instantiate(offensiveIce);
                        break;
                    case SpellElement.EARTH:
                        spell = Instantiate(offensiveDust);
                        break;
                    default:
                        spell = Instantiate(quickFire);
                        break;
                }
                break;
            default:
                spell = Instantiate(quickFire);
                break;
        }

        return spell;
    }
}
