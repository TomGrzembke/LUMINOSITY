
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    #region Variables
    [Space]
    [Header("Variables")]
    /// <summary>
    /// The amount of avalaible healthpots
    /// </summary>
    [SerializeField] int healthPotAmount;
    /// <summary>
    /// Keeps track if the player is slowed
    /// </summary>
    [SerializeField] bool isSlowed;
    /// <summary>
    /// Determines the percantage of slows through bushes
    /// </summary>
    [SerializeField] float slowPercentage;
    /// <summary>
    /// The Current state of the UI which determins wether it should be accesable or not
    /// </summary>
    [SerializeField] UIState uiState;

    public enum UIState
    {
        Ui,
        Uiless,
        Dialogue
    }



    #endregion

    #region Access
    [Header("Optional")]
    /// <summary>
    /// Acces to keians script, will be ignored if null
    /// </summary>
    [SerializeField] KeianMainSceneBehavior keianBehavior;
    /// Used for activating the cursor light on click, will be ignored if null
    /// </summary>
    [SerializeField] CursorLightBehavior cursorLightBehavior;
    /// <summary>
    /// TheAudioSource attached to the player
    /// </summary>
    [SerializeField] Transform playerAudioSourceTrans;

    [Space]
    [Header("Access")]
    PlayerMovement playerMovement;
    /// <summary>
    /// The Manager scrit for the inventory
    /// </summary>
    [SerializeField] SlotManager slotManager;
    /// <summary>
    /// Access to the dialogueManager script
    /// </summary>
    [SerializeField] DialogueManager dialogueManager;
    /// <summary>
    /// The GameObject for the pauseMenu UI
    /// </summary>
    [SerializeField] GameObject pauseMenu;
    /// <summary>
    /// The GameObject for the invetory UI
    /// </summary>
    [SerializeField] GameObject inventory;
    /// <summary>
    /// The GameObject that contains the quest UI
    /// </summary>
    [SerializeField] GameObject questUI;
    /// <summary>
    /// An array which stores all of the gameObjects that should be turned off when pressing esc
    /// </summary>
    /// <summary>
    /// The Animator for your quest overview which is clickable to go to the Quests tab, change isActive to enable/disable
    /// </summary>
    [SerializeField] Animator ingameUIAnim;
    [SerializeField] AudioReverbFilter reverbFilter;
    [SerializeField] AudioLowPassFilter lowPassFilter;
    [SerializeField] GameObject[] objToCloseForPause;
    /// <summary>
    /// Stores all of the gameObjects that should be turned off when pressing q
    /// </summary>
    [SerializeField] GameObject[] objToCloseForQuest;
    /// <summary>
    /// The audiosources for music
    /// </summary>
    [SerializeField] AudioSource[] audioSource;

    /// <summary>
    /// A storage slot for the input actions
    /// </summary>
    PlayerControls inputActions;
    /// <summary>
    /// The navmeshagent component of this gameObject
    /// </summary>
    NavMeshAgent agent;

    [Space]
    [Header("RuntimeAccess")]
    /// <summary>
    /// The current fountainManager will be ignored if null
    /// </summary>
    [SerializeField] FountainManager fountainManager;
    /// <summary>
    /// The current Scriptable object to display, will be ignored if null
    /// </summary>
    [SerializeField] DialogueSO dialogueSO;
    /// <summary>
    /// The current Event object to display, will be ignored if null
    /// </summary>
    [SerializeField] EventHandler currentEventhandler;
    /// <summary>
    /// IsPassed for returning a signal to it for saving that it was triggert
    /// </summary>
    InteractDetection currentInteractDetaction;

    #endregion

    #region Start/Awake/Update/FixedUpdate
    /// <summary>
    /// Connects the input actions with the corresponding methods and get the components of this object
    /// </summary>
    private void Awake()
    {
        inputActions = new PlayerControls();

        inputActions.PlayerControl.Dialogue.performed += ctx => DialogueInput();
        inputActions.PlayerControl.Interact.performed += ctx => InteractInput();
        inputActions.PlayerControl.Questlog.performed += ctx => QuestToggle();
        inputActions.PlayerControl.Inventory.performed += ctx => InventoryToggle();
        inputActions.PlayerControl.Escape.performed += ctx => PauseMenuToggle();
        inputActions.PlayerControl.Attack.performed += ctx => AttackInput();

        playerMovement = GetComponent<PlayerMovement>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }
    #endregion

    #region InputMethods

    void AttackInput()
    {
        cursorLightBehavior?.CursorClicked();
    }

    #region Interact
    /// <summary>
    /// This gets executed when the player presses e
    /// </summary>
    void DialogueInput()
    {
        if (uiState == UIState.Ui) return;

        if (!dialogueManager.GetIsTyping())
        {
            dialogueManager.NextSentence();
        }
        else if (dialogueManager.GetIsTyping())
        {
            dialogueManager.TextSpeedUp();
        }
    }

    void InteractInput()
    {
        DialogueStartLogic();
        FountainLogic();
    }
    /// <summary>
    /// Handels the Dialogue Logic for the current condition
    /// </summary>
    void DialogueStartLogic()
    {
        if (currentEventhandler != null)
            currentEventhandler.OnEvent();

        if (dialogueSO != null && !dialogueManager.dialogueActive)
        {
            uiState = UIState.Dialogue;
            dialogueManager.StartConversation(dialogueSO);
            currentInteractDetaction?.AddToDeactivate();
            playerMovement.ResetWASDPoint();
        }
    }

    public void FountainLogic()
    {
        if (fountainManager == null) return;


        if (slotManager.GetItemSos() != null)
            fountainManager.AddNewItem(slotManager.GetFirstItem());
    }
    #endregion

    #region Escape
    /// <summary>
    /// This gets executed when the player presses esc
    /// </summary>
    public void PauseMenuToggle()
    {
        if (pauseMenu.activeSelf && !(uiState == UIState.Uiless))
        {
            pauseMenu.SetActive(false);
            SetTimeScale(1);
            ingameUIAnim.SetBool("isActive", true);
            return;
        }

        if (uiState == UIState.Uiless)
            return; //returns if there shouldnt be UI displayed currently

        else if (!pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            SetTimeScale(0);

            for (int i = 0; i < objToCloseForPause.Length - 1; i++)
            {
                objToCloseForPause[i].SetActive(false);
            }
        }

    }
    #endregion

    #region Inventory/QuestToggle
    /// <summary>
    /// This gets executed when the player presses i
    /// </summary>
    public void InventoryToggle()
    {
        if (inventory.activeSelf && !(uiState == UIState.Uiless))
        {
            inventory.SetActive(false);
            ingameUIAnim.SetBool("isActive", true);
            SetTimeScale(1);
            return;
        }

        if (uiState == UIState.Uiless)
            return; //returns if there shouldnt be UI displayed currently

        if (!inventory.activeSelf)
        {
            inventory.SetActive(true);
            SetTimeScale(0);
            for (int i = 0; i < objToCloseForPause.Length; i++)
            {
                if (i == 0 || i == 2)
                    objToCloseForPause[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Toggles the UI for the quests
    /// </summary>
    public void QuestToggle()
    {
        if (questUI.activeSelf && !(uiState == UIState.Uiless))
        {
            questUI.SetActive(false);
            ingameUIAnim.SetBool("isActive", true);
            SetTimeScale(1);
            return;
        }

        if (uiState == UIState.Uiless)
            return; //returns if there shouldnt be UI displayed currently

        if (!questUI.activeSelf)
        {
            questUI.SetActive(true);
            SetTimeScale(0);
            for (int i = 0; i < objToCloseForQuest.Length; i++)
            {
                objToCloseForQuest[i].SetActive(false);
            }
        }
    }
    #endregion

    #endregion

    #region SpezialMethods
    /// <summary>
    /// Sets the Speed for the companion
    /// </summary>
    /// <param name="value"></param>
    void SetKeianAgentSpeed(float value)
    {
        if (keianBehavior == null) return;
        keianBehavior.SetSpeed(value);
    }

    /// <summary>
    /// This wil set the timeScale to the given context
    /// </summary>
    /// <param name="scale">scale will be the new time scale</param>
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;

        if (scale == 0)
        {
            reverbFilter.enabled = true;
            lowPassFilter.enabled = true;

            if (playerAudioSourceTrans == null) return;
            playerAudioSourceTrans.localPosition = new Vector3(0, 100, 0);
        }
        else if (scale == 1)
        {
            reverbFilter.enabled = false;
            lowPassFilter.enabled = false;

            if (playerAudioSourceTrans == null) return;
            playerAudioSourceTrans.localPosition = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region Setter
    public void SetUIState(UIState newUIState)
    {
        uiState = newUIState;
        if (uiState == UIState.Uiless)
            ingameUIAnim.SetBool("isActive", false);
        else if (uiState == UIState.Ui)
            ingameUIAnim.SetBool("isActive", true);
        else if (uiState == UIState.Dialogue)
            ingameUIAnim.SetBool("isActive", false);
    }

    /// <summary>
    /// Give a string of either "uiless" or "ui"
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeUIState(string newState)
    {
        if (newState == "ui")
            uiState = UIState.Ui;
        else if (newState == "uiless")
        {
            uiState = UIState.Uiless;
            ingameUIAnim.SetBool("isActive", false);
        }
    }


    public void SetFountainManager(FountainManager newFountainManager)
    {
        fountainManager = newFountainManager;
    }

    public void SetDialogueSO(DialogueSO newDialogueSo)
    {
        dialogueSO = newDialogueSo;
    }

    public void SetCurrentInteractDetection(InteractDetection newInteractDetection)
    {
        currentInteractDetaction = newInteractDetection;
    }

    public void SetCurrentEventhandler(EventHandler newEventHandler)
    {
        currentEventhandler = newEventHandler;
    }
    IEnumerator WarpPlayer(Vector3 newVector3)
    {
        agent.Warp(newVector3);
        yield return new WaitForSeconds(0.1f);
    }
    #endregion

    #region Getter 


    public int GetHealthPotAmount()
    {
        return healthPotAmount;
    }


    public DialogueSO GetCurrentDialogueSO()
    {
        if (dialogueSO == null)
        {
            Debug.Log("No DialogueSO avaliable");
            return null;
        }
        else
        {
            return dialogueSO;
        }
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
        transform.position = data.playerPos;
        this.healthPotAmount = data.potAmount;
        StartCoroutine(WarpPlayer(data.playerPos));
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.playerPos = this.transform.position;
        data.potAmount = this.healthPotAmount;

    }
    #endregion

}
