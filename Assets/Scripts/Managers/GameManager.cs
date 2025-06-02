using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private static float m_playerSpellCoolTime = 0;
    private static int m_score;
    private static int m_playerMaxHp;
    private static int m_playerCurHp;
    private static int m_playerMaxMp;
    private static int m_playerCurMp;

    private static bool m_GameOver = false;
    private static bool m_GameClear = false;
    private static bool m_GamePaused = false;

    private void Awake() => Init();

    private void Init()
    {
        base.SingletonInit();
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;
        m_score = 0;
    }

    public void Update()
    {

    }

    public void GameStart()
    {
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;
        m_score = 0;
        m_GameOver = false;
        m_GameClear = false;
        m_GamePaused = false;
        AudioManager.Instance.PlayBgm(true);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        AudioManager.Instance.PlayBgm(false);
        
        m_GameOver = true;
    }

    public bool IsGameOver()
    {
        return m_GameOver;
    }

    public void GameClear()
    {
        Debug.Log("Game Clear");
        AudioManager.Instance.PlayBgm(false);
        m_GameClear = true;
    }

    public bool IsGameClear()
    {
        return m_GameClear;
    }

    public void Pause(bool isPause)
    {
        m_GamePaused = isPause;
        if (m_GamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public bool IsPause()
    {
       return m_GamePaused;
    }

    #region Score

    public void ScorePlus(int score)
    {
        m_score += score;
        Debug.Log($"현재점수 : {m_score}");
    }

    public int GetScore()
    {
        return m_score;
    }

    #endregion

    #region PlayerCoolTime

    public void SetCoolTime()
    {
        if (m_playerSpellCoolTime < 0) return;
        m_playerSpellCoolTime -= Time.deltaTime;
    }

    public void SetCoolTime(int time)
    {
        m_playerSpellCoolTime = time;
    }

    public float GetCoolTime()
    {
        return m_playerSpellCoolTime;
    }
    #endregion

    #region PlayerHp

    public int GetMaxHP()
    {
        return m_playerMaxHp;
    }

    public int GetCurHP()
    {
        if(m_playerCurHp < 0)
            return 0;
        return m_playerCurHp;
    }

    public void DamageHp(int value)
    {
        m_playerCurHp -= value;
        if (m_playerCurHp <= 0) m_playerCurHp = 0;
    }

    public void RecoverHp(int value)
    {
        m_playerCurHp += value;
        if(m_playerCurHp >= m_playerMaxHp) m_playerCurHp = m_playerMaxHp;
    }

    #endregion

    #region PlayerMp

    public int GetMaxMP()
    {
        return m_playerMaxMp;
    }

    public int GetCurMP()
    {
        if (m_playerCurMp < 0)
            return 0;
        return m_playerCurMp;
    }

    public void UseMp(int value)
    {
        m_playerCurMp -= value;
        if (m_playerCurMp <= 0) m_playerCurMp = 0;
    }

    public void RecoverMp(int value)
    {
        m_playerCurMp += value;
        if (m_playerCurMp >= m_playerMaxMp) m_playerCurMp = m_playerMaxMp;
    }

    #endregion
}
