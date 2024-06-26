// #define GetFactor(Vector2 primeFactor) (int)Mathf.Max(primeFactor.x, primeFactor.y)

using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class FractionVisualizer : MonoBehaviour
{
    //SUBCLASSES
    [Serializable]
    public class DenominatorData
    {
        [Serializable]
        public enum Orientation
        {
            Horizontal, 
            Vertical
        }
        
        public Mesh boardMesh;
        public GameObject figurePrefab;
        public List<Vector2> _primeFactorOrientations; 
    }

    //REFERENCES
    [SerializeField] private GameObject fractionAFigureCollection;
    [SerializeField] private GameObject fractionBFigureCollection;
    [SerializeField] private Mesh boardMesh; 
    
    //EDITOR VARIABLES
    [SerializeField] private DenominatorData[] denominatorData;
    [SerializeField] private String boardsPath;
    [SerializeField] private String figuresPath; 
    [SerializeField] private int xSpacingMultiplier;
    [FormerlySerializedAs("ySpacingMultiplier")] [SerializeField] private int zSpacingMultiplier;
    [SerializeField] private Vector3 basicOffset; 
    
    //CODE VARIABLES
    
    //MONOBEHAVIOUR FUNCTIONS
    
    //OWN FUNCTIONS
    public void UpdateVisuals(Fraction fractionA, Fraction fractionB = null, Operation operation = Operation.Nop)
    {
        DenominatorData denominatorAData = denominatorData[fractionA.Denominator]; 
        boardMesh = denominatorAData.boardMesh;

        Vector2 rowsAndColumns = Vector2.one; 
        foreach (Vector2 primeFactor in denominatorAData._primeFactorOrientations)
        {
            rowsAndColumns *= primeFactor; 
        }
        
        Vector3 figureSpacing = new Vector3(xSpacingMultiplier/rowsAndColumns.x, 0, zSpacingMultiplier/rowsAndColumns.y);
        Vector3 currentOffset = basicOffset + figureSpacing / 2; 
        
        // RecursiveFigureSpawningStart(); TODO: Put in the correct Values here
        
        // Spawning of Secondary Figures&Board (with other shader etc.)
    }
    
    public void SpawnFigure(Vector3 coordinates)
    {
        Debug.Log("spawn at: " + coordinates);
        throw new NotImplementedException(); 
    }

    public void RecursiveFigureSpawningStart(int numerator, int denominator)
    {
        Vector3 coordinates = Vector3.zero;
        int figureIndex = 0;
        Vector2 columnsAndRows = Vector2.one; 
        
        /* seperate divisors into ones by which the fractions can be shortened and other, then put the two lists together to have a fill-up order
         */
        Vector2[] divisorOrder = new Vector2[denominatorData[denominator]._primeFactorOrientations.Count];  //TODO: adjust the order here, so that the least denominator stays last
        {
            List<Vector2> notNumeratorDivisors = new List<Vector2>();

            int numeratorCopy = numerator;
            int denominatorCopy = denominator;
            int divisorOrderIndex = denominatorData[denominator]._primeFactorOrientations.Count - 1; 
            foreach (Vector2 primeFactor in denominatorData[denominator]._primeFactorOrientations)
            {
                if ((numeratorCopy % (int) Mathf.Max(primeFactor.x, primeFactor.y) == 0))
                {
                    numeratorCopy /= (int) Mathf.Max(primeFactor.x, primeFactor.y);
                    denominatorCopy /= (int) Mathf.Max(primeFactor.x, primeFactor.y);
                    divisorOrder[divisorOrderIndex] = primeFactor;
                    divisorOrderIndex--; 
                }
                else
                {
                    notNumeratorDivisors.Add(primeFactor);
                }

                columnsAndRows *= primeFactor; 
            }

            foreach (Vector2 pfo in notNumeratorDivisors)
            {
                divisorOrder[divisorOrderIndex] = pfo;
                divisorOrderIndex--; 
            }
            
            foreach (Vector2 pfo in divisorOrder) //ZyKa!
            {
                Debug.Log(pfo);
            }
        } 
        
        RecursiveFigureSpawning(divisorOrder, 0, 0, numerator, figureIndex, coordinates, denominator);
    }
    
    private void RecursiveFigureSpawning(
        Vector2[] packingOrder, //TODO: make the order here work properly
        int recursionDepth, 
        int minIndex, int maxIndex, int figureIndex,
        Vector2 coordinates, int dividedDenominator
    )
    {
        //rD 0
        //TODO pO from last remaining to first merging
        //figureIndex 0
        //coordinates 0, 0
        //dividedDenominator 0

        if (recursionDepth >= packingOrder.Length)
        {
            Debug.Log("spawningCoordinates: " + coordinates);
        }
        else
        {
            Vector2 primeFactor = packingOrder[recursionDepth]; 
            
            // Debug.Log("coord" + coordinates + ", rD" + recursionDepth + " pF" + primeFactor);
            
            coordinates *= primeFactor;
            dividedDenominator /= (int) Mathf.Max(primeFactor.x, primeFactor.y);
            
            for (int i = 0; i < (int) Mathf.Max(primeFactor.x, primeFactor.y); i++)
            {
                RecursiveFigureSpawning(packingOrder, recursionDepth + 1, 
                    minIndex, maxIndex, 
                    figureIndex + dividedDenominator * i, 
                    coordinates + (primeFactor.x > primeFactor.y ? Vector2.right : Vector2.up) * i, 
                    dividedDenominator);
            }
        }
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
        RecursiveFigureSpawningStart(debug_numerator, debug_denominator);
    }
    
    [ContextMenu("Editor_GrabMeshesPrefabs")]
    public void Editor_GrabMeshesAndPrefabs()
    {
        Mesh[] boardsMeshes = Resources.LoadAll<Mesh>(boardsPath);
        GameObject[] figurePrefabs = Resources.LoadAll<GameObject>(figuresPath); 
        // UnityEngine.Object[] figureObjects = (GameObject[])AssetDatabase.LoadAllAssetsAtPath(figuresPath); 
        AssetDatabase.CreateFolder(boardsPath, "ZyKa"); 
        
        for (int i = 0; i < boardsMeshes.Length; i++)
        {
            denominatorData[i].boardMesh = boardsMeshes[i]; 
        }
        for (int i = 0; i < figurePrefabs.Length; i++)
        {
            denominatorData[i].figurePrefab = figurePrefabs[i]; 
        }
    }
}