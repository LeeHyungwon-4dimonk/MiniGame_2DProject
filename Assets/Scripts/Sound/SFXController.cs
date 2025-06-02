using UnityEngine;

/// <summary>
/// Name : 해당 사운드가 나는 상황을 적어주세요(ex: 공격, 피격)
/// Clip : 재생할 사운드 파일을 참조해주세요
/// </summary>
[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}

/// <summary>
/// 소리를 내는 오브젝트에 직접 달 수 있는 SFX 사운드 재생 컴포넌트
/// </summary>
public class SFXController : MonoBehaviour
{
    [SerializeField] private Sound[] m_sfx = null;
    private AudioSource m_sfxPlayer;

    private void Awake() => Init();

    private void Init()
    {
        m_sfxPlayer = GetComponent<AudioSource>();
    }

    /// <summary>
    /// SFX를 한 번 재생하는 함수(Name을 맞춰 적어주세요)
    /// </summary>
    /// <param name="p_sfxName"></param>
    public void PlaySFX(string p_sfxName)
    {
        m_sfxPlayer.loop = false;
        for (int i = 0; i < m_sfx.Length; i++)
        {
            if (p_sfxName == m_sfx[i].Name)
            {                
                m_sfxPlayer.clip = m_sfx[i].Clip;
                m_sfxPlayer.Play();
                return;
            }
        }
    }
    
    /// <summary>
    /// SFX를 Loop로 재생하는 함수(Name을 맞춰 적어주세요)
    /// </summary>
    /// <param name="p_sfxName"></param>
    public void LoopSFX(string p_sfxName)
    {
        m_sfxPlayer.loop = true;
        for (int i = 0; i < m_sfx.Length; i++)
        {
            if (p_sfxName == m_sfx[i].Name)
            {
                m_sfxPlayer.clip = m_sfx[i].Clip;
                m_sfxPlayer.Play();
                return;
            }
        }
    }

    /// <summary>
    /// SFX 재생을 멈추는 함수(재생 후에는 꼭 정지 부탁드립니다)
    /// </summary>
    public void StopSFX()
    {
        m_sfxPlayer.Stop();
    }
}
