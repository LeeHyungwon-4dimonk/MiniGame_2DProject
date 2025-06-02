using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUI : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_endButton;

    private void Awake()
    {
        m_startButton.onClick.AddListener(GameStart);
        m_endButton.onClick.AddListener(GameEnd);
    }

    // 게임 시작
    private void GameStart()
    {
        SceneChanger.Instance.SceneChange(SceneName.Stage1Scene);
        GameManager.Instance.GameStart();
        AudioManager.Instance.PlayBgm(true);
    }

    // 게임 종료
    private void GameEnd()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        m_startButton?.onClick.RemoveListener(GameStart);
        m_endButton?.onClick.RemoveListener(GameEnd);
    }
}
