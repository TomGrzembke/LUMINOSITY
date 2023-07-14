using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour, IDataPersistence
{
    #region Access
    [Space]
    [Header("Access")]
    [SerializeField] GameObject dialogueOverlay;
    [SerializeField] GameObject endScreenGO;
    [SerializeField] PlayerController playercontroller;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CinemachineVirtualCamera cinemachinemShopCam;
    [SerializeField] CinemachineVirtualCamera mainCamCinemachine;
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Transform varaTrans;
    [SerializeField] Transform explosionCutsceneTrans;
    [SerializeField] Transform trainingGroundTrans;
    [SerializeField] Transform exitCutsceneCamPoint;
    [SerializeField] Transform playerCamFollow;
    [SerializeField] Transform trainingGroundPoint;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] MusicController timingmanager;
    [SerializeField] Collider2D caveBlockObj;
    [SerializeField] Collider2D endScreenCollider;
    [SerializeField] Collider2D secondCombatCollider;
    [SerializeField] Animator caveDoorAnim;
    [SerializeField] Animator fadeAnim;
    [SerializeField] AudioSource dwayneBonkAudioSource;

    [Space]
    [Header("CustomScripts")]
    [SerializeField] KeianMainSceneBehavior keianMainSceneBehavior;
    [SerializeField] DwayneBehavior dwayneBehavior;
    [SerializeField] SlotManager slotManager;
    [SerializeField] ItemBehavior rewardItemBehavior;
    [SerializeField] QuestsManager questManager;
    [SerializeField] MusicController musicController;
    [SerializeField] SoundController soundController;
    [Space]
    [Header("Arrays")]
    [SerializeField] DialogueSO[] dialogueSos;
    [SerializeField] QuestSO[] questSOs;
    [SerializeField] Transform[] dwayneRollTrans;
    public List<Collider2D> keianTalkPoints { private get; set; } = new();
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    [Space]
    [Header("ObjAnimStateSave")]
    [SerializeField] Animator explosionAnim;
    bool cityBroken;

    [SerializeField] Animator doorAnim;
    bool doorOpen;

    private void Awake()
    {
        cinemachineBasicMultiChannelPerlin = mainCamCinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    #endregion

    #region cutscenes
    /// <summary>
    /// For main scene
    /// </summary>
    public void ExplosionTriggerEvent()
    {
        timingmanager.TriggerTransitionToExplosAudio();
        playercontroller.SetUIState(PlayerController.UIState.Uiless);
        playerMovement.SetControlState(PlayerMovement.ControlState.gameControl);
        playerMovement.MoveToCliff(explosionCutsceneTrans.position);
        cityBroken = true;
    }

    /// <summary>
    /// For main scene
    /// </summary>
    public void ExplosionTriggerArrivedEvent()
    {
        Invoke(nameof(ExplosionConversation), 9f);
        playerMovement.UpdateForCutscene();
    }

    void ExplosionConversation()
    {
        dialogueManager.StartConversation(dialogueSos[1]);
        playercontroller.SetUIState(PlayerController.UIState.Uiless);
    }


    IEnumerator ToTutorialScene()
    {
        fadeAnim.SetBool("isOn", true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator TrainingGroundTP()
    {
        fadeAnim.SetBool("isOn", true);
        yield return new WaitForSeconds(1f);
        var venPos = new Vector3(trainingGroundTrans.position.x, trainingGroundTrans.position.y - 4, trainingGroundTrans.position.z);
        playercontroller.gameObject.GetComponent<NavMeshAgent>().Warp(venPos);
        varaTrans.position = trainingGroundTrans.position;
        playerMovement.UpdateForCutscene();
        fadeAnim.SetBool("isOn", false);
        yield return new WaitForSeconds(1f);
        dialogueManager.StartConversation(dialogueSos[3]);
    }

    IEnumerator TrainingGroundConversation()
    {
        varaTrans.transform.position = trainingGroundPoint.position;
        playerMovement.UpdateForCutscene();
        yield return new WaitForSeconds(.7f);
        dialogueManager.StartConversation(dialogueSos[4]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        keianMainSceneBehavior.enabled = true;
        slotManager.EnableKeianSprite();
        questManager.SetCurrentMainQuestSO(questSOs[2]);
        caveBlockObj.enabled = false;
        yield return new WaitForSeconds(1f);
        dialogueManager.StartConversation(dialogueSos[5]);
    }

    IEnumerator DwayneAfterFirstEncounter()
    {
        fadeAnim.SetBool("isOn", true);
        yield return new WaitForSeconds(.3f);
        dwayneBonkAudioSource.Play();
        yield return new WaitForSeconds(.7f);
        StartCoroutine(dwayneBehavior.DwayneBonked());
        fadeAnim.SetBool("isOn", false);
        yield return new WaitForSeconds(1f);
        dialogueManager.StartConversation(dialogueSos[8]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        dwayneBehavior.StartCoroutine(dwayneBehavior.SetMovePosition(dwayneRollTrans[0].position));
        yield return new WaitForSeconds(2f);
        dialogueManager.StartConversation(dialogueSos[9]);
        questManager.SetCurrentMainQuestSO(questSOs[3]);

    }
    public void StartShopBehavior()
    {
        StartCoroutine(ShopBehavior());
    }
    IEnumerator ShopBehavior()
    {
        PreparCamForHardSwap();
        cinemachinemShopCam.Priority = 11;
        musicController.SetMusicState("tucker");
        yield return new WaitForSeconds(1f);
        dialogueManager.StartConversation(dialogueSos[11]);
        dialogueOverlay.SetActive(false);
        secondCombatCollider.enabled = true;
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        dialogueOverlay.SetActive(true);
        yield return new WaitForSeconds(.1f);
        cinemachinemShopCam.Priority = 10;
        musicController.SetMusicState("cA");
        yield return new WaitForSeconds(1f);
        playerMovement.UpdateForCutscene();
        dialogueManager.StartConversation(dialogueSos[12]);
        questManager.SetCurrentMainQuestSO(questSOs[4]);
    }
    public IEnumerator DwayneAltar()
    {
        yield return new WaitForSeconds(1f);
        doorOpen = true;
        dialogueManager.StartConversation(dialogueSos[14]);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        yield return new WaitForSeconds(.1f);
        DwayneAltarLogic();
        yield return new WaitForSeconds(4f);
        dialogueManager.StartConversation(dialogueSos[15]);
        bool inDialogue2 = dialogueManager.dialogueActive;
        while (inDialogue2)
        {
            inDialogue2 = dialogueManager.dialogueActive;
            yield return null;
        }
        dwayneBehavior.SetSpeed(6);
        questManager.SetCurrentMainQuestSO(questSOs[5]);
        DoorCutscene();
        yield return new WaitForSeconds(3.5f);
        AfterCutscene();
        yield return new WaitForSeconds(1f);
        dialogueManager.StartConversation(dialogueSos[16]);
    }

    public void StartEndScreenAnim()
    {
        StartCoroutine(EndScreenAnim());
    }
    IEnumerator EndScreenAnim()
    {

        fadeAnim.SetBool("isOn", true);
        yield return new WaitForSeconds(1.5f);
        playerMovement.SetControlState(PlayerMovement.ControlState.gameControl);
        yield return new WaitForSeconds(1.5f);
        endScreenGO.SetActive(true);
        yield return new WaitForSeconds(1f);
        fadeAnim.SetBool("isOn", false);
    }

    private void DwayneAltarLogic()
    {
        List<Vector3> altarPoints = new() { dwayneRollTrans[1].position, dwayneRollTrans[2].position, dwayneRollTrans[3].position, dwayneRollTrans[4].position, dwayneRollTrans[5].position, dwayneRollTrans[6].position };
        dwayneBehavior.SetSpeed(13);
        dwayneBehavior.StartCoroutine(dwayneBehavior.SetMovePointList(altarPoints));
    }

    private void AfterCutscene()
    {
        playerMovement.SetControlState(PlayerMovement.ControlState.playerControl);
        mainCamCinemachine.Follow = playerCamFollow;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0;
    }

    private void DoorCutscene()
    {
        caveDoorAnim.SetBool("isOpen", true);
        doorOpen = true;
        mainCamCinemachine.Follow = exitCutsceneCamPoint;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 1;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0.2f;
        playerMovement.SetControlState(PlayerMovement.ControlState.gameControl);
        endScreenCollider.enabled = true;
    }

    private void PreparCamForHardSwap()
    {
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Custom;
        cinemachineBrain.m_DefaultBlend.m_Time = 0f;
    }

    /// <summary>
    /// Determines what happens after the dialogues
    /// </summary>
    /// <param name="lastPlayerDialogueSO"></param>
    public void AfterDialogue(DialogueSO lastPlayerDialogueSO)
    {
        int ID = lastPlayerDialogueSO.ID;
        switch (ID)
        {
            case 0:
                Debug.Log("ID of: " + lastPlayerDialogueSO.name + " wasnt declared");
                break;
            case 3:
                StartCoroutine(TrainingGroundTP());
                break;
            case 4:
                StartCoroutine(ToTutorialScene());
                break;
            case 13:
                StartCoroutine(DwayneAfterFirstEncounter());
                break;
            default:
                break;
        }
    }

    void After2ndCombat()
    {
        dialogueManager.StartConversation(dialogueSos[13]);
        slotManager.AddItem(rewardItemBehavior);
        soundController.PlayPickupSfx();
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        cityBroken = data.cityDestroyed;
        doorOpen = data.exitDoorOpen;
        if (cityBroken)
            explosionAnim.SetBool("isBroken", true);
        if (doorOpen)
            doorAnim.SetBool("isOpen", true);
        if (data.combatsDone[0] && !data.combatsDone[1] && !data.combatsDone[2])
        {
            data.combatsDone[0] = false;
            StartCoroutine(TrainingGroundConversation());
            data.shouldReceiveCombatReward[0] = false;
        }
        else if (data.combatsDone[2] && data.shouldReceiveCombatReward[2])
        {
            data.shouldReceiveCombatReward[2] = false;
            Invoke(nameof(After2ndCombat), 0.5f);
        }
    }


    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.cityDestroyed = cityBroken;
        data.exitDoorOpen = doorOpen;
    }
    #endregion
}

