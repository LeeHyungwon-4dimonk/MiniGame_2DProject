using System.Collections;
using System.Collections.Generic;
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

    private void GameStart()
    {
        SceneChanger.Instance.SceneChange(SceneName.Stage1Scene);
    }

    private void GameEnd()
    {
        Application.Quit();
    }
}
