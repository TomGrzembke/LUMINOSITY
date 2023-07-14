using System.Collections;
using UnityEngine;

public class KeianPoint : MonoBehaviour
{
    #region Variables

    #endregion

    #region Access
    KeianMainSceneBehavior keianBehavior;
    PlayerController playerController;
    [SerializeField] Transform idlePoint;
    #endregion

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Keian"))
        {
            keianBehavior = collision.GetComponent<KeianMainSceneBehavior>();
            keianBehavior.FocusedMovement(idlePoint.position);
            StartCoroutine(CheckIfKeianShouldFollow());
        }

        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (keianBehavior == null) return;
        if (collision.CompareTag("Player"))
        {
            keianBehavior.FollowMovement();
            playerController = null;
            StopAllCoroutines();
        }
    }

    IEnumerator CheckIfKeianShouldFollow()
    {
        yield return new WaitForSeconds(8);
        if(playerController == null)
        {
            keianBehavior?.FollowMovement();
        }
    }
}

