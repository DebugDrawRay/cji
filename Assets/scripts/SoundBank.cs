using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour
{
    public enum Music
    {
        Intro,
        Loop
    }
    public AudioClip[] MusicTracks;

    public enum SoundEffects
    {

    }

    public static SoundBank Instance;

    void Awake()
    {
        Instance = this;
    }

    public AudioClip Request(Music musicClip)
    {
        int index = (int)musicClip;
        return MusicTracks[index];
    }
}
