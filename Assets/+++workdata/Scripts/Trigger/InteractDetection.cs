
using UnityEngine;

public class InteractDetection : MonoBehaviour
{
    #region Stats
    [SerializeField] DialogueSO dialogueSO;
    [SerializeField] GameObject eUI;
    PlayerController playerController;
    bool wasUsed;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasUsed) return;
        if (collision.CompareTag("Player"))
        {
            if (dialogueSO == null) return;
            playerController = collision.GetComponent<PlayerController>();
            playerController.SetCurrentInteractDetection(this);
            playerController.SetDialogueSO(dialogueSO);
            if (eUI != null)
                eUI.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (dialogueSO == null) return;
            playerController.SetDialogueSO(null);
            playerController.SetCurrentInteractDetection(null);
            if (eUI != null)
                eUI.SetActive(false);
        }
    }

    /// <summary>
    /// Turns of the object when it was used by the player and passes this GO to the save disabled script
    /// </summary>
    public void AddToDeactivate()
    {
        wasUsed = true;
        GetComponent<Collider2D>().enabled = false; 
        if (eUI != null)
            eUI.SetActive(false);
    }
}
