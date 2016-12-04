using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public AudioSource musicBus;

    public static AudioController Instance;

    private SoundBank bank;

    void Awake()
    {
        Instance = this;
        bank = GetComponent<SoundBank>();
    }

    public void StartMusic()
    {
        musicBus.clip = bank.Request(SoundBank.Music.Intro);
        musicBus.Play();
        musicBus.loop = false;

        StartCoroutine(CheckForEnd(musicBus, bank.Request(SoundBank.Music.Loop)));
    }
    IEnumerator CheckForEnd(AudioSource source, AudioClip next)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        source.clip = next;
        source.loop = true;
        source.Play();
    }
}
