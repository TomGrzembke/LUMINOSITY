using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VaraBehavior : MonoBehaviour, IDataPersistence
{
    #region Variables
    [Space]
    [Header("Variables")]
    /// <summary>
    /// The storage for the current enemy hp
    /// </summary>
    [SerializeField] int hp;
    [SerializeField] bool wasHit;
    /// <summary>
    /// The storage for the current enemy speed
    /// </summary>
    [SerializeField] float speed;

    /// <summary>
    /// The maximum amount of Seconds which will be used as the ceiling of the randomize next CD in Update
    /// </summary>
    [SerializeField] float maxShootCD;
    /// <summary>
    /// The counter for the length of the enemy turn
    /// </summary>
    [SerializeField] float shootCounter;
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
    /// This bool keeps track of the enemy movement
    /// </summary>
    bool isMoving;

    public enum VaraPhases
    {
        one,
        two,
        three,
        four,
        five,
    }
    [SerializeField] VaraPhases varaPhases;
    #endregion

    #region Access
    [Space]
    [Header("Access")]
    [SerializeField] GameObject damageWall;
    /// <summary>
    /// The current bullet which will be used for shooting
    /// </summary>
    [SerializeField] GameObject bulletPrefab;
    /// <summary>
    /// The Vector2 of the least coordinates that the enemy could move to
    /// </summary>
    [SerializeField] Transform XAndYLeastEnemy;
    /// <summary>
    /// The Vector2 of the highest coordinates that the enemy could move to
    /// </summary>
    [SerializeField] Transform XAndYMostEnemy;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] Transform player;
    [SerializeField] Animator leftClickAnim;
    [SerializeField] InstantiateSettings instantiateSettings;
    [SerializeField] NavMeshAgent varaAgent;
    /// <summary>
    /// The slider component which is used for the enemy healthbar
    /// </summary>
    [SerializeField] Slider enemyHealthbar;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] BattleController battleController;
    [SerializeField] PlayerBattle playerManager;
    [SerializeField] DataPersistanceManager dataPersistanceManager;
    [SerializeField] DialogueSO[] dialogueSOs;
    FlashEffect flashEffectScript;
    void Awake()
    {
        varaAgent = GetComponent<NavMeshAgent>();
        varaAgent.updateRotation = false;
        varaAgent.updateUpAxis = false;
        flashEffectScript = GetComponent<FlashEffect>();
    }
    #endregion

    private void Start()
    {
        leftClickAnim.SetBool("isActive", false);
        Invoke(nameof(StartGame), 0.2f);

    }
    /// <summary>
    /// Counts down its timer and shoots when the cooldown is zero
    /// </summary>
    private void FixedUpdate()
    {
        bulletOrigin.right = -(player.position - transform.position); //The bullet origin always "looks at" the player
    }
    private void StartGame()
    {
        NextVaraPhase(VaraPhases.one);
    }

    public void NextVaraPhase(VaraPhases newPhase)
    {
        switch (newPhase)
        {
            case VaraPhases.one:
                StartCoroutine(PhaseOne());
                break;
            case VaraPhases.two:
                StartCoroutine(PhaseTwo());
                break;
            case VaraPhases.three:
                StartCoroutine(PhaseThree());
                break;
            case VaraPhases.four:
                StartCoroutine(PhaseFour());
                break;
            default:
                StartCoroutine(PhaseOne());
                break;
        }
    }

    #region PhasesMethods
    IEnumerator PhaseOne()
    {
        playerManager.SetHealpotAmount(-10);
        dialogueManager.StartConversation(dialogueSOs[0]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            instantiateSettings.NormalPattern(bulletPrefab);
            StartCoroutine(Moving());
            yield return new WaitForSeconds(1.5f);
        }

        varaPhases = VaraPhases.two;
        NextVaraPhase(varaPhases);
    }

    IEnumerator PhaseTwo()
    {
        dialogueManager.StartConversation(dialogueSOs[1]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        battleController.SetTurnState(BattleController.TurnState.playerTurn);
        battleController.SetCursorState(1);
        leftClickAnim.SetBool("isActive", true);

        while (!wasHit)
        {
            yield return null;
        }
        leftClickAnim.SetBool("isActive", false);
        battleController.SetCursorState(0);
        battleController.SetTurnState(BattleController.TurnState.enemyTurn);

        varaPhases = VaraPhases.three;
        NextVaraPhase(varaPhases);
    }

    IEnumerator PhaseThree()
    {
        dialogueManager.StartConversation(dialogueSOs[2]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }

        yield return new WaitForSeconds(.8f);
        instantiateSettings.SetIsAiming(true);
        instantiateSettings.NormalPattern(damageWall);
        yield return new WaitForSeconds(1.3f);
        varaPhases = VaraPhases.four;
        NextVaraPhase(varaPhases);
    }

    IEnumerator PhaseFour()
    {
        dialogueManager.StartConversation(dialogueSOs[3]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        playerManager.SetHealpotAmount(3);
        playerManager.PlayerHeals();

        yield return new WaitForSeconds(1.7f);
        dialogueManager.StartConversation(dialogueSOs[4]);
        varaPhases = VaraPhases.five;


        bool inDialogue2 = dialogueManager.dialogueActive;
        while (inDialogue2)
        {
            inDialogue2 = dialogueManager.dialogueActive;
            yield return null;
        }

        dataPersistanceManager.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }
    #endregion

    /// <summary>
    /// Sets the position where the agent should move to and Starts the Moving method via coroutine
    /// </summary>
    public void SetAgentPosition()
    {
        agentDestination = new Vector3(Random.Range(minXBG, maxXBG), Random.Range(minYBG, maxYBG), transform.position.z);
        StartCoroutine(Moving());
    }
    /// <summary>
    /// Gives the agent the command to move to its destination
    /// </summary>
    /// <returns>Waits the given amount of seconds</returns>
    IEnumerator Moving()
    {
        if (!isMoving)
        {
            isMoving = true;
            varaAgent.SetDestination(agentDestination);
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

    #region Setters
    public void ChangeHP(int addValue)
    {
        wasHit = true;
        hp += addValue;
        UpdateHealthbar();
        if (addValue > 0) return;
        if(flashEffectScript == null)
        {
            Debug.Log("There is no FlashEffect script on Vara", gameObject);
            return;
        }
        flashEffectScript.Flash();
        
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
        if (varaPhases == VaraPhases.five)
        {
            data.combatsDone[0] = true;
        }
    }
    #endregion
}

