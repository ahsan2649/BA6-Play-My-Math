// #define GetFactor(Vector2 primeFactor) (int)Mathf.Max(primeFactor.x, primeFactor.y)

using System;
using System.Collections.Generic;
using System.Linq;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class FractionVisualizer : MonoBehaviour
{
    [Serializable]
    public class PrimeFactors
    {
        public List<Vector2> factors; 
    }
    
    //REFERENCES
    [SerializeField] private GameObject fractionAFigureCollection;
    [SerializeField] private GameObject fractionBFigureCollection;
    [SerializeField] private MeshFilter boardMeshFilter; 
    
    //EDITOR VARIABLES
    [SerializeField] private Mesh[] boardMeshes;
    [FormerlySerializedAs("figurePrefab")] [SerializeField] private GameObject[] figurePrefabs;
    [SerializeField] private PrimeFactors[] primeFactors; 
    [SerializeField] private int xSpacingMultiplier;
    [FormerlySerializedAs("ySpacingMultiplier")] [SerializeField] private int zSpacingMultiplier;
    [SerializeField] private Vector3 boardSize; 
    
    //CODE VARIABLES
    private Fraction _fractionA;
    public Fraction FractionA
    {
        get { return _fractionA; }
        set {
            if (value != _fractionA)
            {
                _fractionA = value; 
                StartRecursiveFigureSpawning(fractionAFigureCollection, 0, _fractionA.Numerator, _fractionA.Denominator);
            }
        }
    }
    
    private Fraction _fractionB;
    public Fraction FractionB
    {
        get { return _fractionB; }
        set {
            if (_fractionB != value)
            {
                _fractionB = value; 
                //Start Recursive Figure Spawning etc
            }
        }
    }

    private Operation _operation;

    public Operation Operation
    {
        get { return _operation; }
        set {
            if (value != _operation)
            {
                _operation = value;
                //Start Recursive Figure Spawning etc
            }
        }
    }
    
    //MONOBEHAVIOUR FUNCTIONS
    
    //OWN FUNCTIONS
    public void UpdateVisuals(Fraction fractionA, Fraction fractionB = null, Operation operation = Operation.Nop)
    { 
        boardMeshFilter.mesh = boardMeshes[fractionA.Denominator];
        
        StartRecursiveFigureSpawning(fractionAFigureCollection, 0, fractionA.Numerator, fractionA.Denominator);
    }
    
    public void StartRecursiveFigureSpawning(GameObject parent, int minIndex, int maxIndex, int denominator)
    {
        for (int i = parent.transform.childCount -1; i >= 0; i--)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(parent.transform.GetChild(i).gameObject);
            }
            else
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        
        Vector2 columnsAndRows = Vector2.one;  
        for(int i = 0; i < primeFactors[denominator].factors.Count; i++)
        {
            columnsAndRows *= primeFactors[denominator].factors[i]; 
        }
        
        Vector3 figureSpacing = new Vector3(xSpacingMultiplier/columnsAndRows.x, 0, zSpacingMultiplier/columnsAndRows.y);
        Vector3 baseOffset = new Vector3(-boardSize.x / 2 + figureSpacing.x / 2, 0, boardSize.z / 2 - figureSpacing.z / 2); 
        
        Vector3 coordinates = Vector3.zero;
        int figureIndex = 0;
        
        /* seperate divisors into ones by which the fractions can be shortened and other, then put the two lists together to have a fill-up order
         */
        // 6/36 -> 2*3 / 2v, 2h, 3h, 3v
        // => (2v, 3h, 2h, 3v)
        // make two lists (one for divisors, one for non-divisors)
        
        Vector2[] divisorOrder = new Vector2[primeFactors[denominator].factors.Count];
        {
            List<Vector2> numeratorDivisors = new List<Vector2>(); 
            List<Vector2> notNumeratorDivisors = new List<Vector2>();
            int numeratorCopy = minIndex;
            int denominatorCopy = denominator;
            
            foreach (Vector2 primeFactor in primeFactors[denominator].factors)
            {
                int factor = (int)Mathf.Max(primeFactor.x, primeFactor.y); 
                if (numeratorCopy % factor == 0)
                {
                    numeratorDivisors.Add(primeFactor);
                    numeratorCopy /= factor; 
                }
                else
                {
                    notNumeratorDivisors.Add(primeFactor);
                }
                denominatorCopy /= factor; 
                
                columnsAndRows *= primeFactor;

                divisorOrder = notNumeratorDivisors.Concat(numeratorDivisors).ToArray(); 
            }
        } 
        
        RecursiveFigureSpawning(divisorOrder, 0, 0, minIndex, figureIndex, coordinates, denominator, denominator, parent, figureSpacing, baseOffset);
    }
    
    private void RecursiveFigureSpawning(
        Vector2[] packingOrder, int recursionDepth, 
        int minIndex, int maxIndex, int figureIndex,
        Vector2 coordinates, int denominator, int dividedDenominator,
        GameObject figureParent, Vector3 figureSpacing, Vector3 figureBasicOffset
    )
    {
        if (figureIndex >= maxIndex || figureIndex+dividedDenominator <= minIndex)
        {
            return; 
        }
        if (recursionDepth >= packingOrder.Length)
        {
            SpawnFigure(figureParent, denominator, figureBasicOffset, figureSpacing, coordinates);
            Debug.Log("spawningCoordinates: " + coordinates);
        }
        else
        {
            Vector2 primeFactor = packingOrder[recursionDepth]; 
            
            Debug.Log("coord" + coordinates + ", rD" + recursionDepth + ", pF" + primeFactor + " index" + figureIndex);
            
            coordinates *= primeFactor;
            dividedDenominator /= (int) Mathf.Max(primeFactor.x, primeFactor.y);
            
            for (int i = 0; i < (int) Mathf.Max(primeFactor.x, primeFactor.y); i++)
            {
                RecursiveFigureSpawning(packingOrder, recursionDepth + 1, 
                    minIndex, maxIndex, 
                    figureIndex + dividedDenominator * i, 
                    coordinates + (primeFactor.x > primeFactor.y ? Vector2.right : Vector2.down) * i, denominator, 
                    dividedDenominator, figureParent, figureSpacing, figureBasicOffset);
            }
        }
    }
    
    public void SpawnFigure(GameObject parent, int denominator, 
        Vector3 baseOffset, Vector3 spacing, Vector2 coordinates)
    {
        GameObject figure = Instantiate(figurePrefabs[denominator], parent.transform); 
        figure.transform.localPosition = baseOffset + new Vector3(coordinates.x * spacing.x, 0, coordinates.y * spacing.z);
    }
    
    public void MoveFigureAfterOperation()
    {
        throw new NotImplementedException(); 
    }
    
    //DEBUG & EDITOR HELPERS
    public int debug_numerator;
    public int debug_denominator;

    [ContextMenu("Debug_StartRecursiveFigureSpawning")]
    public void Debug_StartRecursiveFigureSpawning()
    {
        StartRecursiveFigureSpawning(fractionAFigureCollection, 0, debug_numerator, debug_denominator);
    }
    
    [ContextMenu("Editor_GrabMeshesPrefabs")]
    public void Editor_GrabMeshesAndPrefabs()
    {
        throw new NotImplementedException(); 
        // Mesh[] boardsMeshes = Resources.LoadAll<Mesh>(boardsPath);
        // GameObject[] figurePrefabs = Resources.LoadAll<GameObject>(figuresPath); 
        // UnityEngine.Object[] figureObjects = (GameObject[])AssetDatabase.LoadAllAssetsAtPath(figuresPath); 
        // AssetDatabase.CreateFolder(boardsPath, "newFolder"); 
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

        /*
        for (int i = 0; i < boardsMeshes.Length; i++)
        {
            boardMeshes[i].boardMesh = boardsMeshes[i];
        }
        for (int i = 0; i < figurePrefabs.Length; i++)
        {
            denominatorData[i].figurePrefab = figurePrefabs[i];
        }
        */
    }
}