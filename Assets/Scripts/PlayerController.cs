using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Vector3Int currentPosition;
    private Vector3Int initialPosition;
    private Tilemap playerLayer;
    private Tilemap map;
    public Tile playerTile;
    private Tilemap enemyLayer;
    private Tile enemyTile;
    private AnimatedTile spellHitTile;
    private AnimatedTile swordAttackTile;

    bool isAttacking = false;
    public GameObject attackPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public HealthBar healthBar;
    public HealthBar enemyHealthBar;
    public ManaBar manaBar;
    public HealthSystem healthSystem = new HealthSystem();
    public HealthSystem manaSystem = new HealthSystem();

    EnemyController enemyController;
    Vector3Int enemyPosition;

    private void Start()
    {
        enemyController = enemyLayer.GetComponent<EnemyController>();
    }

    public void Initialize(Tilemap playerLayer, Tile playerTile, Tilemap enemyLayer, 
        Tile enemyTile, AnimatedTile spellHitTile, AnimatedTile swordAttackTile, Tilemap map)
    {
        this.playerLayer = playerLayer;
        this.playerTile = playerTile;
        this.enemyLayer = enemyLayer;
        this.enemyTile = enemyTile;
        this.spellHitTile = spellHitTile;
        this.swordAttackTile = swordAttackTile;
        this.map = map;

        // Assign player's initial position
        currentPosition = new Vector3Int(0, 0, 0);
        initialPosition = currentPosition;

        playerLayer.SetTile(initialPosition, playerTile);
    }

    public Vector3Int GetInitialPosition() 
    {         return initialPosition;
    }

    private void Update()
    {
        if (playerLayer == null || map == null || playerTile == null)
        {
            Debug.LogError("PlayerController is not properly initialized.");
            return;
        }
        if (GameManager.instance.isPlayersTurn && !isAttacking)
        {
            HandleInput();
        }
        else
        {
            return;
        }
    }




    private void HandleInput()
    {
        if (isAttacking)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePlayer(new Vector3Int(0, 1, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(new Vector3Int(-1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(new Vector3Int(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(new Vector3Int(0, -1, 0));
        }
    }

    public void ResetPlayerPosition()
    {
        // Resets player's position to initial position.
        playerLayer.SetTile(currentPosition, null);
        currentPosition = initialPosition;
        playerLayer.SetTile(currentPosition, playerTile);
    }



    private void MovePlayer(Vector3Int direction)
    {
        // Add direction to current position.
        Vector3Int newPosition = currentPosition + direction;
        if (CanMoveTo(newPosition))
        {
            // Set player's current position to new position.
            playerLayer.SetTile(currentPosition, null);
            currentPosition = newPosition;
            playerLayer.SetTile(currentPosition, playerTile);
            GameManager.instance.EndPlayerTurn();
        }
    }

    private bool CanMoveTo(Vector3Int position)
    {
        // Takes where player moves to as parameter.

        if (map == null || playerLayer == null)
        {
            Debug.LogError("Map or playerLayer is not assigned.");
            return false;
        }

        // Get tile as position on the map.
        TileBase mapTile = map.GetTile(position);
        TileBase playerLayerTile = playerLayer.GetTile(position);
        TileBase enemyLayerTile = enemyLayer.GetTile(position);

        // Check if the position is empty on both layers.
        if (mapTile == null && playerLayerTile == null && !isAttacking)
        {
            // If position is empty player can move.
            return true;
        }

        // Apply rules when comparing player's new position.
        // Player cannot walk through walls, chests or houses.
        if (mapTile == map.GetComponent<MapGenerator>().borderTile)
        {
            return false;
        }
        if (mapTile == map.GetComponent<MapGenerator>().manaTile)
        { 
            IncrementMana();
            // Remove the mana potion and set as ground.
            TileBase groundTile = map.GetComponent<MapGenerator>().groundTile;
            map.SetTile(position, groundTile);
            return false;
        }
        if (mapTile.name == "HouseTile")
        {
            return false;
        }
        if (enemyLayerTile == enemyController.enemyTile)
        {
            isAttacking = true;
            attackPanel.SetActive(true);
            Time.timeScale = 0;
            return false;
        }
        // If no conditions are met, player can move.
        return true;
    }

    
    public void AttackEnemy(string buttonName)
    {
        // Check for button press.
        switch (buttonName)
        {
            case "SwingSword":
                SwordHit();
                ApplyEnemyDamage();
                break;
            case "SpellAttack":
                SpellHit();
                ApplyEnemyDamage();
                DecrementMana();
                enemyController.UpdateHealthText();
                break;
        }
        // Disable the panel after attacking, reset time scale and end the player's turn.
        attackPanel.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.EndPlayerTurn();
        GameManager.instance.StartEnemyTurn();
        isAttacking = false;
    }

    void ApplyEnemyDamage()
    {
        enemyController.healthSystem.TakeDamage(20);
        float healthPercentage = (float)enemyController.healthSystem.health / enemyController.healthSystem.maxHealth;
        enemyHealthBar.SetHealthBar(healthPercentage);
        enemyController.UpdateHealthText();
    }

    void IncrementMana()
    {
        manaSystem.Heal(20);
        float manaPercentage = (float)manaSystem.health / manaSystem.maxHealth;
        manaBar.SetManaBar(manaPercentage);
        UpdateManaText();
    }

    void DecrementMana()
    {
        manaSystem.TakeDamage(20);
        float manaPercentage = (float)manaSystem.health / manaSystem.maxHealth;
        manaBar.SetManaBar(manaPercentage);
        UpdateManaText();
    }

    public void SwordHit()
    {
        enemyPosition = enemyController.GetEnemyPosition();
        AnimatedTile modifiedSwordAttackTile = ScriptableObject.CreateInstance<AnimatedTile>();
        Debug.Log("Created scriptable object.");
        modifiedSwordAttackTile.m_AnimatedSprites = swordAttackTile.m_AnimatedSprites;
        modifiedSwordAttackTile.m_AnimationStartFrame = swordAttackTile.m_AnimationStartFrame;
        modifiedSwordAttackTile.m_MinSpeed = 8f;
        modifiedSwordAttackTile.m_MaxSpeed = 8f;
        float endTime =
            modifiedSwordAttackTile.m_AnimatedSprites.Length / modifiedSwordAttackTile.m_MaxSpeed;
        enemyLayer.SetTile(enemyPosition, modifiedSwordAttackTile);

        

        StartCoroutine(ClearTileAfterAnimation(enemyPosition, modifiedSwordAttackTile, endTime));
    }

    public void SpellHit()
    {
        enemyPosition = enemyController.GetEnemyPosition();
        AnimatedTile modifiedSpellHitTile = ScriptableObject.CreateInstance<AnimatedTile>();
        modifiedSpellHitTile.m_AnimatedSprites = spellHitTile.m_AnimatedSprites;
        modifiedSpellHitTile.m_AnimationStartFrame = spellHitTile.m_AnimationStartFrame;
        modifiedSpellHitTile.m_MinSpeed = 4f;
        modifiedSpellHitTile.m_MaxSpeed = 4f;
        modifiedSpellHitTile.m_AnimationStartTime = Time.time;
        float endTime = 
            modifiedSpellHitTile.m_AnimatedSprites.Length / modifiedSpellHitTile.m_MaxSpeed;
        enemyLayer.SetTile(enemyPosition, modifiedSpellHitTile);

        StartCoroutine(ClearTileAfterAnimation(enemyPosition, modifiedSpellHitTile, endTime));
    }

    IEnumerator ClearTileAfterAnimation(Vector3Int position, AnimatedTile tile, float endTime)
    {
        yield return new WaitForSeconds(endTime);

        int lastCheckedEnemyHealth = enemyController.healthSystem.health;
        if (lastCheckedEnemyHealth <= 0)
        {
            enemyController.isDead = true;
            TileBase groundTile = map.GetComponent<MapGenerator>().groundTile;
            enemyLayer.SetTile(position, groundTile);
        }
        else
        {
            enemyLayer.SetTile(position, enemyTile);
        }
    }

    public void UpdateHealthText()
    {
        healthText.text = "HP: " + healthSystem.health.ToString();
    }

    public void UpdateManaText()
    {
        manaText.text = "MP: " + manaSystem.health.ToString();
    }
}