using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DesignPattern;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private static float m_playerSpellCoolTime = 0;
    private static int m_score;
    private static int m_playerMaxHp;
    private static int m_playerCurHp;

    private static bool m_GameOver = false;

    private void Awake() => Init();

    private void Init()
    {
        base.SingletonInit();
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_score = 0;
    }

    public void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        AudioManager.Instance.PlayBgm(true);
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
        if (m_playerCurHp < 0) m_playerCurHp = 0;
    }

    public void RecoverHp(int value)
    {
        m_playerCurHp += value;
        if(m_playerCurHp > m_playerMaxHp) m_playerCurHp = m_playerMaxHp;
    }

    #endregion

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


}
