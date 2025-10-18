using System;

public class HealthSystem
{
    // Variables
    public int health;
    public int maxHealth = 100;
    public string healthStatus;
    public int requiredXP = 100;
    
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
        //UnityEngine.Debug.Log("Health before healing: " + health);
        // Take the value of health after hp has been added and clamp it
        health += hp;
        health = Math.Clamp(health, 0, maxHealth);
        //UnityEngine.Debug.Log("Health after healing: " + health);
    }

    

    public void Revive()
    {
        Heal(100);
    }

    public void ResetGame()
    {
        // Reset all variables to default values
        health = maxHealth;
    }
}

[Serializable] 
public class PlayerSettings
{
    public int health;
    public int maxHealth = 100;

}