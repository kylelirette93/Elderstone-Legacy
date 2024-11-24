using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Image manaBarImage;  // Image component for the health bar

    // Sets the fill amount based on health
    public void SetManaBar(float amount)
    {
        Debug.Log("Setting health bar amount to: " + amount);
        manaBarImage.fillAmount = amount;
    }

    // Sets the maximum health and updates the fill amount
    public void SetHealthBarMax(float amount)
    {
        SetManaBar(amount);
    }
}