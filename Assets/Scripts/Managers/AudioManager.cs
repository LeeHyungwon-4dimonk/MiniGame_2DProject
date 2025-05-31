using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("BGM")]
    public AudioClip BgmClip;
    public float BgmVolume;
    private AudioSource m_bgmPlayer;

    [Header("SFX")]
    public AudioClip[] SfxClips;
    public float SfxVolume;
    public int channels;
    AudioSource[] m_sfxPlayers;
    int channelIndex;

    public enum Sfx { };

    private void Awake() => Init();
    private void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        m_bgmPlayer = bgmObject.AddComponent<AudioSource>();
        m_bgmPlayer.playOnAwake = false;
        m_bgmPlayer.loop = true;
        m_bgmPlayer.volume = BgmVolume;
        m_bgmPlayer.clip = BgmClip;


        GameObject sfxObject = new GameObject("SfxObject");
        sfxObject.transform.parent = transform;
        m_sfxPlayers = new AudioSource[channels];

        for(int index = 0; index < m_sfxPlayers.Length; index++)
        {
            m_sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            m_sfxPlayers[index].playOnAwake = false;
            m_sfxPlayers[index].volume = SfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay) m_bgmPlayer.Play();
        else m_bgmPlayer.Stop();
    }


    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < m_sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % m_sfxPlayers.Length;

            if (m_sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            /*
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }
            */

            channelIndex = loopIndex;
            m_sfxPlayers[loopIndex].clip = SfxClips[(int)sfx];
            m_sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
