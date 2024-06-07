using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip deselect;
    [SerializeField] private AudioClip match;
    [SerializeField] private AudioClip noMatch;
    [SerializeField] private AudioClip woosh;
    [SerializeField] private AudioClip pop;

    private IAudioService audioService;
    private AudioSource audioSource;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }
    
    void OnValidate()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioService.SetAudioSource(audioSource);
    }

    public void PlayClick() => audioService.PlayClick(click);
    public void PlayDeselect() => audioService.PlayDeselect(deselect);
    public void PlayMatch() => audioService.PlayMatch(match);
    public void PlayNoMatch() => audioService.PlayNoMatch(noMatch);
    public void PlayWoosh() => audioService.PlayWoosh(woosh);
    public void PlayPop() => audioService.PlayPop(pop);
}