
using System.Collections;
using UnityEngine;


/// <summary>
/// Navigates the player to the right at the end of the frame
/// </summary>
public class MovePlayerTotheRight : MonoBehaviour
{
    #region Variables
    bool wasActivated;
    #endregion

    #region Access
    PlayerMovement playerMovement;
    [SerializeField] GameObject wASDUI;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueSO startDialogueSo;
    [SerializeField] GameObject blockObj;
    [SerializeField] GameObject questUI;
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(wasActivated)
        {
            return;
        }

        if(collision.CompareTag("Player"))
        {
            wasActivated = true;
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.MoveToRight();
            StartCoroutine(FirstConversation());
            questUI.SetActive(false);
        }
    }

    void StartConversation(DialogueSO newDialogueSO)
    {
        dialogueManager.StartConversation(newDialogueSO);
    }

    IEnumerator FirstConversation()
    {
        yield return new WaitForSeconds(1f);
        StartConversation(startDialogueSo);
        bool inDialogue = dialogueManager.dialogueActive;
        while (inDialogue)
        {
            inDialogue = dialogueManager.dialogueActive;
            yield return null;
        }
        wASDUI.SetActive(true);
        blockObj.SetActive(true);

    }
}

