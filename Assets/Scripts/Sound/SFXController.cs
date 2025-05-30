using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}

public class SFXController : MonoBehaviour
{
    [SerializeField] private Sound[] m_sfx = null;
    private AudioSource m_sfxPlayer;

    private void Awake() => Init();

    private void Init()
    {
        m_sfxPlayer = GetComponent<AudioSource>();
    }

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

    public void StopSFX()
    {
        m_sfxPlayer.Stop();
    }
}
