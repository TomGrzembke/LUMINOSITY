using UnityEngine;

/// <summary>
/// Resets dwayne if the collider on his child objects falls out of the map
/// </summary>
public class DwayneResetZone : MonoBehaviour
{
    DwayneBehavior dwayneBehavior;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dwayne"))
        {
            dwayneBehavior = collision.transform.parent.GetComponent<DwayneBehavior>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dwayne"))
        {
            dwayneBehavior.ResetDwayne();
        }
    }

}

