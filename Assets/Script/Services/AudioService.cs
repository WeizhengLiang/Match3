using UnityEngine;

public class AudioService : IAudioService
{
    private AudioSource audioSource;

    public void SetAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    public void PlayClick(AudioClip click) => audioSource.PlayOneShot(click);
    public void PlayDeselect(AudioClip deselect) => audioSource.PlayOneShot(deselect);
    public void PlayMatch(AudioClip match) => audioSource.PlayOneShot(match);
    public void PlayNoMatch(AudioClip noMatch) => audioSource.PlayOneShot(noMatch);
    public void PlayWoosh(AudioClip woosh) => PlayRandomPitch(woosh);
    public void PlayPop(AudioClip pop) => PlayRandomPitch(pop);

    public void PlayRandomPitch(AudioClip audioClip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(audioClip);
        audioSource.pitch = 1f;
    }
}