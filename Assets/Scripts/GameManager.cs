using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPlayersTurn = true;
    public bool isEnemiesTurn = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EndPlayerTurn()
    {
        // The enemy gets 1 turn for every 2 turns the player gets.
        
            isPlayersTurn = false;
            isEnemiesTurn = true;
    }

    public void StartEnemyTurn()
    {
        isEnemiesTurn = true;
    }

    public void EndEnemiesTurn()
    {
        isEnemiesTurn = false;
        isPlayersTurn = true;
    }
}
