using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    /// <summary> The manual volume of the game </summary>
    [SerializeField]
    private float Volume = 1;
    /// <summary> Is the music muted </summary>
    private bool MusicOn = true;
    /// <summary> Is the sfx muted </summary>
    private bool SFXOn = true;
    [Header("Sources")]
    /// <summary> Music audio used for all </summary>
    [SerializeField]
    private AudioSource MusicSource;
    /// <summary> SFX audio used for all </summary>
    [SerializeField]
    private AudioSource SFXSource;
    [Header("Sound Bundles")]
    /// <summary> Main sound FX bundle </summary>
    [SerializeField]
    private List<AudioClip> SFXBundle;
    /// <summary> Main Music bundle </summary>
    [SerializeField]
    private List<AudioClip> MusicBundle;

    [Header("Tansition")]
    /// <summary> How many seconds the music transition takes </summary>
    [SerializeField]
    private float TransitionTime = 3;
    /// <summary> Is music transitioning </summary>
    private bool MusicTransition = false;
    /// <summary> is transitioning out </summary>
    private bool TransitionOut = false;
    /// <summary> is transitioning in </summary>
    private bool TransitionIn = false;
    /// <summary> The next song to be faded to </summary>
    private AudioClip NextSong;

    /// <summary> Play a sound in SFX_BNDL with name of given string </summary>
    public void PlaySound(string name)
    {
        if (SFXOn)
        {
            SFXSource.PlayOneShot(SFXBundle.Find(o => o.name == name));
        }
    }

    /// <summary> Play a Random sound in SFX_BNDL with name containing given string</summary>
    public void PlayRandomSound(string name)
    {
        if (SFXOn)
        {
            List<AudioClip> sounds = SFXBundle.FindAll(o => o.name.Contains(name));
            SFXSource.PlayOneShot(sounds[Random.Range(0, sounds.Count)]);
        }
    }

    /// <summary> Play sound with name of given string in given bundle </summary>
    public void PlaySound(string name, List<AudioClip> bndl)
    {
        if (SFXOn)
        {
            SFXSource.PlayOneShot(bndl.Find(o => o.name == name));
        }
    }

    /// <summary> Play random sound containing name of given string in given bundle </summary>
    public void PlayRandomSound(string name, List<AudioClip> bndl)
    {
        if (SFXOn)
        {
            List<AudioClip> sounds = bndl.FindAll(o => o.name.Contains(name));
            SFXSource.PlayOneShot(sounds[Random.Range(0, sounds.Count)]);
        }
    }

    /// <summary> Plays song in music bundle with given name </summary>
    public void PlayMusic(string name)
    {
        if (MusicSource.clip != null)
        {
            MusicTransition = true;
            TransitionIn = true;
            TransitionOut = true;
            NextSong = MusicBundle.Find(o => o.name == name);
        }
        else
        {
            MusicSource.clip = MusicBundle.Find(o => o.name == name);
            MusicSource.Play();
        }
    }

    /// <summary> Plays song in given bundle with given name </summary>
    public void PlayMusic(string name, List<AudioClip> bndl)
    {
        if (MusicSource.clip != null)
        {
            MusicTransition = true;
            TransitionIn = true;
            TransitionOut = true;
            NextSong = bndl.Find(o => o.name == name);
        }
        else
        {
            MusicSource.clip = bndl.Find(o => o.name == name);
            MusicSource.Play();
        }
    }

    private void Update()
    {
        if(MusicTransition)
        {
            if(TransitionOut)
            {
                MusicSource.volume = Mathf.Lerp(MusicSource.volume, 0, Time.deltaTime * (TransitionTime * 0.5f));
                if(MusicSource.volume < 0.05f)
                {
                    MusicSource.volume = 0;
                    TransitionOut = false;
                    MusicSource.clip = NextSong;
                    MusicSource.Play();
                }
            }
            else if(TransitionIn)
            {
                MusicSource.volume = Mathf.Lerp(MusicSource.volume, Volume, Time.deltaTime * (TransitionTime * 0.5f));
                if ((Volume - MusicSource.volume) < 0.05f)
                {
                    MusicSource.volume = Volume;
                    TransitionIn = false;
                    MusicTransition = false;
                }
            }
        }
    }

    public void Mute_SFX(bool mute)
    {
        SFXOn = mute;
    }

    public void Mute_Music(bool mute)
    {
        MusicOn = mute;
        MusicSource.mute = !mute;
    }
}
