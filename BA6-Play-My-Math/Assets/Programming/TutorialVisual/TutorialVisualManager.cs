using System.Collections.Generic;
using Programming.ExtensionMethods;
using UnityEngine;

namespace Programming.TutorialVisual
{
    public class TutorialVisualManager : MonoBehaviour 
    {
        public static TutorialVisualManager Instance;

        [SerializeField] private List<GameObject> TutorialPrefabs;
        [SerializeField] private List<MonoBehaviour> SpawnedTutorialElements;

        public T SpawnOrGetTutorial<T>() where T : MonoBehaviour
        {
            foreach (MonoBehaviour tutorial in SpawnedTutorialElements)
            {
                if (tutorial is T)
                {
                    return tutorial as T; 
                }
            }

            foreach (GameObject tutorialPrefab in TutorialPrefabs)
            {
                if (tutorialPrefab.TryGetComponent<T>(out T tutorial))
                {
                    MonoBehaviour newTutorial = Instantiate(tutorialPrefab, this.transform).GetComponent<T>(); 
                    SpawnedTutorialElements.Add(newTutorial);
                    return newTutorial as T; 
                }
            }

            Debug.Log("TutorialManager does not contain Tutorial of Type: " + nameof(T));
            return null; 
        }

        public void DestroyTutorial<T>() where T : TutorialVisualElement
        {
            TutorialVisualElement toRemove = null; 
            foreach (TutorialVisualElement tutorial in SpawnedTutorialElements)
            {
                if (tutorial is T)
                {
                    toRemove = tutorial; 
                    Destroy(tutorial.gameObject);
                }
            }

            if (toRemove is null)
            {
                Debug.Log("Trying to Destroy Tutorial of Type " + nameof(T) + " but no such Tutorial exists");
                return; 
            }

            SpawnedTutorialElements.Remove(toRemove); 
        }
        
        private void Awake()
        {
            this.MakeSingleton(ref Instance);
        }
    }
}
