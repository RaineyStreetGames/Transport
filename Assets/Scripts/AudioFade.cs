using UnityEngine;
using System.Collections;
 
public static class AudioFade {
 
    public static IEnumerator Out (AudioSource audioSource, float FadeTime) {
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
}
 