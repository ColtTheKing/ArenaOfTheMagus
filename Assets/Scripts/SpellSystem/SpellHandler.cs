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
    public QuickFire quickFire; //0
    public HeavyFire heavyFire; //1
    public SpecialFire specialFire; //2
    public QuickWater quickWater; //3
    public HeavyWater heavyWater; //4
    public SpecialWater specialWater; //5
    public QuickEarth quickEarth; //6
    public HeavyEarth heavyEarth; //7
    public SpecialEarth specialEarth; //8
    public QuickAir quickAir; //9
    public HeavyAir heavyAir; //10
    public SpecialAir specialAir; //11
    public DefensiveSteam defensiveSteam; //12
    public OffensiveSteam offensiveSteam; //13
    public DefensiveMagma defensiveMagma; //14
    public OffensiveMagma offensiveMagma; //15
    public DefensiveLightning defensiveLightning; //16
    public OffensiveLightning offensiveLightning; //17
    public DefensiveNature defensiveNature; //18
    public OffensiveNature offensiveNature; //19
    public DefensiveIce defensiveIce; //20
    public OffensiveIce offensiveIce; //21
    public DefensiveDust defensiveDust; //22
    public OffensiveDust offensiveDust; //23

    private Player player;
    private List<float> timers;

    void Start()
    {
        player = GetComponent<Player>();

        timers = new List<float>();

        for (int i = 0; i < 24; i++)
            timers.Add(0f);
    }

    void Update()
    {
        for(int i = 0; i < timers.Count; i++)
        {
            if (timers[i] > 0)
                timers[i] -= Time.deltaTime;
        }
    }

    public void CastSpell(SpellType type, Gesture.GestureHand hand)
    {
        bool left = false;

        if(hand == Gesture.GestureHand.LEFT)
            left = true;

        Spell spell = null;

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
        }

        if(spell != null)
            spell.Cast(player, left);
    }

    //Functions for dealing with the spell types
    private Spell QuickSpell(bool left)
    {
        Spell spell = null;

        SpellElement element = left ? elementLeft : elementRight;

        switch(element)
        {
            case SpellElement.FIRE:
                if (timers[0] <= 0)
                {
                    spell = Instantiate(quickFire);
                    timers[0] = quickFire.cooldown; 
                }
                break;
            case SpellElement.WATER:
                if (timers[3] <= 0)
                {
                    spell = Instantiate(quickWater);
                    timers[3] = quickWater.cooldown; 
                }
                break;
            case SpellElement.EARTH:
                if (timers[6] <= 0)
                {
                    spell = Instantiate(quickEarth);
                    timers[6] = quickEarth.cooldown;
                }
                break;
            case SpellElement.AIR:
                if (timers[9] <= 0)
                {
                    spell = Instantiate(quickAir);
                    timers[9] = quickAir.cooldown;
                }
                break;
        }

        return spell;
    }

    private Spell HeavySpell(bool left)
    {
        Spell spell = null;

        SpellElement element = left ? elementLeft : elementRight;

        switch (element)
        {
            case SpellElement.FIRE:
                if (timers[1] <= 0)
                {
                    spell = Instantiate(heavyFire);
                    timers[1] = heavyFire.cooldown;
                }
                break;
            case SpellElement.WATER:
                if (timers[4] <= 0)
                {
                    spell = Instantiate(heavyWater);
                    timers[4] = heavyWater.cooldown;
                }
                break;
            case SpellElement.EARTH:
                if (timers[7] <= 0)
                {
                    spell = Instantiate(heavyEarth);
                    timers[7] = heavyEarth.cooldown;
                }
                break;
            case SpellElement.AIR:
                if (timers[10] <= 0)
                {
                    spell = Instantiate(heavyAir);
                    timers[10] = heavyAir.cooldown;
                }
                break;
        }

        return spell;
    }

    private Spell SpecialSpell(bool left)
    {
        Spell spell = null;

        SpellElement element = left ? elementLeft : elementRight;

        switch (element)
        {
            case SpellElement.FIRE:
                if (timers[2] <= 0)
                {
                    spell = Instantiate(specialFire);
                    timers[2] = specialFire.cooldown;
                }
                break;
            case SpellElement.WATER:
                if (timers[5] <= 0)
                {
                    spell = Instantiate(specialWater);
                    timers[5] = specialWater.cooldown;
                }
                break;
            case SpellElement.EARTH:
                if (timers[8] <= 0)
                {
                    spell = Instantiate(specialEarth);
                    timers[8] = specialEarth.cooldown;
                }
                break;
            case SpellElement.AIR:
                if (timers[11] <= 0)
                {
                    spell = Instantiate(specialAir);
                    timers[11] = specialAir.cooldown;
                }
                break;
        }

        return spell;
    }
    
    private Spell DefenseSpell()
    {
        Spell spell = null;

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        if (timers[12] <= 0)
                        {
                            spell = Instantiate(defensiveSteam);
                            timers[12] = defensiveSteam.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[14] <= 0)
                        {
                            spell = Instantiate(defensiveMagma);
                            timers[14] = defensiveMagma.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[16] <= 0)
                        {
                            spell = Instantiate(defensiveLightning);
                            timers[16] = defensiveLightning.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[12] <= 0)
                        {
                            spell = Instantiate(defensiveSteam);
                            timers[12] = defensiveSteam.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[18] <= 0)
                        {
                            spell = Instantiate(defensiveNature);
                            timers[18] = defensiveNature.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[20] <= 0)
                        {
                            spell = Instantiate(defensiveIce);
                            timers[20] = defensiveIce.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[14] <= 0)
                        {
                            spell = Instantiate(defensiveMagma);
                            timers[14] = defensiveMagma.cooldown;
                        }
                        break;
                    case SpellElement.WATER:
                        if (timers[18] <= 0)
                        {
                            spell = Instantiate(defensiveNature);
                            timers[18] = defensiveNature.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[22] <= 0)
                        {
                            spell = Instantiate(defensiveDust);
                            timers[22] = defensiveDust.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[16] <= 0)
                        {
                            spell = Instantiate(defensiveLightning);
                            timers[16] = defensiveLightning.cooldown;
                        }
                        break;
                    case SpellElement.WATER:
                        if (timers[20] <= 0)
                        {
                            spell = Instantiate(defensiveIce);
                            timers[20] = defensiveIce.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[22] <= 0)
                        {
                            spell = Instantiate(defensiveDust);
                            timers[22] = defensiveDust.cooldown;
                        }
                        break;
                }
                break;
        }

        return spell;
    }

    private Spell OffenseSpell()
    {
        Spell spell = null;

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        if (timers[13] <= 0)
                        {
                            spell = Instantiate(offensiveSteam);
                            timers[13] = offensiveSteam.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[15] <= 0)
                        {
                            spell = Instantiate(offensiveMagma);
                            timers[15] = offensiveMagma.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[17] <= 0)
                        {
                            spell = Instantiate(offensiveLightning);
                            timers[17] = offensiveLightning.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[13] <= 0)
                        {
                            spell = Instantiate(offensiveSteam);
                            timers[13] = offensiveSteam.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[19] <= 0)
                        {
                            spell = Instantiate(offensiveNature);
                            timers[19] = offensiveNature.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[21] <= 0)
                        {
                            spell = Instantiate(offensiveIce);
                            timers[21] = offensiveIce.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[15] <= 0)
                        {
                            spell = Instantiate(offensiveMagma);
                            timers[15] = offensiveMagma.cooldown;
                        }
                        break;
                    case SpellElement.WATER:
                        if (timers[19] <= 0)
                        {
                            spell = Instantiate(offensiveNature);
                            timers[19] = offensiveNature.cooldown;
                        }
                        break;
                    case SpellElement.AIR:
                        if (timers[23] <= 0)
                        {
                            spell = Instantiate(offensiveDust);
                            timers[23] = offensiveDust.cooldown;
                        }
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        if (timers[17] <= 0)
                        {
                            spell = Instantiate(offensiveLightning);
                            timers[17] = offensiveLightning.cooldown;
                        }
                        break;
                    case SpellElement.WATER:
                        if (timers[21] <= 0)
                        {
                            spell = Instantiate(offensiveIce);
                            timers[21] = offensiveIce.cooldown;
                        }
                        break;
                    case SpellElement.EARTH:
                        if (timers[23] <= 0)
                        {
                            spell = Instantiate(offensiveDust);
                            timers[23] = offensiveDust.cooldown;
                        }
                        break;
                }
                break;
        }

        return spell;
    }
}
