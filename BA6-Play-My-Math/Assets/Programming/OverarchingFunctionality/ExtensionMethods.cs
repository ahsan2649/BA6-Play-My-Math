using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        public static List<T> GetRandomElementsConsecutively<T>(this List<T> list, int count, List<int> probabilities = null)
        {
            List<T> result = new List<T>();

            if (count > list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(list), "Cannot pick more elements than list.Count"); 
            }

            if (count == list.Count)
            {
                return list.FisherYatesShuffle(); 
            }
            
            if (probabilities is null)
            {
                throw new NullReferenceException("no probabilities assigned"); 
            }

            List<T> pickFromThis = new List<T>(list);
            List<int> pickProbabilities = new List<int>(probabilities); 
            while (count > pickFromThis.Count)
            {
                pickFromThis = pickFromThis.Concat(list).ToList();
                pickProbabilities = pickProbabilities.Concat(probabilities).ToList(); 
            }

            for (int i = 0; i < count; i++)
            {
                result.Add(pickFromThis.GetRandomElement(out int removedIndex, pickProbabilities));
                pickProbabilities.RemoveAt(removedIndex);
                pickFromThis.RemoveAt(removedIndex); 
            }

            return result; 
        }
        
        public static T GetRandomElement<T>(this IEnumerable<T> enumerable, IEnumerable<int> probabilities = null)
        {
            if (probabilities is null)
            {
                return enumerable.ElementAt(new Random().Next(0, enumerable.Count())); 
            }
            
            int fullProbability = probabilities.Sum(); 
            Random random = new Random();
            double r = random.NextDouble() * fullProbability;
            
            double sum = 0;
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

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable, out int returnedIndex, IEnumerable<int> probabilities = null)
        {
            if (probabilities is null)
            {
                int randomIndex = new Random().Next(0, enumerable.Count());
                returnedIndex = randomIndex; 
                return enumerable.ElementAt(randomIndex); 
            }

            if (enumerable.Count() != probabilities.Count())
            {
                throw new ArgumentException("enumerable and probabilities are not the same length"); 
            }
            
            
            int fullProbability = probabilities.Sum(); 
            Random random = new Random();
            double r = random.NextDouble() * fullProbability;
            
            double sum = 0; //ZyKa this is a danger for bugs
            int index = 0; 
            foreach (int probability in probabilities)
            {
                sum += probability;
                if (sum >= r)
                {
                    returnedIndex = index; 
                    return enumerable.ElementAt(index); 
                }
                index++; 
            }

            throw new IndexOutOfRangeException("randomListLength and ProbabilityLength do not match"); 
        }
        
        public static List<T> EnsureListMixing<T>(this List<T> toMix, Func<T, T, bool> comparison, int maxSameInRow)
        {
            T[] toMixArray = toMix.ToArray();
            MixOneWay(ref toMixArray, true); 
            MixOneWay(ref toMixArray, false); 
            return toMixArray.ToList(); 
            
            void MixOneWay(ref T[] array, bool forwards)
            {
                Queue<KeyValuePair<int, T>> toSwitch = new Queue<KeyValuePair<int, T>>();
                Queue<Tuple<int, int>> toSwitchTuples = new Queue<Tuple<int, int>>(); 
                int sameElementsInARow = 0;
                T lastT = default; 
            
                for (int i = forwards ? 0 : array.Length-1; 
                     forwards ? i < array.Length : i >= 0; 
                     i = forwards ? i+1 : i-1)
                {
                    if (comparison(array[i], lastT))
                    {
                        sameElementsInARow++;
                        if (sameElementsInARow % (maxSameInRow + 1) == 0)
                        {
                            toSwitch.Enqueue(new KeyValuePair<int, T>(i, array[i])); 
                        }
                    }
                    else
                    {
                        if (toSwitch.Count > 0 && !comparison(toSwitch.Peek().Value, toMix[i]))
                        {
                            toSwitchTuples.Enqueue(new Tuple<int, int>(i, toSwitch.Peek().Key));
                            toSwitch.Dequeue(); 
                        }
                    }
                }

                foreach (Tuple<int, int> switchTuple in toSwitchTuples)
                {
                    (toMix[switchTuple.Item1], toMix[switchTuple.Item2]) =
                        (toMix[switchTuple.Item2], toMix[switchTuple.Item1]); 
                }
            }; 
        }

        /*
        public static List<T> RemoveNulls<T>(this List<T> list)
        {
            foreach (T t in list)
            {
                if (t is null)
                {
                    list.Remove(t); 
                }
            }
        }
        */
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
                Object.DontDestroyOnLoad(Instance.transform.root);
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
