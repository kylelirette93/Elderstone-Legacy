using System;
using System.Diagnostics;

public class HealthSystem
{
    // Variables
    public int health;
    const int maxHealth = 100;
    public string healthStatus;
    public int shield;
    const int maxShield = 100;
    private int _lives;
    public int lives
    {
        get { return _lives; }
        set
        {
            _lives = value;
        }
    }
    public int requiredXP = 100;


    // Optional XP system variables
    public int xp = 0;
    public int level = 1;


    public string HealthStatus(int hp)
    {
        return hp switch
        {
            > 90 => "Perfect Health",
            > 75 => "Healthy",
            > 50 => "Hurt",
            > 25 => "Badly Hurt",
            _ => "Imminent Danger",
        };
    }
    public HealthSystem()
    {
        ResetGame();
    }

    public string ShowHUD()
    {
        healthStatus = HealthStatus(health);
        // Implement HUD display
        return $"Health: {health}" +
            $"\nShield: {shield}" +
            $"\nLives: {lives}" +
            $"\nHealth Status: {healthStatus}" +
            $"\nLevel: {level}" +
            $"\nXP: {xp}" +
            $"\nRequired XP to Level up: {requiredXP}";
    }

    public void TakeDamage(int damage)
    {
        UnityEngine.Debug.Log("Health and Shield before taking damage:" +
            "Health: " + health + "Shield: " + shield);
        // Prevent negative damage input
        if (damage < 0)
        {
            damage = 0;
        }

        if (damage > shield)
        {
            // Remaining damage after the shield absorbs damage
            int remainingDamage = damage - shield;
            // Shield should be fully depleted
            shield = 0;
            UnityEngine.Debug.Log("Health and shield status now that " +
                "shield is depleted: " + "Health: " + health + " Shield: " + shield);
            health -= remainingDamage;

            // Clamp the health so it stays in range
            health = Math.Clamp(health, 0, maxHealth);
        }

        else
        {
            shield -= damage;

            // Clamp the shield so it stays in range
            shield = Math.Clamp(shield, 0, maxShield);
        }

        UnityEngine.Debug.Log("Health and Shield after taking damage:" +
            "Health: " + health + "Shield: " + shield);

        if (health <= 0 && lives > 0)
        {
            UnityEngine.Debug.Log("Revive function is called.");
            Revive();
        }
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

    public void RegenerateShield(int hp)
    {
        // Prevent negative regen values
        if (hp <= 0)
        {
            hp = 0;
        }
        // Clamp the shield just like health
        UnityEngine.Debug.Log("Shield before regenerating: " + shield);
        shield += hp;
        UnityEngine.Debug.Log("Shield after regenerating: " + shield);
        shield = Math.Clamp(shield, 0, maxShield);
    }

    public void Revive()
    {
        Heal(100);
        RegenerateShield(100);
        lives--;
    }

    public void ResetGame()
    {
        // Reset all variables to default values
        health = maxHealth;
        shield = maxShield;
        lives = 3;
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