using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
