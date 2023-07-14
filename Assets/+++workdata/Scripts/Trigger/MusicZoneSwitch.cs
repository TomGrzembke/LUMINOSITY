
using UnityEngine;

/// <summary>
/// Handels the music switch when switching to another Area
/// </summary>
public class MusicZoneSwitch : MonoBehaviour
{
    #region Access
    GameObject player;
    [SerializeField] MusicController musicTiming;
    #endregion

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            Invoke(nameof(AreaCheck), 0.2f);
        }
    }

    /// <summary>
    /// Checks in which area the player currently is and adjusts the music handling enum
    /// </summary>
    void AreaCheck()
    {
        if(player == null)
            return;

        float xPosDifference = player.transform.position.x - gameObject.transform.position.x;

        if (xPosDifference < 0)
        {
            //Play ow
            musicTiming.SetMusicState("oW");
        }
        else if(xPosDifference > 0)
        {
            //play cA
            musicTiming.SetMusicState("cA");
        }
    }

    #region Getters

    #endregion

    #region Setters

    #endregion

}

