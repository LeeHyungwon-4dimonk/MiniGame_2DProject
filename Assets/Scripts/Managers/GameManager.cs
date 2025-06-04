using DesignPattern;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // 플레이어 정보 - Static
    private static float m_playerSpellCoolTime = 0;
    private static int m_score;
    private static int m_playerMaxHp;
    private static int m_playerCurHp;
    private static int m_playerMaxMp;
    private static int m_playerCurMp;

    // 게임 상태 Bool 변수
    private static bool m_GameOver = false;
    private static bool m_GameClear = false;
    private static bool m_GamePaused = false;
    private static bool m_TryPause = false;

    private void Awake() => Init();

    private void Init()
    {
        base.SingletonInit();

        // 플레이어 스텟 초기화
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;

        // 플레이어 점수 초기화
        m_score = 0;
    }

    public void Update() { }

    /// <summary>
    /// 게임 초기화 함수
    /// * 타이틀 씬으로 이동할 때는 사용X 1스테이지로 전환시에 사용
    /// </summary>
    public void GameStart()
    {
        // 플레이어 스텟 초기화
        m_playerMaxHp = 10;
        m_playerCurHp = m_playerMaxHp;
        m_playerMaxMp = 5;
        m_playerCurMp = m_playerMaxMp;

        // 플레이어 점수 초기화
        m_score = 0;

        // 게임 상태 초기화
        m_GameOver = false;
        m_GameClear = false;
        m_GamePaused = false;
        Time.timeScale = 1;

        // 인게임 사운드 재생
        AudioManager.Instance.PlayBgm(true);
    }

    #region GameOver

    /// <summary>
    /// 게임 오버 선언
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over");
        AudioManager.Instance.PlayBgm(false);
        Time.timeScale = 0;
        
        m_GameOver = true;
    }

    // 게임 오버 여부 확인
    public bool IsGameOver()
    {
        return m_GameOver;
    }

    #endregion

    #region GameClear

    /// <summary>
    /// 게임 클리어 선언
    /// </summary>
    public void GameClear()
    {
        Debug.Log("Game Clear");
        AudioManager.Instance.PlayBgm(false);
        Time.timeScale = 0;

        m_GameClear = true;
    }

    // 게임 클리어 여부 확인
    public bool IsGameClear()
    {
        return m_GameClear;
    }

    #endregion

    #region GamePause

    /// <summary>
    /// 게임 일시중지 선언 - bool 변수로 중지여부 변경 가능
    /// </summary>
    /// <param name="isPause"></param>
    public void Pause(bool isPause)
    {
        m_GamePaused = isPause;
        if (m_GamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    // 게임 일시중지 여부 확인
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
    /// 스코어 추가 선언
    /// </summary>
    /// <param name="score"></param>
    public void ScorePlus(int score)
    {
        m_score += score;
        Debug.Log($"현재점수 : {m_score}");
    }

    // 스코어 확인
    public int GetScore()
    {
        return m_score;
    }

    #endregion

    #region PlayerCoolTime

    /// <summary>
    /// 플레이어 스펠 쿨타임 경과 반영
    /// </summary>
    public void SetCoolTime()
    {
        if (m_playerSpellCoolTime < 0) return;
        m_playerSpellCoolTime -= Time.deltaTime;
    }

    /// <summary>
    /// 플레이어 쿨타임 설정(시간)
    /// </summary>
    /// <param name="time"></param>
    public void SetCoolTime(float time)
    {
        m_playerSpellCoolTime = time;
    }

    // 플레이어 스펠 쿨타임 정보 확인
    public float GetCoolTime()
    {
        return m_playerSpellCoolTime;
    }

    #endregion

    #region PlayerHp

    // 플레이어 최대 체력 정보 확인
    public int GetMaxHP()
    {
        return m_playerMaxHp;
    }

    // 플레이어 현재 체력 정보 확인
    public int GetCurHP()
    {
        if(m_playerCurHp < 0)
            return 0;
        return m_playerCurHp;
    }

    /// <summary>
    /// 플레이어 체력 데미지 (value 수치만큼)
    /// </summary>
    /// <param name="value"></param>
    public void DamageHp(int value)
    {
        m_playerCurHp -= value;
        if (m_playerCurHp <= 0) m_playerCurHp = 0;
    }

    /// <summary>
    /// 플레이어 체력 회복 (value 수치만큼)
    /// </summary>
    /// <param name="value"></param>
    public void RecoverHp(int value)
    {
        m_playerCurHp += value;
        if(m_playerCurHp >= m_playerMaxHp) m_playerCurHp = m_playerMaxHp;
    }

    #endregion

    #region PlayerMp

    // 플레이어 최대 마나 정보 확인
    public int GetMaxMP()
    {
        return m_playerMaxMp;
    }

    // 플레이어 현재 마나 정보 확인
    public int GetCurMP()
    {
        if (m_playerCurMp < 0)
            return 0;
        return m_playerCurMp;
    }

    /// <summary>
    /// 플레이어 마나 소모 (value 수치만큼)
    /// </summary>
    /// <param name="value"></param>
    public void UseMp(int value)
    {
        m_playerCurMp -= value;
        if (m_playerCurMp <= 0) m_playerCurMp = 0;
    }

    /// <summary>
    /// 플레이어 마나 회복 (value 수치만큼)
    /// </summary>
    /// <param name="value"></param>
    public void RecoverMp(int value)
    {
        m_playerCurMp += value;
        if (m_playerCurMp >= m_playerMaxMp) m_playerCurMp = m_playerMaxMp;
    }

    #endregion
}