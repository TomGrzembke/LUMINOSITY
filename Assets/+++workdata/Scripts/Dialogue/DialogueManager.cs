using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager fo the dialogue. I functions as an input for text and sprites to display 
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region Vars
    [Space]
    [Header("Variables")]
    /// <summary>
    /// The amount of speed in which the letters will be displayed. If this is smaller it gets faster
    /// </summary>
    [SerializeField] float typingSpeed;
    public bool dialogueActive { get; private set; }
    /// <summary>
    /// Saves the given typing speed at the start of the game
    /// </summary>
    float typingSpeedSave;
    /// <summary>
    /// Gets subtracted to the typing speed when the player presses left click while conversation (less typing speed means more textspeed)
    /// </summary>
    [SerializeField] float bonusTypingSpeed;
    /// <summary>
    /// Keeps track of wether the game is typing
    /// </summary>
    bool isTyping;
    /// <summary>
    /// The Image of the current speaker
    /// </summary>
    [SerializeField] Image mainCharPicture;

    [SerializeField] Image nameCharBGImage;

    /// <summary>
    /// The current number of sentence that will be displayed
    /// </summary>
    [SerializeField] int conversationCurrentNumber;
    /// <summary>
    /// The current dialogue obj passed by a dialogue trigger
    /// </summary>
    [SerializeField] DialogueSO currentDialogueSO;
    #endregion

    #region Access
    [Space]
    [Header("Access")]
    /// <summary>
    /// Access to the playerController
    /// </summary>
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerMovement playerMovement;
    /// <summary>
    /// The GameObject of the conversationWindow
    /// </summary>
    [SerializeField] GameObject conversationWindow;
    /// <summary>
    /// The TextmeshproUGUI for the main textfield
    /// </summary>
    [SerializeField] TextMeshProUGUI textDisplay;
    /// <summary>
    ///  The TextmeshproUGUI for name of the speaker
    /// </summary>
    [SerializeField] TextMeshProUGUI nameDisplay;
    /// <summary>
    /// The gameObject of the next button
    /// </summary>
    [SerializeField] GameObject nextButton;
    [SerializeField] StoryManager storyManager;
    [SerializeField] SoundController soundController;


    /// <summary>
    /// Access to all of the Chars Scriptable Objects
    /// </summary>
    [SerializeField] CharSO[] charsSOs;
    /// <summary>
    /// Storage for the images of the charakters
    /// </summary>
    [SerializeField] Sprite[] charNameBGImages;
    [SerializeField] List<int> conversationIDWasPlayed = new();

    const string textAlpha = "<color=#00000000>";
    #endregion

    /// <summary>
    /// Sets the text of the textbox to null
    /// </summary>
    private void Start()
    {
        typingSpeedSave = typingSpeed;
        textDisplay.text = null;
    }
    /// <summary>
    /// Starts the next conversation with the current context
    /// </summary>
    public void StartConversation(DialogueSO newDialogueSO)
    {
        for (int i = 0; i < conversationIDWasPlayed.Count - 1; i++)
        {
            if (newDialogueSO.ID == conversationIDWasPlayed[i])
                return;
        }

        dialogueActive = true;
        conversationCurrentNumber = 0;
        currentDialogueSO = newDialogueSO;
        conversationWindow.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(DisplayLineAlpha());
        conversationCurrentNumber++;
        playerController?.SetUIState(PlayerController.UIState.Dialogue);
        playerMovement?.SetControlState(PlayerMovement.ControlState.gameControl);
    }

    /// <summary>
    /// The coroutine for the typing of the single characters of the words
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayLineAlpha()
    {
        var sentences = currentDialogueSO.GetSentences();

        NextButtonToggle(false);
        UpdateCharacterProperties();

        string originalText = sentences[conversationCurrentNumber];

        for (int i = 0; i < originalText.Length + 1; i++)
        {
            textDisplay.text = originalText.Insert(i, textAlpha);
            soundController.PlayType();
            yield return new WaitForSeconds(typingSpeed);
        }

        NextButtonToggle(true);
    }

    /// <summary>
    /// Initiates the next sentence
    /// </summary>
    public void NextSentence()
    {
        if (currentDialogueSO == null) return;
        if (isTyping) return;

        typingSpeed = typingSpeedSave;

        string[] sentences = currentDialogueSO.GetSentences();

        if (conversationCurrentNumber >= sentences.Length)
        {
            EndDialogue();
            return;
        }

        StartCoroutine(DisplayLineAlpha());
        conversationCurrentNumber++;

    }

    /// <summary>
    /// What happens when the dialogue ends
    /// </summary>
    void EndDialogue()
    {
        dialogueActive = false;
        conversationWindow.SetActive(false);
        conversationIDWasPlayed.Add(currentDialogueSO.ID);
        playerController?.SetUIState(PlayerController.UIState.Ui);
        playerMovement?.SetControlState(PlayerMovement.ControlState.playerControl);
        playerController?.SetDialogueSO(null);
        typingSpeed = typingSpeedSave;
        if (storyManager != null)
        {
            storyManager.AfterDialogue(currentDialogueSO);
        }
    }

    /// <summary>
    /// Updates the displayed Char UI and name
    /// </summary>
    void UpdateCharacterProperties()
    {
        int[] currentCharNameIDArray = currentDialogueSO.GetSentencesNameID();
        int charID = currentCharNameIDArray[conversationCurrentNumber];

        nameDisplay.text = charsSOs[charID].charName;
        nameCharBGImage.sprite = charNameBGImages[charID];
        mainCharPicture.sprite = charsSOs[charID].charSprite;
    }
    /// <summary>
    /// Toggle the button and isTypingBool
    /// </summary>
    void NextButtonToggle(bool value)
    {
        if (!value)
        {
            isTyping = true;
            nextButton.SetActive(false);
        }
        else
        {
            isTyping = false;
            nextButton.SetActive(true);
        }
    }

    /// <summary>
    /// Subtracts the bonusTypingSpeed when the player presses interact when the game is typing the single letters to speed up the text
    /// </summary>
    public void TextSpeedUp()
    {
        typingSpeed -= bonusTypingSpeed;
    }

    /// <summary>
    /// Gets the current isTyping state
    /// </summary>
    /// <returns>The state of is typing</returns>
    public bool GetIsTyping()
    {
        return isTyping;
    }


}
