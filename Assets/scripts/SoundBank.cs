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
        StarGood,
        StarBad,
        ConstellationHit,
        ConstellationSent,
        ConstellationComplete,
        ConstellationBroken
    }
    public AudioClip[] SoundEffectClips;

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
    public AudioClip Request(SoundEffects soundEffect)
    {
        int index = (int)soundEffect;
        return SoundEffectClips[index];
    }
}
