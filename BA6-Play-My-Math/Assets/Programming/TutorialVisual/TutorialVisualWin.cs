using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Programming.TutorialVisual
{
    public class TutorialVisualWin: MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject keepPlayingObject;
        [SerializeField] private GameObject gotoMainMenuObject; 
        
        [SerializeField] private Scene mainMenu;
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu"); 
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.hovered.Contains(keepPlayingObject))
            {
                Destroy(this.gameObject);
                return; 
            }
            if (eventData.hovered.Contains(gotoMainMenuObject))
            {
                Destroy(this.gameObject);
                LoadMainMenu();   
            }
        }
    }
}
