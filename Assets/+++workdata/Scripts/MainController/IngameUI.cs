
using UnityEngine;
public class IngameUI : MonoBehaviour
{
    #region Access
    [SerializeField] GameObject quest, inventory, ingameSettings;
    [SerializeField] AudioReverbFilter reverbFilter;
    [SerializeField] AudioLowPassFilter lowPassFilter;
    [SerializeField] GameObject playerAudioSource;
    [SerializeField] PlayerController playerController;
    [SerializeField] Animator fadeAnim;

    #endregion

    private void Start()
    {
        fadeAnim.SetTrigger("fadeIn");
        inventory.SetActive(false);
        quest.SetActive(false);
        ingameSettings.SetActive(false);
    }

    #region To
    public void ToIngame()
    {
        inventory.SetActive(false);
        quest.SetActive(false);
        ingameSettings.SetActive(false);
        inventory.SetActive(false);
        SetTimeScale(1);
    }
    public void ToQuest()
    {
        quest.SetActive(true);
        inventory.SetActive(false);
        ingameSettings.SetActive(false);
        SetTimeScale(0);
    }
    public void ToInventory()
    {
        inventory.SetActive(true);
        quest.SetActive(false);
        ingameSettings.SetActive(false);
        SetTimeScale(0);
    }
    public void ToIngameSettings()
    {
        ingameSettings.SetActive(true);
        quest.SetActive(false);
        inventory.SetActive(false);
        SetTimeScale(0);
    }
    #endregion

    public void FromTo(GameObject From, GameObject To)
    {
        From.SetActive(false);
        To.SetActive(true);
    }
    /// <summary>
    /// This wil set the timeScale to the given context
    /// </summary>
    /// <param name="scale">scale will be the new time scale</param>
    void SetTimeScale(float scale)
    {
        Time.timeScale = scale;

        if (scale == 0)
        {
            reverbFilter.enabled = true;
            lowPassFilter.enabled = true;
            playerAudioSource.transform.localPosition = new Vector3(0, 100, 0);
        }
        else if (scale == 1)
        {
            reverbFilter.enabled = false;
            lowPassFilter.enabled = false;
            playerAudioSource.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

}
