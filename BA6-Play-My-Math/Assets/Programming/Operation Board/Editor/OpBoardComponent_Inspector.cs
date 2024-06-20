using UnityEditor;
using UnityEngine.UIElements;

namespace Programming.Operation_Board.Editor {
    [CustomEditor(typeof(OpBoardComponent))]
    public class OpBoardComponentInspector : UnityEditor.Editor {
        public VisualTreeAsset inspectorXML;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            
            inspectorXML.CloneTree(inspector);
            
            return inspector;
        }
    }
}
