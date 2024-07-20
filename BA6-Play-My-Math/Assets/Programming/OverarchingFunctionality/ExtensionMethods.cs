using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Programming.ExtensionMethods
{
    public static class ExtensionMethods
    {
        #region DataStructures
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

        public static T RandomElement<T>(this IEnumerable<T> enumerable, IEnumerable<int> probabilities = null)
        {
            if (probabilities is null)
            {
                return enumerable.ElementAt(new Random().Next(0, enumerable.Count())); 
            }
            
            int fullProbability = probabilities.Sum(); 
            Random random = new Random();
            int r = random.Next(0, fullProbability);
            
            int sum = 0;
            int index = 0; 
            foreach (int probability in probabilities)
            {
                sum += probability;
                if (sum >= r)
                {
                    return enumerable.ElementAt(index); 
                }
                index++; 
            }

            throw new IndexOutOfRangeException("randomListLength and ProbabilityLength do not match"); 
        }
        #endregion
        
        #region DoForHierarchy
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

        #region GetInHierarchy
        public static T GetComponentInSiblings<T>(this Transform monoBehaviour) where T : MonoBehaviour
        {
            return monoBehaviour.transform.parent.GetComponentInChildren<T>(); 
        }

        public static T GetComponentInAncestors<T>(this Transform thisTransform) where T : MonoBehaviour
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
        #endregion
        
        #endregion
        
        #region MonoBehaviour
        public static void MakeSingleton<T>(this T newMonoBehaviour, ref T Instance, bool bDontDestroyOnLoad = false) where T : MonoBehaviour
        {
            if (Instance is null || Instance == newMonoBehaviour)
            {
                Instance = newMonoBehaviour; 
                Object.DontDestroyOnLoad(Instance.gameObject);
            }
            else
            {
                Object.Destroy(newMonoBehaviour.gameObject);
            }
        }
        
        #region Transform
        public static void SetGlobalScale(this Transform transform, Vector3 scale)
        {
            Transform parent = transform.parent; 
            transform.SetParent(null);
            transform.localScale = scale;
            transform.SetParent(parent); 
        }
        #endregion
        #endregion
        
        
    }
}
