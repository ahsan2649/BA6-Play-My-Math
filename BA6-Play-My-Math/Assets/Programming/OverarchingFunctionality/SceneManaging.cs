using Programming.ExtensionMethods;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Programming.OverarchingFunctionality
{
    public class SceneManaging : MonoBehaviour
    {
        public static SceneManaging Instance; 
        public bool bTeacherMode; 
        
        private void Awake()
        {
            this.MakeSingleton(ref Instance, true);
        }
    
        public void changeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
