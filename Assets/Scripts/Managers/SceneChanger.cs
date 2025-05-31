using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName 
{ 
    TitleScene,
    Stage1Scene,
    Stage2Scene,
}

public class SceneChanger : Singleton<SceneChanger>
{
    public void SceneChange(SceneName sceneName)
    {
        SceneManager.LoadScene($"{sceneName}");
    }
}
