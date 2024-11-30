using System;
using System.Diagnostics;

public class HealthSystem
{
    // Variables
    public int health;
    public int maxHealth = 100;
    public string healthStatus;
    public int requiredXP = 100;


    // Optional XP system variables
    public int xp = 0;
    public int level = 1;


    
    public HealthSystem()
    {
        ResetGame();
    }

    

    public void TakeDamage(int damage)
    { 
            health -= damage;

            // Clamp the health so it stays in range
            health = Math.Clamp(health, 0, maxHealth);    
    }


    public void Heal(int hp)
    {
        // Prevent negative healing input
        if (hp <= 0)
        {
            hp = 0;
        }
        UnityEngine.Debug.Log("Health before healing: " + health);
        // Take the value of health after hp has been added and clamp it
        health += hp;
        UnityEngine.Debug.Log("Health after healing: " + health);
        health = Math.Clamp(health, 0, maxHealth);
    }

    

    public void Revive()
    {
        Heal(100);
    }

    public void ResetGame()
    {
        // Reset all variables to default values
        health = maxHealth;
        xp = 0;
        level = 1;
    }


    public void IncreaseXP(int exp)
    {
        // Add the powerup xp to total xp
        UnityEngine.Debug.Log("Current XP before adding xp: " + xp);
        UnityEngine.Debug.Log("Before XP is added, " +
                "player level is: " + level);
        xp += exp;
        UnityEngine.Debug.Log("XP before resetting: " + xp);

        if (xp < 0)
        {
            xp = 0;
        }

        // Level will increment every 100xp
        requiredXP = 100;

        // Check if total xp is greater than the required xp
        while (xp >= requiredXP && level < 99)
        {
            UnityEngine.Debug.Log("After XP has been added, " +
                "player level is now: " + level);
            level++;

            // Keeps track of how much xp is left after leveling
            xp -= requiredXP;
            UnityEngine.Debug.Log("XP after resetting: " + xp);
        }

        if (level >= 99)
        {
            level = 99;
            xp = 0;
        }
    }
}