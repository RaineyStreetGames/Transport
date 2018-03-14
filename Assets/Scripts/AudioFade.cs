using UnityEngine;
using System.Collections;
 
public static class AudioFade {
 
    public static IEnumerator VolumeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;
    }

    public static IEnumerator In (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
        audioSource.volume = 0.0f;
        
        while (audioSource.volume < startVolume) {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.volume = startVolume;
    }

    public static IEnumerator PitchOut (AudioSource audioSource, float FadeTime) {
        float startPitch = audioSource.pitch;
 
        while (audioSource.pitch > 0) {
            audioSource.pitch -= startPitch * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.pitch = startPitch;
    }
}
 