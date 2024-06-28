// #define GetFactor(Vector2 primeFactor) (int)Mathf.Max(primeFactor.x, primeFactor.y)

using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Serialization;

//TODO: fix bug where one Fraction somehow sets the value of the other Fraction

public class FractionVisualizer : MonoBehaviour
{
    public enum VisualisationType
    {
        NONE,
        Left,
        LeftChange,
        Right,
        RightChange
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
        public FractionVisualisationData(Fraction fraction, int minIndex, int maxIndex,
            FractionVisualiserFigureParent parent, OffsetAndSpacing offsetAndSpacing, Vector2[] packingCoordinates)
        {
            Fraction = fraction;
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            VisualisationParent = parent;
            OffsetAndSpacing = offsetAndSpacing;
            PackingCoordinates = packingCoordinates;
        }

        public Fraction Fraction;
        public int MinIndex, MaxIndex;
        public FractionVisualiserFigureParent VisualisationParent;
        public OffsetAndSpacing OffsetAndSpacing;
        public Vector2[] PackingCoordinates;
    }

    //REFERENCES
    [SerializeField] private FractionVisualiserFigureParent leftVisualParent;
    [SerializeField] private FractionVisualiserFigureParent leftChangeVisualParent;

    [SerializeField] private FractionVisualiserFigureParent rightVisualParent;
    [SerializeField] private FractionVisualiserFigureParent rightChangeVisualParent;

    [SerializeField] private MeshFilter boardMeshFilter;

    //EDITOR VARIABLES
    [SerializeField] private Mesh[] boardMeshes;
    [SerializeField] private GameObject[] figurePrefabs;
    [SerializeField] private Vector2Array[] numbersToPrimeFactors;
    [SerializeField] private Vector3 boardSize;

    //CODE VARIABLES
    private Dictionary<VisualisationType, FractionVisualisationData> VisualisationDataMap =
        new Dictionary<VisualisationType, FractionVisualisationData>();

    private Vector2[] _fractionADivisorOrder;
    private Operation _operation;

    //MONOBEHAVIOUR FUNCTIONS
    private void Awake()
    {
    }

    public void VisualiseOperation(Operation operation)
    {
        _operation = operation;

        if (!(VisualisationDataMap.ContainsKey(VisualisationType.Left) && VisualisationDataMap.ContainsKey(VisualisationType.Right)))
        {
            return;
        }

        VisualiseFraction(VisualisationDataMap[VisualisationType.Right].Fraction, VisualisationType.Right);
    }

    //OWN FUNCTIONS
    public void VisualiseFraction(Fraction fraction, VisualisationType visualisationType)
    {
        if (fraction.Equals(null))
        {
            return;
        }
        
        switch (visualisationType)
        {
            case VisualisationType.Left:
                VisualiseLeftCard(fraction);
                if (VisualisationDataMap.ContainsKey(VisualisationType.Right))
                {
                    VisualiseRightCard(VisualisationDataMap[VisualisationType.Right].Fraction);
                }
                break; 
            case VisualisationType.Right:
                VisualiseRightCard(fraction);
                break; 
            case VisualisationType.LeftChange:
                throw new NotImplementedException(); 
                break; 
            case VisualisationType.RightChange:
                throw new NotImplementedException(); 
                break; 
            case VisualisationType.NONE:
                throw new NotSupportedException(); 
                break; 
        }
    }

    private void VisualiseLeftCard(Fraction fraction)
    {
        if (VisualisationDataMap.ContainsKey(VisualisationType.Left))
        {
            VisualisationDataMap[VisualisationType.Left].VisualisationParent.transform.DestroyAllChildren();
            VisualisationDataMap.Remove(VisualisationType.Left);
        }
        if (fraction.Equals(null))
        {
            return;
        }
        
        FractionVisualisationData visualisationData =
            new FractionVisualisationData(
                fraction,
                0,
                fraction.Numerator,
                leftVisualParent,
                new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize),
                CalculatePackingCoordinates(fraction) 
            );

        VisualisationDataMap.Add(VisualisationType.Left, visualisationData);
        
        SpawnFigures(visualisationData);
    }

    private void VisualiseRightCard(Fraction fraction)
    {
        if (VisualisationDataMap.ContainsKey(VisualisationType.Right))
        {
            VisualisationDataMap[VisualisationType.Right].VisualisationParent.transform.DestroyAllChildren();
            VisualisationDataMap.Remove(VisualisationType.Right);
        }
        
        //Deleting for replacement / removing of cards
        Operation operationCopy = _operation;
        int minIndex = 0;
        int maxIndex = 0;
        
        //checking for leftCard
        FractionVisualisationData leftData; 
        Fraction leftFraction = null;
        if (VisualisationDataMap.TryGetValue(VisualisationType.Left, out leftData))
        {
            leftFraction = leftData.Fraction; 
        }
        else
        {
            leftFraction = new Fraction(0, 1); //needs a value to avoid compilation errpr
            operationCopy = Operation.Nop;
        }

        FractionVisualiserFigureParent visualParent;
        OffsetAndSpacing offsetAndSpacing;
        Vector2[] packingCoordinates;

        switch (operationCopy)
        {
            case Operation.Nop:
                minIndex = 0;
                maxIndex = fraction.Numerator;
                offsetAndSpacing = new OffsetAndSpacing(
                    numbersToPrimeFactors[fraction.Denominator].factors,
                    boardSize);
                packingCoordinates = CalculatePackingCoordinates(fraction);
                break;
            case Operation.Add:
                if (!(fraction + leftFraction).IsBetween(0, 1))
                {
                    throw new NotImplementedException();
                }
                minIndex = leftFraction.Numerator;
                maxIndex = leftFraction.Numerator + fraction.Numerator;
                offsetAndSpacing = leftData.OffsetAndSpacing;
                packingCoordinates = leftData.PackingCoordinates;
                break;
            case Operation.Subtract:
                if (!(fraction - leftFraction).IsBetween(0, 1))
                {
                    throw new NotImplementedException();
                }
                minIndex = leftFraction.Numerator - fraction.Numerator;
                maxIndex = leftFraction.Numerator;
                offsetAndSpacing = leftData.OffsetAndSpacing;
                packingCoordinates = leftData.PackingCoordinates;
                break;
            case Operation.Multiply:
                if (!(fraction * leftFraction).IsBetween(0, 1))
                {
                    throw new NotImplementedException();
                }
                minIndex = 0;
                maxIndex = leftFraction.Numerator * fraction.Numerator;
                offsetAndSpacing = new OffsetAndSpacing(
                    numbersToPrimeFactors[leftFraction.Denominator].factors.Concat(
                        numbersToPrimeFactors[fraction.Denominator].factors).ToArray(),
                    boardSize);
                packingCoordinates = CalculatePackingCoordinates(leftFraction * fraction);
                break;
            case Operation.Divide:
                if (!(fraction / leftFraction).IsBetween(0, 1))
                {
                    throw new NotImplementedException();
                }
                minIndex = 0;
                maxIndex = leftFraction.Numerator * fraction.Denominator;
                offsetAndSpacing = new OffsetAndSpacing(
                    numbersToPrimeFactors[leftFraction.Denominator].factors.Concat(
                        numbersToPrimeFactors[fraction.Numerator].factors).ToArray(),
                    boardSize);
                packingCoordinates = CalculatePackingCoordinates(leftFraction / fraction);
                break;
            default:
                throw new NotImplementedException();
        }

        FractionVisualisationData visualisationData =
            new FractionVisualisationData(
                fraction,
                minIndex,
                maxIndex,
                rightVisualParent,
                offsetAndSpacing,
                packingCoordinates
            );

        VisualisationDataMap.Add(VisualisationType.Right, visualisationData);
        SpawnFigures(visualisationData);
    }

    //TODO: Expand, Shorten & Operator

    private Vector2[] CalculatePackingCoordinates(Fraction fraction)
    {
        Vector2[] packingCoordinates = new Vector2[fraction.Denominator];
        Vector2[] divisorOrder = OrderPrimeFactors(numbersToPrimeFactors[fraction.Denominator].factors,
            fraction.Numerator, fraction.Denominator);

        if (debug_divisorOrder.Length > 0)
        {
            divisorOrder = debug_divisorOrder;
        }

        RecursiveCalculateFigurePositions(divisorOrder, 0, 0, Vector2.zero, fraction.Denominator,
            ref packingCoordinates);

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

                Debug.Log("coord" + coordinates + ", rD" + recursionDepth + ", pF" + primeFactor + " index" +
                          figureIndex);

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

    private void SpawnFigures(Transform figureParent, Vector2[] packingCoordinates, OffsetAndSpacing offsetAndSpacing,
        int minIndex, int maxIndex)
    {
        figureParent.transform.DestroyAllChildren();
        for (int i = minIndex; i < maxIndex; i++)
        {
            SpawnFigure(figureParent, figurePrefabs[packingCoordinates.Length], offsetAndSpacing,
                packingCoordinates[i]);
        }
    }

    private void SpawnFigures(FractionVisualisationData visualisationData)
    {
        visualisationData.VisualisationParent.transform.DestroyAllChildren();
        for (int i = visualisationData.MinIndex; i < visualisationData.MaxIndex; i++)
        {
            SpawnFigure(
                visualisationData.VisualisationParent.transform,
                figurePrefabs[visualisationData.PackingCoordinates.Length],
                visualisationData.OffsetAndSpacing,
                visualisationData.PackingCoordinates[i]);
        }

        visualisationData.VisualisationParent.UpdateChildrenVisuals();
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

    public void MoveFiguresAfterOperation()
    {
        throw new NotImplementedException();
    }

    //DEBUG & EDITOR HELPERS
    public Vector2[] debug_divisorOrder;
    public Fraction debug_fraction;
    public VisualisationType debug_VisInputType;
    public Operation debug_operation;

    [ContextMenu("Debug_UpdateFraction")]
    public void Debug_UpdateFraction()
    {
        VisualiseFraction(debug_fraction, debug_VisInputType);
    }

    [ContextMenu("Debug_UpdateOperator")]
    public void Debug_UpdateOperator()
    {
        VisualiseOperation(debug_operation);
    }
}