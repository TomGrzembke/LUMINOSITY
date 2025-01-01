using UnityEngine;

public class SoundController : MonoBehaviour
{

    #region Access
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip[] sfxAudioClips;
    [SerializeField] int typeIntervalMax = 6;
    [SerializeField] int typeIntervalMin = 4;
    int currentTypeInterval = 5;
    int currentTypeCounter;
    #endregion

    private void Start()
    {
        sfxAudioSource.playOnAwake = false;
    }

    #region Sounds
    public void ButtonClickSound()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[0]);
    }
    public void BackClickSound()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[2]);
    }

    public void HoverButtonSfx()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[1]);
    }
    public void PlayPickupSfx()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[3]);
    }
    public void PlayDisposeSfx()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[4]);
    }
    public void PlaySworslash()
    {
        ResetPitch();
        sfxAudioSource.PlayOneShot(sfxAudioClips[5]);
    }
    public void PlayType()
    {
        currentTypeCounter++;
        if (currentTypeCounter < currentTypeInterval) return;

        currentTypeInterval = Random.Range(typeIntervalMin, typeIntervalMax);
        sfxAudioSource.pitch = Random.Range(.8f, 1.4f);
        sfxAudioSource.PlayOneShot(sfxAudioClips[6]);
        currentTypeCounter = 0;
    }
    #endregion

    public void ResetPitch()
    {
        sfxAudioSource.pitch = 1;
    }
}

