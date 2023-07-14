using UnityEngine;

public class SoundController : MonoBehaviour
{

    #region Access
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip[] sfxAudioClips;
    #endregion

    private void Start()
    {
        sfxAudioSource.playOnAwake = false;
    }
    #region Sounds
    public void ButtonClickSound()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[0]);
    }
    public void BackClickSound()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[2]);
    }

    public void HoverButtonSfx()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[1]);
    }
    public void PlayPickupSfx()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[3]);
    } 
    public void PlayDisposeSfx()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[4]);
    } 
    public void PlaySworslash()
    {
        sfxAudioSource.PlayOneShot(sfxAudioClips[5]);
    }
    #endregion

}

