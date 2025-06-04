using DesignPattern;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // �÷��̾� ���� - Static
    private static float m_playerSpellCoolTime = 0;
    private static int m_score;
    private static int m_playerMaxHp;
    private static int m_playerCurHp;
    private static int m_playerMaxMp;
    private static int m_playerCurMp;

    // ���� ���� Bool ����
    private static bool m_GameOver = false;
    private static bool m_GameClear = false;
    private static bool m_GamePaused = false;
    private static bool m_TryPause = false;

    private void Awake() => Init();

    private void Init()
    {
        base.SingletonInit();

        // �÷��̾� ���� �ʱ�ȭ
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;

        // �÷��̾� ���� �ʱ�ȭ
        m_score = 0;
    }

    public void Update() { }

    /// <summary>
    /// ���� �ʱ�ȭ �Լ�
    /// * Ÿ��Ʋ ������ �̵��� ���� ���X 1���������� ��ȯ�ÿ� ���
    /// </summary>
    public void GameStart()
    {
        // �÷��̾� ���� �ʱ�ȭ
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;

        // �÷��̾� ���� �ʱ�ȭ
        m_score = 0;

        // ���� ���� �ʱ�ȭ
        m_GameOver = false;
        m_GameClear = false;
        m_GamePaused = false;
        Time.timeScale = 1;

        // �ΰ��� ���� ���
        AudioManager.Instance.PlayBgm(true);
    }

    #region GameOver

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over");
        AudioManager.Instance.PlayBgm(false);
        Time.timeScale = 0;
        
        m_GameOver = true;
    }

    // ���� ���� ���� Ȯ��
    public bool IsGameOver()
    {
        return m_GameOver;
    }

    #endregion

    #region GameClear

    /// <summary>
    /// ���� Ŭ���� ����
    /// </summary>
    public void GameClear()
    {
        Debug.Log("Game Clear");
        AudioManager.Instance.PlayBgm(false);
        Time.timeScale = 0;

        m_GameClear = true;
    }

    // ���� Ŭ���� ���� Ȯ��
    public bool IsGameClear()
    {
        return m_GameClear;
    }

    #endregion

    #region GamePause

    /// <summary>
    /// ���� �Ͻ����� ���� - bool ������ �������� ���� ����
    /// </summary>
    /// <param name="isPause"></param>
    public void Pause(bool isPause)
    {
        m_GamePaused = isPause;
        if (m_GamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    // ���� �Ͻ����� ���� Ȯ��
    public bool IsPause()
    {
       return m_GamePaused;
    }

    #endregion

    public void TryPause(bool isPause)
    {
        m_TryPause = isPause;
    }

    public bool IsTryPause()
    {
        return m_TryPause;
    }

    #region Score

    /// <summary>
    /// ���ھ� �߰� ����
    /// </summary>
    /// <param name="score"></param>
    public void ScorePlus(int score)
    {
        m_score += score;
        Debug.Log($"�������� : {m_score}");
    }

    // ���ھ� Ȯ��
    public int GetScore()
    {
        return m_score;
    }

    #endregion

    #region PlayerCoolTime

    /// <summary>
    /// �÷��̾� ���� ��Ÿ�� ��� �ݿ�
    /// </summary>
    public void SetCoolTime()
    {
        if (m_playerSpellCoolTime < 0) return;
        m_playerSpellCoolTime -= Time.deltaTime;
    }

    /// <summary>
    /// �÷��̾� ��Ÿ�� ����(�ð�)
    /// </summary>
    /// <param name="time"></param>
    public void SetCoolTime(float time)
    {
        m_playerSpellCoolTime = time;
    }

    // �÷��̾� ���� ��Ÿ�� ���� Ȯ��
    public float GetCoolTime()
    {
        return m_playerSpellCoolTime;
    }

    #endregion

    #region PlayerHp

    // �÷��̾� �ִ� ü�� ���� Ȯ��
    public int GetMaxHP()
    {
        return m_playerMaxHp;
    }

    // �÷��̾� ���� ü�� ���� Ȯ��
    public int GetCurHP()
    {
        if(m_playerCurHp < 0)
            return 0;
        return m_playerCurHp;
    }

    /// <summary>
    /// �÷��̾� ü�� ������ (value ��ġ��ŭ)
    /// </summary>
    /// <param name="value"></param>
    public void DamageHp(int value)
    {
        m_playerCurHp -= value;
        if (m_playerCurHp <= 0) m_playerCurHp = 0;
    }

    /// <summary>
    /// �÷��̾� ü�� ȸ�� (value ��ġ��ŭ)
    /// </summary>
    /// <param name="value"></param>
    public void RecoverHp(int value)
    {
        m_playerCurHp += value;
        if(m_playerCurHp >= m_playerMaxHp) m_playerCurHp = m_playerMaxHp;
    }

    #endregion

    #region PlayerMp

    // �÷��̾� �ִ� ���� ���� Ȯ��
    public int GetMaxMP()
    {
        return m_playerMaxMp;
    }

    // �÷��̾� ���� ���� ���� Ȯ��
    public int GetCurMP()
    {
        if (m_playerCurMp < 0)
            return 0;
        return m_playerCurMp;
    }

    /// <summary>
    /// �÷��̾� ���� �Ҹ� (value ��ġ��ŭ)
    /// </summary>
    /// <param name="value"></param>
    public void UseMp(int value)
    {
        m_playerCurMp -= value;
        if (m_playerCurMp <= 0) m_playerCurMp = 0;
    }

    /// <summary>
    /// �÷��̾� ���� ȸ�� (value ��ġ��ŭ)
    /// </summary>
    /// <param name="value"></param>
    public void RecoverMp(int value)
    {
        m_playerCurMp += value;
        if (m_playerCurMp >= m_playerMaxMp) m_playerCurMp = m_playerMaxMp;
    }

    #endregion
}