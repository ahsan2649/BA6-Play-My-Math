using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Fraction_Engine
{
    public class FractionVisualisationLayer : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public int layerNumber;
        [SerializeField] public FractionVisualiser parent;

        private void Awake()
        {
            if (parent is null)
            {
                Debug.Log("setting parent of FractionVisualisationLayer via Code, not recommended for Runtime"); 
                
                Transform iterationObject = transform; 
                while (parent is null && iterationObject is not null)
                {
                    parent = iterationObject.GetComponent<FractionVisualiser>();
                    iterationObject = iterationObject.parent; 
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            parent.VisualiseLayer(layerNumber);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            parent.VisualiseLayer(layerNumber);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            parent.VisualiseLayer(parent.topLayer);
        }
    }
}
