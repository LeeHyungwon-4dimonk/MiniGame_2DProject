using System;
using DesignPattern;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬이 추가되면 여기에 새로운 씬 이름을 등록해주세요
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
