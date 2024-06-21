using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Programming.Operation_Board.Editor {
    [CustomEditor(typeof(OpBoardComponent))]
    public class OpBoardComponentInspector : UnityEditor.Editor {
        public VisualTreeAsset inspectorXML;
        private OpBoardComponent _opBoardComponent;
        private VisualElement _root;
        private VisualTreeAsset _treeAsset;
        private StyleSheet _styleSheet;

        private void OnEnable()
        {
            _root = new VisualElement();
            _treeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Programming/Operation Board/Editor/OpBoardComponent_UXML.uxml");
            _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Programming/Operation Board/Editor/OpBoardComponent_Styles.uss");

            _treeAsset.CloneTree(_root);
            _root.styleSheets.Add(_styleSheet);

            _opBoardComponent = (OpBoardComponent)target;
            
            _root.Q<Button>("Left").clicked += SetLeftFraction;
        }

        public override VisualElement CreateInspectorGUI()
        {
            
            
            
            return _root;
        }

        public void SetLeftFraction()
        {
            Debug.Log("Setting Left Fraction");
        }
    }
}
