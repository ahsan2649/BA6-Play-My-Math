using UnityEditor; 
using UnityEngine;

namespace Programming.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount -1; i >= 0; i--)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
                else
                {
                    Object.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        public static void DestroyAllGrandChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                child.DestroyAllChildren();
            }
        }

        public static MonoBehaviour GetMonoBehaviourFromSibling(this Transform transform)
        {
            return transform.parent.GetComponentInChildren<MonoBehaviour>(); 
        }
        
        public static T GetComponentInSiblings<T>(this MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.transform.parent.GetComponentInChildren<T>(); 
        }

        public static void SetGlobalScale(this Transform transform, Vector3 scale)
        {
            Transform parent = transform.parent; 
            transform.SetParent(null);
            transform.localScale = scale;
            transform.SetParent(parent); 
        }
    }
}
