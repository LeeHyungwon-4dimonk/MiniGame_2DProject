using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject m_panel;
    [SerializeField] private Button m_pauseButton;
    [SerializeField] private Button m_resumebutton;
    [SerializeField] private Button m_titleButton;

    private void Awake() => Init();

    private void Init()
    {
        m_pauseButton.onClick.AddListener(GamePause);
        m_resumebutton.onClick.AddListener(GameResume);
        m_titleButton.onClick.AddListener(ReturnTitle);
    }

    private void GamePause()
    {
        GameManager.Instance.Pause(true);
    }

    private void GameResume()
    {
        GameManager.Instance.Pause(false);
    }

    private void ReturnTitle()
    {
        AudioManager.Instance.PlayBgm(false);
        GameManager.Instance.Pause(false);
        SceneChanger.Instance.SceneChange(SceneName.TitleScene);
    }
}
