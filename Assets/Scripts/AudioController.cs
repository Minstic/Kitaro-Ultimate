using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip ClickSound;
    public AudioClip HitSound;
    public AudioClip HBSound;
    public AudioSource SfxController;
    public AudioSource MusicController;
    Dictionary<string, AudioClip> soundCache = new Dictionary<string, AudioClip>();
    public void PlaySound()
    {
        SfxController.PlayOneShot(ClickSound);
    }

    public void PauseMusic()
    {
        if (MusicController.isPlaying)
        {
            MusicController.Pause();
        }
        else
        {
            MusicController.UnPause();
        }
    }

    public void PlayHit()
    {
        SfxController.clip = HitSound;
        SfxController.Play();
    }

    public void PlayHB()
    {
        SfxController.clip = HBSound;
        SfxController.Play();
    }



    public void PlayAbilitySound(string ability)
    {
        if (!soundCache.TryGetValue(ability, out AudioClip clip))
        {
            clip = Resources.Load<AudioClip>("Sounds/" + ability);
            if (clip != null) soundCache[ability] = clip;
        }
        SfxController.clip = clip;
        SfxController.Play();
    }
}
