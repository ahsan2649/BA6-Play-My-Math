using Programming.Enemy;
using Programming.OverarchingFunctionality;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Programming.OverarchingFunctionality.SceneManaging;

public class SceneChanger : MonoBehaviour
{
    GameObject scenemanager; 
    // Start is called before the first frame update
    void Start()
    {
        scenemanager = GameObject.FindGameObjectWithTag("SceneManager");
    }
    public void changeScene(string sceneName)
    {
        scenemanager.GetComponent<SceneManaging>().ChangeScene(sceneName);
    }

    public void setGameMode(string gameModeName)
    {
        scenemanager.GetComponent<SceneManaging>().SetGameMode(gameModeName);
        LevelGeneration.updateGameMode((GameMode)Enum.Parse(typeof(GameMode), gameModeName));
    }

  
}
