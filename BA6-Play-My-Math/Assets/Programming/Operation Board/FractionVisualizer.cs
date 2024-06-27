// #define GetFactor(Vector2 primeFactor) (int)Mathf.Max(primeFactor.x, primeFactor.y)
    
using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Serialization;

public class FractionVisualizer : MonoBehaviour
{
    public enum VisualisationInputType
    {
        Shorten, 
        Expand, 
        LeftOperand,
        RightOperand, 
        Operator
    }

    private enum VisMode
    {
        Shorten, 
        Expand, 
        Left,
        RightNop, 
        Add, 
        Sub, 
        Mult, 
        Div, 
        FullBoard
    }
    
    [Serializable]
    public class Vector2Array
    {
        public Vector2[] factors;
    }

    public struct OffsetAndSpacing
    {
        public OffsetAndSpacing(Vector2[] primeFactors, Vector3 boardSize)
        {
            columnsAndRows = Vector2.one;
            for (int i = 0; i < primeFactors.Length; i++)
            {
                columnsAndRows *= primeFactors[i];
            }

            figureSpacing = new Vector3(boardSize.x / columnsAndRows.x, 0, boardSize.z / columnsAndRows.y);
            baseOffset = new Vector3(-boardSize.x / 2 + figureSpacing.x / 2, 0, boardSize.z / 2 - figureSpacing.z / 2);
        }
        
        public Vector2 columnsAndRows;
        public Vector3 baseOffset; 
        public Vector3 figureSpacing;

        public Vector3 CalculatePosition(Vector2 coordinates)
        {
            return baseOffset + new Vector3(coordinates.x * figureSpacing.x, 0, coordinates.y * figureSpacing.z);
        }
    }
    
    private struct FractionVisualisationData
    {
        public Fraction Fraction;
        public FractionVisualiserFigureParent visualData; 
        public OffsetAndSpacing OffsetAndSpacing;
        public Vector2[] PackingCoordinates; 
    }
    
    //REFERENCES
    [FormerlySerializedAs("fractionAFigureParent")] [SerializeField] private FractionVisualiserFigureParent mainFractionFigureParent;
    [FormerlySerializedAs("fractionBFigureParent")] [SerializeField] private FractionVisualiserFigureParent secondaryFractionFigureParent;
    [SerializeField] private MeshFilter boardMeshFilter;

    //EDITOR VARIABLES
    [SerializeField] private Mesh[] boardMeshes;

    [FormerlySerializedAs("figurePrefab")] [SerializeField]
    private GameObject[] figurePrefabs;
    [SerializeField] private Vector2Array[] numbersToPrimeFactors;
    [SerializeField] private Vector3 boardSize;

    //CODE VARIABLES
    private Dictionary<VisMode, FractionVisualisationData> VisualisationDataMap = new Dictionary<VisMode, FractionVisualisationData>(); 
    
    private Vector2[] _fractionADivisorOrder;
    private Operation _operation; 

    //MONOBEHAVIOUR FUNCTIONS

    //OWN FUNCTIONS
    public void AddVisuals(VisualisationInputType mode, Fraction fraction = null)
    {
        FractionVisualisationData visualisationData = new FractionVisualisationData();
        
        switch (mode)
        {
            case VisualisationInputType.LeftOperand:
                if (VisualisationDataMap.Count == 0)
                {
                    VisualisationDataMap.Add(VisMode.Left, visualisationData);
                    visualisationData.OffsetAndSpacing = new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize);
                    visualisationData.PackingCoordinates = CalculatePackingCoordinates(fraction.Numerator, fraction.Denominator); 

                    SpawnFigures(transform.GetChild(0), visualisationData.PackingCoordinates, visualisationData.OffsetAndSpacing, 0, fraction.Numerator);
                    transform.GetChild(0).GetComponent<FractionVisualiserFigureParent>().UpdateChildrenVisuals(); 
                    visualisationData.visualData = transform.GetChild(0).GetComponent<FractionVisualiserFigureParent>();

                }
                else
                {
                    VisualisationDataMap.Clear(); //ZyKa!
                    //Check for other stuff
                }
                break; 
            case VisualisationInputType.RightOperand:
                if (VisualisationDataMap.Count == 0)
                {
                    
                }
                else
                {
                    
                }
                switch (_operation)
                {
                    case Operation.Nop:

                        break; 
                    case Operation.Add:

                        break; 
                    case Operation.Subtract:

                        break; 
                    case Operation.Multiply:

                        break; 
                    case Operation.Divide:

                        break; 
                }
                break; 
            case VisualisationInputType.Shorten:
                if (VisualisationDataMap.Count == 0)
                {
                    //TODO: Visualise the Shortening
                }
                else
                {
                    //TODO: Check what's there & act according to it
                }
                break; 
            case VisualisationInputType.Expand:
                if (VisualisationDataMap.Count == 0)
                {
                    //TODO: Visualise the Expanding
                }
                else
                {
                    //TODO: Check what's there & act according to it
                }
                break;
            case VisualisationInputType.Operator:
                if (VisualisationDataMap.ContainsKey(VisMode.Left) && VisualisationDataMap.ContainsKey(VisMode.RightNop))
                {
                    //TODO: Act accordingly
                }
                break; 
        }
    }
    
    public void RemoveVisuals(VisualisationInputType mode)
    {
        switch (mode)
        {
            case VisualisationInputType.LeftOperand:
                switch (_operation)
                {
                    
                }
                break; 
            case VisualisationInputType.RightOperand:
                switch (_operation)
                {
                    
                }
                break; 
            case VisualisationInputType.Shorten:
                //Check whether there is left/right/full, act accordingly
                break; 
            case VisualisationInputType.Expand:
                //Check whether there is left/right/full, act accordingly
                break;
        }
    }

    public void UpdateVisuals()
    {
        //TODO update visuals of figures
    }

    public void UpdateOperator(Operation operation)
    {
        _operation = operation;
        //TODO: Update Fraction Visuals
    }
    
    private Vector2[] CalculatePackingCoordinates(int numerator, int denominator)
    {
        Vector2[] packingCoordinates = new Vector2[denominator]; 
        Vector2[] divisorOrder = OrderPrimeFactors(numbersToPrimeFactors[denominator].factors, numerator, denominator);

        if (debug_divisorOrder.Length > 0)
        {
            divisorOrder = debug_divisorOrder;
        }
        
        RecursiveCalculateFigurePositions(divisorOrder, 0, 0, Vector2.zero, denominator, ref packingCoordinates);

        return packingCoordinates; 
        
        void RecursiveCalculateFigurePositions(Vector2[] divisorOrder, int recursionDepth,
            int figureIndex, Vector2 coordinates,
            int dividedDenominator, ref Vector2[] packingOrder)
        {
            if (recursionDepth >= divisorOrder.Length)
            {
                packingOrder[figureIndex] = coordinates; 
            }
            else
            {
                Vector2 primeFactor = divisorOrder[recursionDepth];

                Debug.Log("coord" + coordinates + ", rD" + recursionDepth + ", pF" + primeFactor + " index" + figureIndex);

                coordinates *= primeFactor;
                dividedDenominator /= (int)Mathf.Max(primeFactor.x, primeFactor.y);

                for (int i = 0; i < (int)Mathf.Max(primeFactor.x, primeFactor.y); i++)
                {
                    RecursiveCalculateFigurePositions(divisorOrder, recursionDepth + 1,
                        figureIndex + dividedDenominator * i,
                        coordinates + (primeFactor.x > primeFactor.y ? Vector2.right : Vector2.down) * i,
                        dividedDenominator, ref packingOrder);
                }
            }
        }
    }

    private void SpawnFigures(Transform figureParent, Vector2[] packingCoordinates, OffsetAndSpacing offsetAndSpacing, int minIndex, int maxIndex)
    {
        figureParent.transform.DestroyAllChildren();
        for (int i = minIndex; i < maxIndex; i++)
        {
            SpawnFigure(figureParent, figurePrefabs[packingCoordinates.Length], offsetAndSpacing, packingCoordinates[i]);
        }
    }
    
    public void SpawnFigure(Transform parent, GameObject figurePrefab, OffsetAndSpacing offsetAndSpacing,
        Vector2 coordinates)
    {
        GameObject figure = Instantiate(figurePrefab, parent.transform);
        figure.transform.localPosition = offsetAndSpacing.CalculatePosition(coordinates);
        //updating visuals is done afterwards via the parent
    }

    public Vector2[] OrderPrimeFactors(Vector2[] unorderedDivisors, int numerator, int denominator) 
    {
        List<Vector2> numeratorDivisors = new List<Vector2>();
        List<Vector2> notNumeratorDivisors = new List<Vector2>();
        int numeratorCopy = numerator;
        int denominatorCopy = denominator;

        foreach (Vector2 primeFactor in unorderedDivisors)
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
        }

        return notNumeratorDivisors.Concat(numeratorDivisors).ToArray();
    }

    public void MoveFigureAfterOperation()
    {
        throw new NotImplementedException();
    }

    //DEBUG & EDITOR HELPERS
    public Vector2[] debug_divisorOrder;
    public Fraction debug_fraction;
    public VisualisationInputType debug_VisInputType; 
    
    [ContextMenu("Debug_UpdateMainFraction")]
    public void Debug_UpdateMainFraction()
    {
        // UpdateCard(debug_NumberCardComponent); //TODO Test!
    }

    [ContextMenu("Debug_AddCardVisuals")]
    public void Debug_AddCardVisuals()
    {
        AddVisuals(debug_VisInputType, debug_fraction);
    }
    
    [ContextMenu("Debug_RemoveCardVisuals")]
    public void Debug_RemoveCardVisuals()
    {
        RemoveVisuals(debug_VisInputType);
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
