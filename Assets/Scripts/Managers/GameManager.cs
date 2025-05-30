using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private static float m_playerSpellCoolTime = 0;
    private static int m_score;

    private void Awake() => Init();

    private void Init()
    {
        base.SingletonInit();
    }

    public void ScorePlus(int score)
    {
        m_score += score;
        Debug.Log($"현재점수 : {m_score}");
    }

    public int GetScore()
    {
        return m_score;
    }


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
}
