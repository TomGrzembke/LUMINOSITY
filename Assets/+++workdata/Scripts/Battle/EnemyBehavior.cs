using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// The Behavior of an enemy in a fighting scene
/// </summary>
public class EnemyBehavior : MonoBehaviour, IDataPersistence
{
    #region Variables
    /// <summary>
    /// The storage for the current enemy hp
    /// </summary>
    [SerializeField] int hp;
    /// <summary>
    /// The storage for the current enemy speed
    /// </summary>
    [SerializeField] float speed;
    /// <summary>
    /// The ShootCD for the next bullet which will get randomized 
    /// </summary>
    [SerializeField] float shootCD;
    /// <summary>
    /// The maximum amount of Seconds which will be used as the ceiling of the randomize next CD in Update
    /// </summary>
    [SerializeField] float maxShootCD;
    /// <summary>
    /// The counter for the length of the enemy turn
    /// </summary>
    [SerializeField] float shootCounter;
    /// <summary>
    /// The Vector2 of the least coordinates that the enemy could move to
    /// </summary>
    [SerializeField] Transform XAndYLeastEnemy;
    /// <summary>
    /// The Vector2 of the highest coordinates that the enemy could move to
    /// </summary>
    [SerializeField] Transform XAndYMostEnemy;
    /// <summary>
    /// The current destination of the enemy
    /// </summary>
    Vector3 agentDestination;
    /// <summary>
    /// THe amount of cooldown an agent has for moving
    /// </summary>
    [SerializeField] float agentMoveCD;

    /// <summary>
    /// The single storage slots for the minimum and maximum positions of x and y that enemy could move to
    /// </summary>
    float minXBG, maxXBG, minYBG, maxYBG;
    /// <summary>
    /// This bool keeps track of the enemy shooting
    /// </summary>
    bool isShooting;
    /// <summary>
    /// This bool keeps track of the enemy movement
    /// </summary>
    bool isMoving;
    bool isDead;
    #endregion

    #region Access
    [SerializeField] int combatID;
    /// <summary>
    /// The Point where all of the bullets get instanciated
    /// </summary>
    [SerializeField] Transform bulletOrigin;
    /// <summary>
    /// The current bullet which will be used for shooting
    /// </summary>
    [SerializeField] GameObject bulletPrefab;
    /// <summary>
    /// The slider component which is used for the enemy healthbar
    /// </summary>
    [SerializeField] Slider enemyHealthbar;
    /// <summary>
    /// Access to the player GameObject
    /// </summary>
    [SerializeField] GameObject player;
    /// <summary>
    /// The NavmeshAgent for the enemy
    /// </summary>
    NavMeshAgent agent;
    /// <summary>
    /// The script which keeps track of the instancing of the bullets
    /// </summary>
    InstantiateSettings instantiateSettings;
    /// <summary>
    /// Handels scenes and loading
    /// </summary>
    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] DataPersistanceManager persistanceManager;
    [SerializeField] Animator fadeAnim;
    FlashEffect flashEffectScript;
    Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        instantiateSettings = gameObject.GetComponent<InstantiateSettings>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        flashEffectScript = GetComponent<FlashEffect>();
        anim = GetComponent<Animator>();
    }
    #endregion

    #region Start/Update
    /// <summary>
    /// Assigns all of the minimum and maximum mevement points/coordinates to the right floats 
    /// </summary>
    private void Start()
    {
        minXBG = XAndYLeastEnemy.transform.position.x;
        minYBG = XAndYLeastEnemy.transform.position.y;

        maxXBG = XAndYMostEnemy.transform.position.x;
        maxYBG = XAndYMostEnemy.transform.position.y;
        enemyHealthbar.maxValue = hp;
        UpdateHealthbar();
    }
    /// <summary>
    /// Counts down its timer and shoots when the cooldown is zero
    /// </summary>
    private void Update()
    {
        bulletOrigin.transform.right = -(player.transform.position - transform.position); //The bullet origin always "looks at" the player

        if (isShooting)
        {
            shootCounter += Time.deltaTime;
        }

        if (bulletPrefab != null && shootCounter >= shootCD)
        {
            InstantiateBullet();
            shootCounter = 0;
            shootCD = Random.Range(0.1f, maxShootCD);

            SetAgentPosition();
        }
    }
    #endregion


    /// <summary>
    /// Gives the agent the command to move to its destination
    /// </summary>
    /// <returns>Waits the given amount of seconds</returns>
    IEnumerator Moving()
    {
        if (!isMoving)
        {
            isMoving = true;
            agent.SetDestination(agentDestination);
            yield return new WaitForSeconds(agentMoveCD);
            isMoving = false;
        }
    }
    /// <summary>
    /// Executes the InstantiateBullet method on the instantiateSettings script
    /// </summary>
    public void InstantiateBullet()
    {
        instantiateSettings.InstantiateBullet(bulletPrefab);
    }
    /// <summary>
    /// Updates the slider component for the heatlthbar
    /// </summary>
    public void UpdateHealthbar()
    {
        enemyHealthbar.value = hp;
    }

    /// <summary>
    /// Resets the shootCD At the start of the enmys turn
    /// </summary>
    public void ResetForTurnStart()
    {
        shootCD = 0f;
    }

    #region Setter
    /// <summary>
    /// Sets the position where the agent should move to and Starts the Moving method via coroutine
    /// </summary>
    public void SetAgentPosition()
    {
        agentDestination = new Vector3(Random.Range(minXBG, maxXBG), Random.Range(minYBG, maxYBG), transform.position.z);
        StartCoroutine(Moving());
    }

    public void UpdateSpeed()
    {
        agent.speed = speed;
    }
    /// <summary>
    /// Sets the isShotting bool
    /// </summary>
    /// <param name="state">The new state of isSHooting</param>
    public void ChangeIsShooting(bool state)
    {
        isShooting = state;
        ResetForTurnStart();
    }
    /// <summary>
    /// Sets the hp and updates the hpBar
    /// </summary>
    /// <param name="hpAdd">The amount that should be added to hp</param>
    public void ChangeHP(int hpAdd)
    {
        hp += hpAdd;
        UpdateHealthbar();
        if (hp <= 0)
        {
            anim.SetTrigger("isDead");
            agent.ResetPath();
            hp = 0;
            isDead = true;
            persistanceManager?.SaveGame();
            if (fadeAnim != null)
                fadeAnim.SetBool("isOn", true);
            Invoke(nameof(BackToMainScene), 1f);
        }

        if (flashEffectScript == null)
        {
            Debug.Log("There is no FlashEffect script on Enemy", gameObject);
            return;
        }
        flashEffectScript.Flash();
    }

    private void BackToMainScene()
    {
        sceneLoadManager.ChangeSceneTo(1);
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {

    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        if (!isDead) return;
        data.combatsDone[combatID] = true;
    }
    #endregion
}
