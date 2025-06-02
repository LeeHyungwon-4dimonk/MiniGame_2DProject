using System.Collections;
using System.Collections.Generic;
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

    // GameOverUI
    [SerializeField] private GameObject m_gameOverPanel;
    [SerializeField] private TMP_Text m_gameOverText;
    [SerializeField] private TMP_Text m_gameClearText;
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private Button m_restartButton;

    private Coroutine m_coroutine;

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
    }

    private void GameResume()
    {
        GameManager.Instance.Pause(false);
    }

    #endregion

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
}
