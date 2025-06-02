using DesignPattern;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // Bgm 관련 변수
    [Header("BGM")]
    public AudioClip BgmClip;
    public float BgmVolume;
    private AudioSource m_bgmPlayer;

    // Sfx 관련 변수
    [Header("SFX")]
    public AudioClip[] SfxClips;
    public float SfxVolume;
    public int channels;
    AudioSource[] m_sfxPlayers;
    int channelIndex;

    /// <summary>
    /// Sfx 사용 시 여기에 사용 이름 등록 바랍니다
    /// </summary>
    public enum Sfx { };

    private void Awake() => Init();
    private void Init()
    {
        // Bgm 초기 세팅
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        m_bgmPlayer = bgmObject.AddComponent<AudioSource>();
        m_bgmPlayer.playOnAwake = false;
        m_bgmPlayer.loop = true;
        m_bgmPlayer.volume = BgmVolume;
        m_bgmPlayer.clip = BgmClip;

        // Sfx 초기 세팅
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

    /// <summary>
    /// Bgm 재생 기능 (bool - true : 재생 / false : 정지)
    /// </summary>
    /// <param name="isPlay"></param>
    public void PlayBgm(bool isPlay)
    {
        if (isPlay) m_bgmPlayer.Play();
        else m_bgmPlayer.Stop();
    }

    /// <summary>
    /// Sfx 재생 기능 (enum으로 등록한 이름으로 재생)
    /// </summary>
    /// <param name="sfx"></param>
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < m_sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % m_sfxPlayers.Length;

            if (m_sfxPlayers[loopIndex].isPlaying)
                continue;
            /*
            int ranIndex = 0;
            
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
