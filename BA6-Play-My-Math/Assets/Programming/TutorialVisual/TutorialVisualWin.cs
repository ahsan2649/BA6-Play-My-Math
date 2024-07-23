using Programming.Visualisers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Programming.TutorialVisual
{
    public class TutorialVisualWin: TutorialVisualElement, IPointerClickHandler
    {
        [SerializeField] private DeckComponentVisualiser deckComponentVisualiser;
        [SerializeField] private GameObject keepPlayingObject;
        [SerializeField] private GameObject gotoMainMenuObject; 
        
        [SerializeField] private Scene mainMenu; 
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenu.buildIndex); 
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
