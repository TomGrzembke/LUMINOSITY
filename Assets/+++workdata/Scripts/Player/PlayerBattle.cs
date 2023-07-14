using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// The playerscript for the combat scenes
/// </summary>
public class PlayerBattle : MonoBehaviour, IDataPersistence
{
    #region Variables
    /// <summary>
    /// The storage for the current player hp
    /// </summary>
    [SerializeField] float hp;

    /// <summary>
    /// Stores the playerDamage
    /// </summary>
    [SerializeField] int playerDamage;

    /// <summary>
    /// Insert 0.1f for 10 percent
    /// </summary>
    [SerializeField] float healPercentage;

    /// <summary>
    /// Insert 0.1 so that the autoheal procs at 10% health
    /// </summary>
    [SerializeField] float percentageForAutoheal;
    /// <summary>
    /// Stores the maxHP
    /// </summary>
    float maxHP = 10;

    /// <summary>
    /// The amount of available potions
    /// </summary>
    [SerializeField] int potionAmount;

    /// <summary>
    /// Keeps track wether the player has iFrames or not
    /// </summary>
    [SerializeField] bool iFrames;
    /// <summary>
    /// The amount of time of invincability granted when the player is hit;
    /// </summary>
    [SerializeField] float iFrameTime;
    #endregion

    #region Access
    /// <summary>
    /// The dialogueManager which is used in the combat tutorial, will be ignored if null
    /// </summary>
    [SerializeField] DialogueManager dialogueManager;
    /// <summary>
    /// The keianBattleBehavior script, which is used for the behavior of keian, which includes him attacking and his random Signals
    /// </summary>
    [SerializeField] KeianBattleBehavior keianBattleBehavior;

    [SerializeField] SceneLoadManager sceneLoadManager;
    /// <summary>
    /// The battleController script, which is used for the management of the battle and all of the battle phases
    /// </summary>
    [SerializeField] BattleController battleController;

    /// <summary>
    /// The main cam of the scene
    /// </summary>
    [SerializeField] Camera mainCam;

    /// <summary>
    /// Is used for the leftclick movement
    /// </summary>
    [SerializeField] Vector3 leftClickTarget;

    /// <summary>
    /// The screen to world position of the cursor
    /// </summary>
    [SerializeField] Vector3 lookDir;

    /// <summary>
    /// The Slider component for the healthbar
    /// </summary>
    [SerializeField] Slider healthbar;

    /// <summary>
    /// The input actions for the playerMovement
    /// </summary>
    PlayerControls inputActions;

    /// <summary>
    /// The Navmeshagent of the player
    /// </summary>
    NavMeshAgent agent;

    [SerializeField] TextMeshProUGUI potionCounterText;

    [SerializeField] Animator fade;

    Animator anim;

    [SerializeField] Animator healAnim;

    FlashEffect flashEffectScript;

    [SerializeField] SoundController soundController;

    [SerializeField] Animator leftClickUI;
    #endregion

    #region Awake/Start/Update
    /// <summary>
    /// Connects the Inputactions with their methods and prevents the agent from rotating itself 
    /// </summary>
    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputActions = new();


        inputActions.PlayerControl.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        inputActions.PlayerControl.Dialogue.performed += ctx => Dialogue();
        inputActions.PlayerControl.Attack.performed += ctx => Attack();
        inputActions.PlayerControl.React.performed += ctx => ReactToggle();
        inputActions.PlayerControl.React.canceled += ctx => ReactToggle();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        flashEffectScript = GetComponent<FlashEffect>();

    }

    /// <summary>
    /// Adjusts the speed of the navmesh agent to the variable
    /// </summary>
    private void Start()
    {
        fade?.SetTrigger("fadeIn");
        UpdatePotionCounter();
    }

    #endregion

    #region InputactionsMethods

    /// <summary>
    /// Gets executed when the player LeftClicks
    /// </summary>
    void Dialogue()
    {
        if (dialogueManager == null) return;
        if (!dialogueManager.dialogueActive) return;

        if (!dialogueManager.GetIsTyping())
        {
            dialogueManager.NextSentence();
        }
        else if (dialogueManager.GetIsTyping())
        {
            dialogueManager.TextSpeedUp();
        }
    }

    private void Attack()
    {
        if (!(battleController.GetTurnState() == BattleController.TurnState.playerTurn)) return;
        if (!battleController.GetPlayerCanShoot()) return;
        Invoke(nameof(SlashCommand), 0.3f);
        anim.SetTrigger("isAttacking");
        if(soundController == null) return;
        soundController.PlaySworslash();

        if(leftClickUI == null) return;
        leftClickUI.SetBool("isActive", false);
    }

    private void SlashCommand()
    {
        battleController.PlayerAttack();
    }

    /// <summary>
    /// Gets executed when the player reacts
    /// </summary
    void ReactToggle()
    {
        if(keianBattleBehavior == null) return;
        if (keianBattleBehavior.GetCanAttack())
        {
            keianBattleBehavior.CantAttack();
            battleController.StopTurnForKeian();
        }
        else if (!keianBattleBehavior.GetCanAttack())
        {
            keianBattleBehavior.ChangeIsInCombat(false);
        }
    }

    /// <summary>
    /// Gets executed when the player moves his mouse and saves the current mouse position
    /// </summary
    void Look(Vector2 direction)
    {
        lookDir = mainCam.ScreenToWorldPoint(direction);
        lookDir.z = 0;
    }
    #endregion

    #region IFrames
    IEnumerator IFrames()
    {
        iFrames = true;
        yield return new WaitForSeconds(iFrameTime);
        iFrames = false;
    }
    #endregion

    #region Setter
    /// <summary>
    /// Sets the HP of the Player with the given context 
    /// </summary>
    /// <param name="hpToAdd">The received damage which will be subtracted by the playerHP </param>
    public void ApplyDamage(float hpToAdd)
    {
        float hpAfterDamage = hp + hpToAdd;

        if (hpAfterDamage < maxHP * percentageForAutoheal)
        {
            PlayerHeals();
        }

        if (!iFrames)
        {
            StartCoroutine(IFrames());
            hp += hpToAdd;

            if (hp <= 0)
            {
                hp = 0;
                StartCoroutine(PlayerDeath());
            }
            else if (hp > maxHP)
            {
                hp = maxHP;
            }
            UpdateHealthbar();

            if (flashEffectScript == null)
            {
                Debug.Log("There is no FlashEffect script on Player", gameObject);
                return;
            }
            if (hpToAdd < 0)
                flashEffectScript.Flash();
        }
    }
    IEnumerator PlayerDeath()
    {
        fade.SetBool("isOn", true);
        yield return new WaitForSeconds(1.5f);
        sceneLoadManager?.ChangeSceneTo();
    }

    public void PlayerHeals()
    {
        if (potionAmount < 0)
        {
            return;
        }

        if (potionAmount > 0)
        {
            ApplyDamage(maxHP * healPercentage);
            potionAmount--;
            UpdatePotionCounter();
            healAnim.SetTrigger("isHealing");
        }
    }

    /// <summary>
    /// Adds your given value
    /// </summary>
    /// <param name="amount"></param>
    public void SetHealpotAmount(int amount)
    {
        potionAmount += amount;
        if(potionAmount < 0)
        {
            potionAmount = 0;
        }
        UpdatePotionCounter();
    }

    /// <summary>
    /// Updates the healthbar value to the current hp of the player
    /// </summary>
    public void UpdateHealthbar()
    {
        healthbar.value = hp;
    }

    public void UpdatePotionCounter()
    {
        potionCounterText.text = potionAmount.ToString();

    }
    #endregion

    #region Getter
    /// <summary>
    /// Gets the playerDamage
    /// </summary>
    /// <returns></returns>
    public int GetPlayerDamage()
    {
        return playerDamage;
    }

    /// <summary>
    /// Gets the information of the LookDir Vector3
    /// </summary>
    /// <returns>The stat of the LookDir Vector3</returns>
    public Vector3 GetLookDir()
    {
        return lookDir;
    }
    #endregion

    #region OnEnable/Disable
    /// <summary>
    /// Gets called when this script is enabled
    /// </summary>
    public void OnEnable()
    {
        inputActions.Enable();
    }

    /// <summary>
    /// Gets called when this script is disabled
    /// </summary>
    public void OnDisable()
    {
        inputActions.Disable();
    }

    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        this.potionAmount = data.potAmount;
        if(potionAmount < 3)
        {
            potionAmount++;
        }
        UpdatePotionCounter();
        UpdateHealthbar();
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.potAmount = this.potionAmount;
    }
    #endregion
}
