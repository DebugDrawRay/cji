using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public AudioSource musicBus;
    public AudioSource[] effectBus;

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

        StartCoroutine(CheckForEnd(musicBus, bank.Request(SoundBank.Music.Loop), true));
    }

    public void PlayAtEnd(AudioSource source, AudioClip next, bool loop)
    {
        StartCoroutine(CheckForEnd(source, next, loop));
    }

    IEnumerator CheckForEnd(AudioSource source, AudioClip next, bool loop)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        source.clip = next;
        source.loop = loop;
        source.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        int index = 0;
        while(effectBus[index].isPlaying)
        {
            index++;
            if(index >= effectBus.Length - 1)
            {
                index = 0;
            }
        }
        AudioSource bus = effectBus[index];

        bus.clip = clip;
        bus.loop = false;
        bus.Play();
    }
}
