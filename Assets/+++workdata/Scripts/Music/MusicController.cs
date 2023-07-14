using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour, IDataPersistence
{
    #region Access
    [SerializeField] IngameSettings settings;
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioSource explosionAudioSource;
    [SerializeField] AudioMixer mainAudioMixer;
    [SerializeField] Animator explosionAnim;
    [SerializeField] AudioClip explosions;
    [SerializeField] AudioClip oWLoopClip;
    [SerializeField] AudioClip cALoopClip;
    [SerializeField] AudioClip tuckerMusic;
    #endregion

    #region Variables
    /// <summary>
    /// If true it will save and load the information of the music to play from disc
    /// </summary>
    [SerializeField] bool affectedBySaving;
    /// <summary>
    /// How often it turns down the volume befor transitions
    /// </summary>
    [SerializeField] int transitionDampAmount;
    [SerializeField] MusicState musicState;
    #endregion

    /// <summary>
    /// ow = overworld
    /// cA = Cave
    /// </summary>
    public enum MusicState
    {
        oWIntroLoop,
        oWDrop,
        oWAfterDrop,
        cALoop,
        tucker
    }
        
    IEnumerator VolumeTransition()
    {
        float adjustValue = 0;
        for (int i = 0; i < transitionDampAmount; i++)
        {
            adjustValue -= 2;
            yield return new WaitForSeconds(0.1f);
            AdjustVolume(settings.GetCurrentMusicVolume() + adjustValue);
        }

        playerAudioSource.Pause();
        SongClipLogic(musicState);

        for (int i = 0; i < transitionDampAmount; i++)
        {
            adjustValue += 2;
            yield return new WaitForSeconds(0.1f);
            AdjustVolume(settings.GetCurrentMusicVolume() + adjustValue);
        }
    }
    
    void SongClipLogic(MusicState state)
    {

        switch (state)
        {
            case MusicState.oWIntroLoop:
                AdjustPitch(1f);
                playerAudioSource.clip = oWLoopClip;
                playerAudioSource.Play();
                break;
            case MusicState.oWDrop:
                playerAudioSource.Stop();
                AdjustPitch(1f);
                ExplosionLogic();
                break;
            case MusicState.oWAfterDrop:
                AdjustPitch(0.7f);
                playerAudioSource.clip = oWLoopClip;
                playerAudioSource.Play();
                break;
            case MusicState.cALoop:
                AdjustPitch(1f);
                playerAudioSource.clip = cALoopClip;
                playerAudioSource.Play();
                break;
            case MusicState.tucker:
                AdjustPitch(1f);
                playerAudioSource.clip = tuckerMusic;
                playerAudioSource.Play();
                break;
            default:
                Debug.Log("SongClipLogic at Timing switched to default");
                SongClipLogic(MusicState.oWIntroLoop);
                break;
        }


    }
    public void TriggerTransitionToExplosAudio()
    {
        musicState = MusicState.oWDrop;
        StartCoroutine(VolumeTransition());
    }

    void ExplosionLogic()
    {
        if (!(musicState == MusicState.oWDrop))
        {
            return;
        }

        explosionAudioSource.Play();
        musicState = MusicState.oWAfterDrop;
        if (explosionAnim != null)
        {
            explosionAnim.SetTrigger("cutscene");
        }
    }
    public void AdjustVolume(float volume)
    {
        mainAudioMixer.SetFloat("musicVolume", volume);
    }

    /// <summary>
    /// Adjusts the pitch of the start audio source
    /// </summary>
    /// <param name="value"></param>
    public void AdjustPitch(float value = 1)
    {
        playerAudioSource.pitch = value;
    }

    /// <summary>
    /// oW for overworld,cA for Cave, tucker for Tucker
    /// </summary>
    /// <param name="condition">oW for overworld and cA for Cave</param>
    public void SetMusicState(string condition)
    {
        if (condition == "oW")
            musicState = MusicState.oWAfterDrop;

        else if (condition == "cA")
            musicState = MusicState.cALoop;
        else if(condition == "tucker")
        {
            musicState = MusicState.tucker;
        }
        StartCoroutine(VolumeTransition());
    }

    
    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        if (!affectedBySaving) return;
        this.musicState = data.musicState;
        SongClipLogic(data.musicState);
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        if (!affectedBySaving) return;
        data.musicState = this.musicState;

    }
    #endregion

}
