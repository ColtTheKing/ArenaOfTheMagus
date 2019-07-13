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

    public static float FIRE_DOT_DURATION = 4;
    public static int FIRE_DOT_DAMAGE = 1;

    public SpellElement elementLeft, elementRight;
    public Spell quickFire, heavyFire, specialFire,
        quickWater, heavyWater, specialWater,
        quickEarth, heavyEarth, specialEarth,
        quickAir, heavyAir, specialAir;

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

        switch(type)
        {
            case SpellType.ONE_QUICK:
                QuickSpell(left);
                break;
            case SpellType.ONE_HEAVY:
                HeavySpell(left);
                break;
            case SpellType.ONE_SPECIAL:
                SpecialSpell(left);
                break;
            case SpellType.TWO_DEFENSE:
                DefenseSpell();
                break;
            case SpellType.TWO_OFFENSE:
                OffenseSpell();
                break;
        }
    }

    //Functions for dealing with the spell types
    private void QuickSpell(bool left)
    {
        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting QUICK Spell");

        switch(element)
        {
            case SpellElement.FIRE:
                FireQuick(left);
                break;
            case SpellElement.WATER:
                WaterQuick(left);
                break;
            case SpellElement.EARTH:
                EarthQuick(left);
                break;
            case SpellElement.AIR:
                AirQuick(left);
                break;
        }
    }

    private void HeavySpell(bool left)
    {
        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting HEAVY Spell");

        switch (element)
        {
            case SpellElement.FIRE:
                FireHeavy(left);
                break;
            case SpellElement.WATER:
                WaterHeavy(left);
                break;
            case SpellElement.EARTH:
                EarthHeavy(left);
                break;
            case SpellElement.AIR:
                AirHeavy(left);
                break;
        }
    }

    private void SpecialSpell(bool left)
    {
        SpellElement element = left ? elementLeft : elementRight;

        Debug.Log("Casting SPECIAL Spell");

        switch (element)
        {
            case SpellElement.FIRE:
                FireSpecial(left);
                break;
            case SpellElement.WATER:
                WaterSpecial(left);
                break;
            case SpellElement.EARTH:
                EarthSpecial(left);
                break;
            case SpellElement.AIR:
                AirSpecial(left);
                break;
        }
    }
    
    private void DefenseSpell()
    {
        Debug.Log("Casting DEFENSE Spell");

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        SteamDefense();
                        break;
                    case SpellElement.EARTH:
                        MagmaDefense();
                        break;
                    case SpellElement.AIR:
                        LightningDefense();
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        SteamDefense();
                        break;
                    case SpellElement.EARTH:
                        NatureDefense();
                        break;
                    case SpellElement.AIR:
                        IceDefense();
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        MagmaDefense();
                        break;
                    case SpellElement.WATER:
                        NatureDefense();
                        break;
                    case SpellElement.AIR:
                        DustDefense();
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        LightningDefense();
                        break;
                    case SpellElement.WATER:
                        IceDefense();
                        break;
                    case SpellElement.EARTH:
                        DustDefense();
                        break;
                }
                break;
        }
    }

    private void OffenseSpell()
    {
        Debug.Log("Casting OFFENSE Spell");

        switch (elementLeft)
        {
            case SpellElement.FIRE:
                switch (elementRight)
                {
                    case SpellElement.WATER:
                        SteamOffense();
                        break;
                    case SpellElement.EARTH:
                        MagmaOffense();
                        break;
                    case SpellElement.AIR:
                        LightningOffense();
                        break;
                }
                break;
            case SpellElement.WATER:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        SteamOffense();
                        break;
                    case SpellElement.EARTH:
                        NatureOffense();
                        break;
                    case SpellElement.AIR:
                        IceOffense();
                        break;
                }
                break;
            case SpellElement.EARTH:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        MagmaOffense();
                        break;
                    case SpellElement.WATER:
                        NatureOffense();
                        break;
                    case SpellElement.AIR:
                        DustOffense();
                        break;
                }
                break;
            case SpellElement.AIR:
                switch (elementRight)
                {
                    case SpellElement.FIRE:
                        LightningOffense();
                        break;
                    case SpellElement.WATER:
                        IceOffense();
                        break;
                    case SpellElement.EARTH:
                        DustOffense();
                        break;
                }
                break;
        }
    }

    //Functions for the effects of the spells considering their type and element
    private void FireQuick(bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        Spell fireball = Instantiate(quickFire);
        fireball.transform.position = hand.transform.position;
        fireball.SetDirection(new Vector3(hand.transform.forward.x, 0, hand.transform.forward.z));
    }

    private void WaterQuick(bool left)
    {

    }

    private void EarthQuick(bool left)
    {

    }

    private void AirQuick(bool left)
    {

    }

    private void FireHeavy(bool left)
    {
        PlayerHand hand = left ? player.leftHand : player.rightHand;

        Spell fireball = Instantiate(heavyFire);
        fireball.transform.position = hand.transform.position;
        fireball.SetDirection(new Vector3(hand.transform.forward.x, 0, hand.transform.forward.z));
    }

    private void WaterHeavy(bool left)
    {

    }

    private void EarthHeavy(bool left)
    {

    }

    private void AirHeavy(bool left)
    {

    }

    private void FireSpecial(bool left)
    {
        Spell fireCone = Instantiate(specialFire);
        fireCone.transform.position = new Vector3(player.GetPos().x, 0, player.GetPos().z)
            + new Vector3(player.GetPos().x, 0, player.GetPos().z);
    }

    private void WaterSpecial(bool left)
    {

    }

    private void EarthSpecial(bool left)
    {

    }

    private void AirSpecial(bool left)
    {

    }

    private void SteamDefense()
    {

    }

    private void MagmaDefense()
    {

    }

    private void LightningDefense()
    {

    }

    private void NatureDefense()
    {

    }

    private void IceDefense()
    {

    }

    private void DustDefense()
    {

    }

    private void SteamOffense()
    {

    }

    private void MagmaOffense()
    {

    }

    private void LightningOffense()
    {

    }

    private void NatureOffense()
    {

    }

    private void IceOffense()
    {

    }

    private void DustOffense()
    {

    }
}
