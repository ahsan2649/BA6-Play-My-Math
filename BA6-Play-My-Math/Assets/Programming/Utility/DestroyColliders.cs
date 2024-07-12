using UnityEditor;
using UnityEngine;

namespace Programming.Utility
{
    public class DestroyColliders : MonoBehaviour
    {
        [ContextMenu("DeleteColliders")]
        public void deleteColliders()
        {
            Undo.SetCurrentGroupName("DeleteColliders");
            foreach (Collider collider in FindObjectsOfType<Collider>())
            {
                Debug.Log("Destroy(" + collider.name + ")"); 
                Undo.DestroyObjectImmediate(collider);
            }
        }
    
    
    }
}
