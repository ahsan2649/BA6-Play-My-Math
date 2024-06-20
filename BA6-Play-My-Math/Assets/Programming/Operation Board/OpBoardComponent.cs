using System;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Operation_Board {
    public class OpBoardComponent : MonoBehaviour {
        private OperationBoard _opBoard;
        public Fraction leftOperand;
        public Fraction rightOperand;

        private void Awake()
        {
            _opBoard.UpdateVisual += UpdateVisual;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnDestroy()
        {
            _opBoard.UpdateVisual -= UpdateVisual;
        }

        [ContextMenu("Update Visual")]
        void UpdateVisual()
        {
            Debug.Log("UpdatedVisual");
        }
    }
}