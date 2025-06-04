using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    // PauseUI
    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private Button m_pauseButton;
    [SerializeField] private Button m_resumebutton;
    [SerializeField] private Button[] m_titleButton;

    // GameEndUI
    [SerializeField] private GameObject m_gameOverPanel;
    [SerializeField] private TMP_Text m_gameOverText;
    [SerializeField] private TMP_Text m_gameClearText;
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private Button m_restartButton;

    // 게임 종료 UI를 출력을 띄엄띄엄하게하는 효과용
    // 현재 오류가 있어 추후 반영 예정
    // private Coroutine m_coroutine;

    private void Awake() => Init();

    private void Init()
    {
        m_pauseButton.onClick.AddListener(GamePause);
        m_resumebutton.onClick.AddListener(GameResume);
        for (int i = 0; i < m_titleButton.Length; i++)
        {
            m_titleButton[i].onClick.AddListener(ReturnTitle);
        }
        m_restartButton.onClick.AddListener(GameRestart);
    }

    private void Update()
    {
        if(GameManager.Instance.IsGameOver() || GameManager.Instance.IsGameClear())
        {
            m_gameOverPanel.SetActive(true);
            if (GameManager.Instance.IsGameOver())
            {
                m_gameClearText.enabled = false;
            }
            else if (GameManager.Instance.IsGameClear())
            {
                m_gameOverText.enabled = false;
            }

            int score = GameManager.Instance.GetScore();
            m_scoreText.text = $"Score : {score.ToString("000")}";

            //m_coroutine = StartCoroutine(GameEnd());
        }
    }

    #region PauseMenu

    private void GamePause()
    {
        GameManager.Instance.Pause(true);
        GameManager.Instance.TryPause(true);
    }

    private void GameResume()
    {
        GameManager.Instance.Pause(false);
        GameManager.Instance.TryPause(false);
    }

    #endregion

    #region GameEndMenu

    private void ReturnTitle()
    {
        m_gameOverPanel.SetActive(false);
        AudioManager.Instance.PlayBgm(false);
        GameManager.Instance.Pause(false);
        SceneChanger.Instance.SceneChange(SceneName.TitleScene);
    }

    private void GameRestart()
    {
        /*
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }
        */
        m_gameOverPanel.SetActive(false);
        GameManager.Instance.GameStart();
        GameManager.Instance.Pause(false);
        SceneChanger.Instance.SceneChange(SceneName.Stage1Scene);
    }

    #endregion

    // 게임 종료 시 UI를
    // 게임 오버/게임 승리 -> 스코어 -> 버튼 순으로 순차적으로 띄우는 기능
    // 오류가 있어 반영하지 못하고 수정중
    /*
    IEnumerator GameEnd()
    {
        WaitForSeconds delay = new WaitForSeconds(1);

        m_gameOverPanel.SetActive(true);

        if (GameManager.Instance.IsGameOver())
        {
            m_gameOverText.enabled = true;
            m_gameClearText.enabled = false;
        }
        else if (GameManager.Instance.IsGameClear())
        {
            m_gameOverText.enabled = false;
            m_gameClearText.enabled = true;
        }

        yield return delay;

        int score = GameManager.Instance.GetScore();
        m_scoreText.text = $"Score : {score.ToString("000")}";

        yield return delay;

        m_restartButton.enabled = true;
        m_titleButton[1].enabled = true;
    }
    */
}
