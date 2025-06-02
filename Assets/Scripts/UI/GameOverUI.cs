using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_gameOverText;
    [SerializeField] private TMP_Text m_gameClearText;
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] Button m_restartButton;
    [SerializeField] Button m_titleButton;

    private Coroutine m_coroutine;

    private void Awake() => Init();
    private void Init()
    {
        m_restartButton.onClick.AddListener(GameRestart);
        m_titleButton.onClick.AddListener(ReturnTitle);
        m_gameOverText.enabled = false;
        m_gameClearText.enabled = false;
        m_scoreText.enabled = false;
        m_restartButton.enabled = false;
        m_titleButton.enabled = false;
    }

    private void OnEnable()
    {  
        m_coroutine = StartCoroutine(GameEnd());
    }

    private void GameRestart()
    {
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }

        GameManager.Instance.Pause(false);
        GameManager.Instance.GameStart();
        AudioManager.Instance.PlayBgm(false);
        SceneChanger.Instance.SceneChange(SceneName.Stage1Scene);
        gameObject.SetActive(false);
    }

    private void ReturnTitle()
    {
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }

        GameManager.Instance.Pause(false);
        GameManager.Instance.GameStart();
        AudioManager.Instance.PlayBgm(false);
        SceneChanger.Instance.SceneChange(SceneName.TitleScene);
        gameObject.SetActive(false);
    }

    IEnumerator GameEnd()
    {
        WaitForSeconds delay = new WaitForSeconds(1);

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
        m_titleButton.enabled = true;
    }
}
