
using UnityEngine;

public class BushBehavior : MonoBehaviour
{
    #region Access
    [SerializeField] PlayerMovement playerMovement;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.SetIsSlowed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            playerMovement.SetIsSlowed(false);
        }
    }
}
