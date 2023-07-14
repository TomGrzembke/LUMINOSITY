using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the battle and playerActions
/// </summary>
public class BattleController : MonoBehaviour
{
    #region Variables
    [Space]
    [Header("Variables")]
    /// <summary>
    /// Used for counting the roundLength of the enemy round. Includes the start delay
    /// </summary>
    [SerializeField] float enemyRoundLength;

    /// <summary>
    /// The maximum amount of time that the enemy round will take. Includes the start delay
    /// </summary>
    [SerializeField] float maxEnemyRoundLength;

    /// <summary>
    /// The maximum amount of bullets/strikes that the player is abled to fire during his attack phase
    /// </summary>
    [SerializeField] float maxPlayerBulletAmount;

    /// <summary>
    /// The amount of bullets/strikes that the player fired during attack phase
    /// </summary>
    [SerializeField] float playerBulletShot;

    /// <summary>
    /// The roundCounter int, which is used for keeping track of the current round
    /// </summary>
    [SerializeField] int roundCounter;

    /// <summary>
    /// An enum that contains the possible states for the battle
    /// </summary>
    public enum TurnState
    {
        enemyTurn,
        playerTurn,
        keianTurn
    }
    /// <summary>
    /// The current state of the battle
    /// </summary>
    [SerializeField] TurnState currenGameState = TurnState.playerTurn;

    #endregion

    #region Access
    [Space]
    [Header("Access")]
    [SerializeField] Texture2D normalCursor;
    [SerializeField] Texture2D turnCursor;
    [SerializeField] CursorLightBehavior cursorLightBehavior;
    /// <summary>
    /// The Behavior script of keian, he has a chance to glow red when the enemy attacks
    /// </summary>
    [SerializeField] KeianBattleBehavior keianBattleBehavior;

    /// <summary>
    /// This is optional to assign, if not the enemy behavior wont be used
    /// </summary>
    [SerializeField] EnemyBehavior enemyBehavior;

    /// <summary>
    /// The TextMeshProUGUI for counting the rounds
    /// </summary>
    [SerializeField] TextMeshProUGUI countdownText;

    /// <summary>
    /// The gameObject of the player 
    /// </summary>
    [SerializeField] GameObject player;

    /// <summary>
    /// The prefab of the playerSlash7Bullet/Projectile
    /// </summary>
    [SerializeField] GameObject playerSlashBullet;

    /// <summary>
    /// The Transform for the playerProjectileOrigin which is used for instancing the slashe in isAttacking phase
    /// </summary>
    [SerializeField] Transform playerProjectOrigin;


    /// <summary>
    /// The player behavior script, which is used for the movement, inputs, hp management and more
    /// </summary>
    PlayerBattle playerManager;

    [SerializeField] Animator yourTurnAnim;
    /// <summary>
    /// gets the PlayerManager and SliderlessEnemyBehavior scripts
    /// </summary>
    private void Awake()
    {
        playerManager = player.GetComponent<PlayerBattle>();
    }
    #endregion

    #region Start and update
    private void Start()
    {
        if (!(SceneManager.GetActiveScene().name == "TutorialBattle"))
        {
            SetCursorState(1);
            yourTurnAnim.SetTrigger("isYourturn");
        }
    }
    /// <summary>
    /// updates the position of the playerProjectOrigin if the player is in attacking phase and counts the enemyRoundLength if its their turn
    /// </summary>
    private void Update()
    {
        if (currenGameState == TurnState.playerTurn)
        {
            playerProjectOrigin.transform.right = (playerManager.GetLookDir() - player.transform.position);
        }

        EnemyTurnTimeLogic();
    }

    #endregion

    #region Turns
    void EnemyTurnTimeLogic()
    {
        if (enemyBehavior == null) return;

        if (currenGameState == TurnState.enemyTurn)
        {
            enemyRoundLength += Time.deltaTime;

            if (enemyRoundLength >= maxEnemyRoundLength)
            {
                enemyRoundLength = 0;
                NextRound();
            }
        }
    }

    /// <summary>
    /// Starts the next round by counting the roundCounter and toggling the Your and EnemyTurn methods
    /// </summary>
    public void NextRound()
    {
        roundCounter++;

        if (currenGameState == TurnState.enemyTurn)
        {
            PlayerTurn();
        }
        else if (currenGameState == TurnState.playerTurn)
        {
            StartCoroutine(PrepareEnemyturn());
        }
        else if (currenGameState == TurnState.keianTurn)
        {
            PlayerTurn();
        }
    }

    /// <summary>
    /// Counts down to prepar the player for the enemy turn
    /// </summary>
    /// <returns></returns>
    IEnumerator PrepareEnemyturn()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(0.8f);
        countdownText.text = null;
        EnemyTurn();
    }
    /// <summary>
    /// The Enemy will stop shooting and keian will stop to randomize his quickTimeEvent 
    /// </summary>
    void PlayerTurn()
    {
        currenGameState = TurnState.playerTurn;
        yourTurnAnim.SetTrigger("isYourturn");
        SetCursorState(1);
        enemyBehavior.ChangeIsShooting(false);
        keianBattleBehavior?.ChangeIsInCombat(false);
    }

    /// <summary>
    /// The Enemy will start shooting and keian will start to randomize his quickTimeEvent 
    /// </summary>
    void EnemyTurn()
    {
        Cursor.SetCursor(normalCursor, new Vector2(16, 16), CursorMode.Auto);
        currenGameState = TurnState.enemyTurn;
        playerBulletShot = 0; //resets the bullets for the next playerturn
        if (enemyBehavior != null)
        {
            enemyBehavior.ChangeIsShooting(true);
            keianBattleBehavior?.ChangeIsInCombat(true);
        }
    }

    /// <summary>
    /// When the conditions are met and the player presses left click at the PlayerManager this will instance a strike. It also calls the Enemy´Turn at the end
    /// </summary>
    public void PlayerAttack()
    {
        if (currenGameState == TurnState.playerTurn && maxPlayerBulletAmount > playerBulletShot)
        {
            playerBulletShot++;

            GameObject newBullet = Instantiate(playerSlashBullet, playerProjectOrigin.position, Quaternion.identity);
            newBullet.transform.position = playerProjectOrigin.position;

            enemyBehavior?.SetAgentPosition();
            if (currenGameState == TurnState.playerTurn && maxPlayerBulletAmount == playerBulletShot)
            {
                SetCursorState(0);
                NextRound();
                return;
            }
        }
    }

    /// <summary>
    /// This will execute when the playe reacted and keian is ready to attack
    /// </summary>
    public void StopTurnForKeian()
    {
        if (keianBattleBehavior == null) return;

        currenGameState = TurnState.keianTurn;
        enemyRoundLength = 0;
        enemyBehavior.ChangeIsShooting(false);
        keianBattleBehavior.ChangeIsInCombat(true);
        keianBattleBehavior.Attacks();
        Invoke(nameof(NextRound), 1f);
    }
    #endregion


    public void SetTurnState(TurnState newTurnState)
    {
        currenGameState = newTurnState;

        if (currenGameState == TurnState.playerTurn)
        {
            yourTurnAnim.SetTrigger("isYourturn");
        }
    }

    /// <summary>
    /// Sets the cursor state with the help of an id (0 = normal, 1 = attack)
    /// </summary>
    /// <param name="id"></param>
    public void SetCursorState(int id = 0)
    {
        if (normalCursor == null) return;
        if (turnCursor == null) return;
        if (id == 0)
        {
            Cursor.SetCursor(normalCursor, new Vector2(16, 16), CursorMode.Auto);
            cursorLightBehavior.CursorLightBlue();
        }
        else
        {
            Cursor.SetCursor(turnCursor, new Vector2(16, 16), CursorMode.Auto);
            cursorLightBehavior.CursorLightPurple();
        }
    }
    public TurnState GetTurnState()
    {
        return currenGameState;
    }
    public bool GetPlayerCanShoot()
    {
        if (playerBulletShot < maxPlayerBulletAmount)
        {
            return true;
        }
        return false;
    }
}
