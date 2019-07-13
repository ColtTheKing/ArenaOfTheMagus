using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    private int maxHP, currentHP, shieldHP, dotDamage;
    private bool alive;
    private float carryOverDamage, dotTimer;

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
        if(dotTimer > 0)
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

            if(dotTimer <= 0)
            {
                dotTimer = 0;
                carryOverDamage = 0;
            }
        }

        return alive;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= shieldHP)
        {
            shieldHP -= damage;
            return;
        }
        else
        {
            currentHP -= damage - shieldHP;
        }

        if (currentHP <= 0)
        {
            alive = false;
            Debug.Log("Character has died");
        }
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

    public bool Alive()
    {
        return alive;
    }

    public void AddDOTEffect(float duration, int dps)
    {
        dotTimer = duration;
        dotDamage = dps;
    }
}
