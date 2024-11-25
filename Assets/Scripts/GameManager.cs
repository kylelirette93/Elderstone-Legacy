using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPlayersTurn = true;
    public int playerTurnCount = 0;
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
        playerTurnCount++;
        if (playerTurnCount == 2)
        {
            isPlayersTurn = false;
            isEnemiesTurn = true;
            playerTurnCount = 0;
        }
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
