using System;
using Programming.ExtensionMethods;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace Programming.OverarchingFunctionality
{
    public class SceneManaging : MonoBehaviour
    {
        public enum GameMode { none, easy23, medium235, hard2357, easyAdditionSmallNumbers, mediumAddition, multiplicationOnly, teachingMode, tutorial, daily23, daily235, daily2357 }

        public static GameMode gameMode { get; set; } = GameMode.none;


        public static SceneManaging Instance; 
        public bool bTeacherMode; 
        
        private void Awake()
        {
            this.MakeSingleton(ref Instance, true);
        }
    
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("changeScene");
        }

        public void SetGameMode(string gameModeName)
        {
            gameMode = (GameMode) Enum.Parse(typeof(GameMode), gameModeName);
        }
    }
}
