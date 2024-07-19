using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    public static SceneManaging Instance; 
    public bool bTeacherMode; 
    
    private void Awake()
    {
        this.MakeSingleton(ref Instance);
        // DontDestroyOnLoad(this.gameObject); 
    }
    
    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
