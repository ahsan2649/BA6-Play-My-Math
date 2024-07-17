using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Programming.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void DoForAllDescendants(this Transform transform, Action<Transform> action, bool bActOnSelf = true)
        {
            if (bActOnSelf){action.Invoke(transform); }
            foreach (Transform child in transform)
            {
                DoForAllDescendantsRecursion(child, action);
            }

            void DoForAllDescendantsRecursion(Transform descendantTransform, Action<Transform> action)
            {
                action.Invoke(descendantTransform);
                foreach (Transform child in descendantTransform)
                {
                    DoForAllDescendantsRecursion(child, action);
                }
            }
        }
        
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
        
        public static T GetComponentInSiblings<T>(this Transform monoBehaviour)
        {
            return monoBehaviour.transform.parent.GetComponentInChildren<T>(); 
        }

        public static T GetComponentInAncestors<T>(this Transform thisTransform)
        { 
            Transform checkTransform = thisTransform;
            T result; 
            while (checkTransform is not null)
            {
                if (checkTransform.TryGetComponent(out result))
                { 
                    return result; 
                }
                checkTransform = checkTransform.parent; 
            }
            return default; 
        }

        public static void SetGlobalScale(this Transform transform, Vector3 scale)
        {
            Transform parent = transform.parent; 
            transform.SetParent(null);
            transform.localScale = scale;
            transform.SetParent(parent); 
        }

        public static List<T> FisherYatesShuffle<T>(this List<T> shuffleList)
        {
            Random random = new Random(); 
            int n = shuffleList.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(0, n);
                (shuffleList[k], shuffleList[n]) = (shuffleList[n], shuffleList[k]); 
            }

            return shuffleList; 
        }
    }
}
