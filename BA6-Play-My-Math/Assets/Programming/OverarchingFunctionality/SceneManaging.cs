using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    //private void Awake()
    //{
    //    DontDestroyOnLoad(this.gameObject); 
    //}
    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
