using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public TextMeshProUGUI healthPotionCountText;
    public TextMeshProUGUI manaPotionCountText;

    private void Start()
    {
        // Add default items to inventory on start.
        AddItem("HealthPotion", 3);
        AddItem("ManaPotion", 3);
        UpdateInventoryUI();
    }
    public void AddItem(string item, int count)
    {
        if (items.ContainsKey(item))
        {
           items[item] += count;
        }
        else
        {
            items.Add(item, count);
        }
        UpdateInventoryUI();
    }

    public void RemoveItem(string item, int count)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= count;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
        }
        else
        {
            Debug.Log("Item not found in inventory.");
        }
        UpdateInventoryUI();
    }

    public bool HasItem(string item)
    {
        return items.ContainsKey(item);
    }

    public bool IsEmpty()
    {
        return items.Count == 0;
    }

    public void UpdateInventoryUI()
    {
        healthPotionCountText.text = items.GetValueOrDefault("HealthPotion", 0).ToString();
        manaPotionCountText.text = items.GetValueOrDefault("ManaPotion", 0).ToString();
    }

}
