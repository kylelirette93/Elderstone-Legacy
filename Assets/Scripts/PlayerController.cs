using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Vector3Int currentPosition;
    private Vector3Int initialPosition;
    Vector3Int enemyPosition;
    bool isEnemies;
    public bool isAttacking = false;
    bool canCast = true;

    public Tilemap playerLayer;
    public Tilemap map;
    public Tile playerTile;
    public Tile openDoorTile;
    public Tile groundTile;
    public Tilemap enemyLayer;
    private Tile enemyTile;
    private AnimatedTile spellHitTile;
    private AnimatedTile swordAttackTile;
    public GameObject attackPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI combatText;
    public HealthBar healthBar;
    public HealthBar enemyHealthBar;
    public ManaBar manaBar;
    public HealthSystem healthSystem = new HealthSystem();
    public HealthSystem manaSystem = new HealthSystem();
    EnemyController enemyController;
    public GameObject gameOverPanel;

    public Inventory inventory;

    // Variables for win condition.
    int killedEnemies = 0;
    public TextMeshProUGUI killText;

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

    private void Start()
    {
        enemyController = enemyLayer.GetComponent<EnemyController>();
        inventory = GetComponent<Inventory>();
        StartCoroutine(ClearCombatText(0f));
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

        currentPosition = GameManager.instance.FindEntryTilePosition(map);

        playerLayer.SetTile(currentPosition, playerTile);
    }

    public Vector3Int GetInitialPosition()
    {
        return initialPosition;
    }

    public Vector3Int GetCurrentPosition()
    {
        return currentPosition;
    }

    public void SetPlayerPosition(Vector3Int entryTilePos, Tilemap player)
    {
        player.SetTile(entryTilePos, playerTile);
    }

    IEnumerator ClearCombatText(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Debug.Log("Clearing combat text.");
        combatText.text = "";
    }

    public void UpdateCombatTextMissed(string attacker)
    {
        combatText.text = $"{attacker} has missed the player!";
        // Debug.Log("Updating combat text. Enemy missed.");
    }

    public void UpdateCombatText(string attacker, int damage)
    {
        combatText.text = $"{attacker} attacked for {damage} damage!";
        // Debug.Log("Updating combat text. Enemy attacks player.");
    }

    

    private void Update()
    {
        if (killedEnemies >= 5)
        {
            StartCoroutine(GameManager.instance.WinGame(2f));
        }

        if (playerLayer == null || map == null || playerTile == null)
        {
            // Debug.LogError("PlayerController is not properly initialized.");
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

        // Check current mana to see if player can cast spell.
        int lastCheckedMana = manaSystem.health;

        if (lastCheckedMana < 20)
        {
            canCast = false;
        }
        else
        {
            canCast = true;
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            UseHealthPotion();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UseManaPotion();
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

            isEnemies = CheckForEnemies(enemyLayer);

            if (isEnemies)
            {
                GameManager.instance.EndPlayerTurn();
            }
            else
            {
                // Player's turn should continue if there are no enemies left.
            }
        }
    }

    private bool CanMoveTo(Vector3Int position)
    {
        // Takes where player moves to as parameter.

        if (map == null || playerLayer == null)
        {
            // Debug.LogError("Map or playerLayer is not assigned yet.");
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
            inventory.AddItem("ManaPotion", 1);
            TileBase groundTile = map.GetComponent<MapGenerator>().groundTile;
            map.SetTile(position, groundTile);
            return false;
        }

        if (mapTile == map.GetComponent<MapGenerator>().healthTile)
        {
            inventory.AddItem("HealthPotion", 1);
            TileBase groundTile = map.GetComponent<MapGenerator>().groundTile;
            map.SetTile(position, groundTile);
            return false;
        }

        if (mapTile == map.GetComponent<MapGenerator>().doorTile)
        {
            if (!GameManager.instance.AreEnemiesRemaining())
            {
                GameManager.instance.LoadNextLevel(ref map, ref enemyLayer, ref playerLayer);
                StartCoroutine(SetPlayerPositionAfterDelay(0.1f));
                return true;
            }
            else
            {
                // Player cannot move, door is locked. 
                return false;
            }
        }

        if (mapTile.name == "HouseTile")
        {
            return false;
        }

        if (enemyLayerTile == enemyController.enemyTile && GameManager.instance.isPlayersTurn)
        {
            // Trigger combat if player walks into enemy.
            isAttacking = true;
            attackPanel.SetActive(true);
            Time.timeScale = 1;
            return false;
        }
        // If no conditions are met, player can move.
        return true;
    }

    private IEnumerator SetPlayerPositionAfterDelay(float delay)
    {
        // Player should spawn on the entry tile. Needs coroutine because of dependencies.
        yield return new WaitForSeconds(delay);
        Vector3Int entryTilePosition = GameManager.instance.FindEntryTilePosition(map);
        playerLayer.SetTile(currentPosition, null);
        playerLayer.SetTile(entryTilePosition, playerTile);
        currentPosition = entryTilePosition;

        // Debug.Log("Player position after moving through door: " + currentPosition);
        // Debug.Log("Should be" + entryTilePosition);
    }



    public void AttackEnemy(string buttonName)
    {
        // Check for button press.
        switch (buttonName)
        {
            case "SwingSword":
                SwordHit();
                int swordDamage = 20;
                UpdateCombatText(this.tag, swordDamage);
                ApplyEnemyDamage(swordDamage);
                StartCoroutine(ClearCombatText(0.5f));
                break;
            case "SpellAttack":
                if (canCast)
                {
                    SpellHit();
                    int spellDamage = 30;
                    UpdateCombatText(this.tag, spellDamage);
                    ApplyEnemyDamage(spellDamage);
                    DecrementMana();
                    enemyController.UpdateHealthText();
                    StartCoroutine(ClearCombatText(0.5f));
                }
                else
                {
                    combatText.text = "Not enough mana!";
                    StartCoroutine(ClearCombatText(0.5f));
                    Debug.Log("Combat text cleared.");
                }
                break;
        }

        // Disable the panel after attacking, reset time scale and end the player's turn.
        isAttacking = false;
        attackPanel.SetActive(false);
        Time.timeScale = 1;

        if (GameManager.instance.AreEnemiesRemaining())
        {
            GameManager.instance.EndPlayerTurn();
            GameManager.instance.StartEnemyTurn();
        }
        else
        {
            // If no enemies remain, player can continue.
            GameManager.instance.EndEnemiesTurn();
        }
    }



    bool CheckForEnemies(Tilemap enemyLayer)
    {
        return GameManager.instance.AreEnemiesRemaining();
    }

    void ApplyEnemyDamage(int damage)
    {
        // Apply damage to the enemy.
        enemyController.healthSystem.TakeDamage(damage);

        // Update enemy health bar with float value for fill amount.
        float healthPercentage = (float)enemyController.healthSystem.health / enemyController.healthSystem.maxHealth;
        enemyHealthBar.SetHealthBar(healthPercentage);

        // Update enemy health text.
        enemyController.UpdateHealthText();

        // Check if enemy is dead, if so deactivate health bar.
        if (enemyController.healthSystem.health <= 0f)
        {
            killedEnemies++;
            UpdateKillText();
            enemyController.healthBar.gameObject.SetActive(false);
        }
    }

    void UpdateKillText()
    {
        killText.text = $"Killed: {killedEnemies}/5";
    }

    void UseManaPotion()
    {
        // Player used a mana potion.
        inventory.RemoveItem("ManaPotion", 1);
        manaSystem.Heal(30);

        float manaPercentage = (float)manaSystem.health / manaSystem.maxHealth;
        manaBar.SetManaBar(manaPercentage);

        UpdateManaText();
    }

    void DecrementMana()
    {
        // Player cast a spell that cost 20 mana.
        manaSystem.TakeDamage(20);

        float manaPercentage = (float)manaSystem.health / manaSystem.maxHealth;
        manaBar.SetManaBar(manaPercentage);

        UpdateManaText();
    }

    void UseHealthPotion()
    {
        // Player used a health potion.
        inventory.RemoveItem("HealthPotion", 1);
        healthSystem.Heal(30);

        float healthPercentage = (float)healthSystem.health / healthSystem.maxHealth;
        healthBar.SetHealthBar(healthPercentage);

        UpdateHealthText();
    }

    public void SwordHit()
    {
        // Get the enemy's position.
        enemyPosition = enemyController.GetEnemyPosition();

        // Create seperate instance of sword attack animated tile.
        AnimatedTile modifiedSwordAttackTile = ScriptableObject.CreateInstance<AnimatedTile>();
        // Debug.Log("Created scriptable object.");

        // Modify the sword attack animation.
        modifiedSwordAttackTile.m_AnimatedSprites = swordAttackTile.m_AnimatedSprites;
        modifiedSwordAttackTile.m_AnimationStartFrame = swordAttackTile.m_AnimationStartFrame;
        modifiedSwordAttackTile.m_MinSpeed = 8f;
        modifiedSwordAttackTile.m_MaxSpeed = 8f;
        float endTime =
            modifiedSwordAttackTile.m_AnimatedSprites.Length / modifiedSwordAttackTile.m_MaxSpeed;

        // Set the animated tile to the enemy's position.
        enemyLayer.SetTile(enemyPosition, modifiedSwordAttackTile);

        // Start the coroutine to replace or clear tile after animation ends.
        StartCoroutine(ClearTileAfterAnimation(enemyPosition, modifiedSwordAttackTile, endTime));
    }

    public void SpellHit()
    {
        // Pretty much the same as sword hit logic.
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
        // Wait until animation ends.
        yield return new WaitForSeconds(endTime);

        // Check if enemy is dead.
        int lastCheckedEnemyHealth = enemyController.healthSystem.health;
        if (lastCheckedEnemyHealth <= 0)
        {
            // Update enemy controller and replace enemy with ground tile.
            enemyController.isDead = true;
            TileBase groundTile = map.GetComponent<MapGenerator>().groundTile;
            enemyLayer.SetTile(position, groundTile);

            // Update game manager to remove remaining enemy from list.
            GameManager.instance.OnEnemyDeath(enemyController);
            GameManager.instance.DeregisterEnemy(enemyController);
        }
        else
        {
            // If enemy isn't dead, reset tile to enemy tile.
            enemyLayer.SetTile(position, enemyTile);
        }
    }

    public void Die()
    {
        // Handle player death.
        healthBar.gameObject.SetActive(false);
        playerLayer.SetTile(currentPosition, null);
        enemyController.Respawn();
        Invoke("GameOver", 2f);
    }

    void GameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    public void RestartButton()
    {
        killedEnemies = 0;
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.LoadNextLevel(ref map, ref enemyLayer, ref playerLayer);
        healthSystem.Heal(healthSystem.maxHealth);
        healthBar.gameObject.SetActive(true);
        UpdateHealthText();
        manaSystem.Heal(manaSystem.maxHealth);
        UpdateManaText();
        StartCoroutine(SetPlayerPositionAfterDelay(0.1f));
    }

    public void QuitButton()
    {
        Application.Quit();
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