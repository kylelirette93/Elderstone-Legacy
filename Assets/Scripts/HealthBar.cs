using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;  // Image component for the health bar

    // Sets the fill amount based on health
    public void SetHealthBar(float amount)
    {
        Debug.Log("Setting health bar amount to: " + amount);
        healthBarImage.fillAmount = amount;
    }

    // Sets the maximum health and updates the fill amount
    public void SetHealthBarMax(float amount)
    {
        SetHealthBar(amount);
    }
}