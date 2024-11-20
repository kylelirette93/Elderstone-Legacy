using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;  // Image component for the health bar

    // Sets the fill amount based on health
    public void SetHealthBar(float amount)
    {
        // Set the fill amount of the health bar image
        healthBarImage.fillAmount = amount;
    }

    // Sets the maximum health and updates the fill amount
    public void SetHealthBarMax(float amount)
    {
        // Set the max value for the health (this step is optional for Image, but it's good practice)
        // We won't need this for the fillAmount, as it will adjust based on the max health.
        SetHealthBar(amount);
    }
}