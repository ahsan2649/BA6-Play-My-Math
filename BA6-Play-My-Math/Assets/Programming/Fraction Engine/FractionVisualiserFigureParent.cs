using System;
using UnityEngine;

namespace Programming.Operation_Board
{
    public class FractionVisualiserFigureParent : MonoBehaviour
    {
        [Serializable]
        public struct FiguresVisualData
        {
            public Material figureMaterial;   
        }

        public FiguresVisualData visualData; 
    
        [ContextMenu("UpdateChildrenVisuals")]
        public void UpdateChildrenVisuals()
        {
            foreach (Transform childTransform in transform)
            {
                childTransform.GetComponent<MeshRenderer>().material = visualData.figureMaterial; 
            }
        }
    }
}
