using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;

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
