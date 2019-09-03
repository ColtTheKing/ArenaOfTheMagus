using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    private int maxHP, currentHP, shieldHP;
    private bool alive, shieldBroke;
    private float carryOverDamage, dotTimer, dotDamage;

    public Health(int startingHP)
    {
        maxHP = startingHP;
        currentHP = startingHP;
        alive = true;
        shieldHP = 0;
        dotTimer = 0;
        carryOverDamage = 0;
    }

    //Returns whether the character is alive or not
    public bool Update(float deltaTime)
    {
        if (dotTimer > 0)
        {
            float totalDamage = carryOverDamage;

            //Only calculate damage for remaining time of DOT
            if (dotTimer < deltaTime)
                totalDamage += dotTimer * dotDamage;
            else
                totalDamage += deltaTime * dotDamage;

            //Save the decimal part for later
            carryOverDamage = totalDamage % 1;

            //Take the whole number part in damage
            TakeDamage((int)totalDamage);

            dotTimer -= deltaTime;

            if (dotTimer <= 0)
            {
                dotTimer = 0;
                carryOverDamage = 0;
            }
        }
        else if (carryOverDamage > 1)
        {
            //Take the whole number part in damage
            TakeDamage((int)carryOverDamage);

            //Save the decimal part for later
            carryOverDamage = carryOverDamage % 1;
        }

        return alive;
    }

    public bool TakeDamage(float damage)
    {
        int takenDamage = (int)damage;

        carryOverDamage += damage % 1;

        if (damage <= shieldHP)
        {
            shieldHP -= takenDamage;

            if (shieldHP == 0)
                shieldBroke = true;

            return alive;
        }
        else
        {
            currentHP -= takenDamage - shieldHP;
            
            if(shieldHP != 0)
            {
                shieldHP = 0;
                shieldBroke = true;
            }
        }

        if (currentHP <= 0)
        {
            alive = false;
            Debug.Log("Character has died");
        }

        return alive;
    }

    public void Heal(int health)
    {
        currentHP += health;

        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void Shield(int health)
    {
        if (health > shieldHP)
            shieldHP = health;
    }

    public bool ShieldBroke()
    {
        if(shieldBroke)
        {
            shieldBroke = false;
            return true;
        }

        return false;
    }

    public bool Alive()
    {
        return alive;
    }

    public void AddDOTEffect(float dps, float duration)
    {
        dotTimer = duration;
        dotDamage = dps;
    }

    public int GetHealth()
    {
        return currentHP;
    }
}
