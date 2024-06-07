using UnityEngine;

public interface IAudioService
{
    void SetAudioSource(AudioSource audioSource);
    void PlayClick(AudioClip click);
    void PlayDeselect(AudioClip deselect);
    void PlayMatch(AudioClip match);
    void PlayNoMatch(AudioClip noMatch);
    void PlayWoosh(AudioClip woosh);
    void PlayPop(AudioClip pop);
    void PlayRandomPitch(AudioClip audioClip);
}