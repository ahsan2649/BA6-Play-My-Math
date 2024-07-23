using Programming.Card_Mechanism;
using Programming.Visualisers;
using UnityEngine;

namespace Programming.TutorialVisual
{
    public class TutorialVisualCardsInDeck: MonoBehaviour
    {
        public void Start()
        {
            DeckComponentVisualiser dCV = DeckComponent.Instance.gameObject.GetComponent<DeckComponentVisualiser>(); 
            dCV.onDeactivateVisualisation.AddListener(Close);
        }

        private void OnDestroy()
        {
            DeckComponentVisualiser dCV = DeckComponent.Instance.gameObject.GetComponent<DeckComponentVisualiser>(); 
            dCV.onDeactivateVisualisation.RemoveListener(Close);
        }

        private void Close()
        {
            Destroy(this.gameObject);
        }
    }
}
