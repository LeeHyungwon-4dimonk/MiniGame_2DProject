using System;
using DesignPattern;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� �߰��Ǹ� ���⿡ ���ο� �� �̸��� ������ּ���
/// </summary>
[Serializable]
public enum SceneName 
{ 
    TitleScene,
    Stage1Scene,
    Stage2Scene,
    Stage3_1Scene,
    Stage3_2Scene,
    Stage4Scene,
}

public class SceneChanger : Singleton<SceneChanger>
{
    public void SceneChange(SceneName sceneName)
    {
        SceneManager.LoadScene($"{sceneName}");
    }
}
