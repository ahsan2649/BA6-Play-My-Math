using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;

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
    public struct Vector2Array
    {
        public Vector2Array(Vector2[] factors)
        {
            this.factors = factors;
        }

        public Vector2Array(List<Vector2> factors)
        {
            this.factors = factors.ToArray();
        }

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

    [SerializeField] private MeshRenderer boardRenderer;

    //EDITOR VARIABLES
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
        transform.DestroyAllGrandChildren();
    }

    public void VisualiseOperation(Operation operation)
    {
        _operation = operation;

        if (!(VisualisationDataMap.ContainsKey(VisualisationType.Left) &&
              VisualisationDataMap.ContainsKey(VisualisationType.Right)))
        {
            return;
        }

        VisualiseFraction(VisualisationDataMap[VisualisationType.Right].Fraction, VisualisationType.Right);
    }

    //OWN FUNCTIONS
    public void VisualiseFraction(Fraction fraction, VisualisationType visualisationType)
    {
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

        if (fraction is null)
        {
            UpdateBoard(Vector2.one);
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

        UpdateBoard(visualisationData.OffsetAndSpacing.columnsAndRows);
    }

    private void VisualiseRightCard(Fraction fraction)
    {
        if (VisualisationDataMap.ContainsKey(VisualisationType.Right))
        {
            VisualisationDataMap[VisualisationType.Right].VisualisationParent.transform.DestroyAllChildren();
            VisualisationDataMap.Remove(VisualisationType.Right);
        }

        if (fraction is null)
        {
            return; 
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

        if ((_operation == Operation.Add || _operation == Operation.Subtract) && 
            leftFraction.Denominator != fraction.Denominator)
        {
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
                if (!(leftFraction + fraction).IsBetween(0, 1))
                {
                    Debug.LogError(leftFraction + " + " + fraction + " = " + (leftFraction / fraction) +
                                   " is not between 0 and 1 -> not yet implemented");
                    return;
                }

                minIndex = leftFraction.Numerator;
                maxIndex = leftFraction.Numerator + fraction.Numerator;
                offsetAndSpacing = leftData.OffsetAndSpacing;
                packingCoordinates = leftData.PackingCoordinates;
                break;
            case Operation.Subtract:
                if (!(leftFraction - fraction).IsBetween(0, 1))
                {
                    Debug.LogError(leftFraction + " - " + fraction + " = " + (leftFraction / fraction) +
                                   " is not between 0 and 1 -> not yet implemented");
                    return;
                }

                minIndex = leftFraction.Numerator - fraction.Numerator;
                maxIndex = leftFraction.Numerator;
                offsetAndSpacing = leftData.OffsetAndSpacing;
                packingCoordinates = leftData.PackingCoordinates;
                break;
            case Operation.Multiply:
                if (!(leftFraction * fraction).IsBetween(0, 1))
                {
                    Debug.LogError(leftFraction + " * " + fraction + " = " + (leftFraction / fraction) +
                                   " is not between 0 and 1 -> not yet implemented");
                    return;
                }

                minIndex = 0;
                maxIndex = leftFraction.Numerator * fraction.Numerator;
                offsetAndSpacing = new OffsetAndSpacing(
                    numbersToPrimeFactors[leftFraction.Denominator * fraction.Denominator].factors,
                    boardSize);
                Debug.Log(leftFraction * fraction);
                packingCoordinates = CalculatePackingCoordinates(leftFraction * fraction);
                break;
            case Operation.Divide:
                if (!(leftFraction / fraction).IsBetween(0, 1))
                {
                    Debug.LogError(leftFraction + "/ " + fraction + " = " + (leftFraction / fraction) +
                                   " is not between 0 and 1 -> not yet implemented");
                    return;
                }

                minIndex = 0;
                maxIndex = leftFraction.Numerator * fraction.Denominator;
                offsetAndSpacing = new OffsetAndSpacing(
                    numbersToPrimeFactors[leftFraction.Denominator * fraction.Numerator].factors.ToArray(),
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

    private void UpdateBoard(Vector2 columnsAndRows)
    {
        boardRenderer.materials[0].SetFloat("_X_Amount", columnsAndRows.x);
        boardRenderer.materials[0].SetFloat("_Y_Amount", columnsAndRows.y);
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

                // Debug.Log("coord" + coordinates + ", rD" + recursionDepth + ", pF" + primeFactor + " index" + figureIndex);

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

    private void SpawnFigures(FractionVisualisationData visualisationData)
    {
        visualisationData.VisualisationParent.transform.DestroyAllChildren();
        for (int i = visualisationData.MinIndex; i < visualisationData.MaxIndex; i++)
        {
            if (visualisationData.PackingCoordinates.Length <= 12)
            {
                SpawnFigure(
                    visualisationData.VisualisationParent.transform,
                    figurePrefabs[visualisationData.PackingCoordinates.Length],
                    visualisationData.OffsetAndSpacing,
                    visualisationData.PackingCoordinates[i]);
            }
            else
            {
                GameObject newFigure = SpawnFigure(
                    visualisationData.VisualisationParent.transform,
                    figurePrefabs[0],
                    visualisationData.OffsetAndSpacing,
                    visualisationData.PackingCoordinates[i]);
                Vector3 spacing = visualisationData.OffsetAndSpacing.figureSpacing; 
                newFigure.transform.localScale = new Vector3(spacing.x * 0.75f / boardSize.x, 3, spacing.z * 0.75f / boardSize.z); 
            }
        }

        visualisationData.VisualisationParent.UpdateChildrenVisuals();
    }

    public GameObject SpawnFigure(Transform parent, GameObject figurePrefab, OffsetAndSpacing offsetAndSpacing,
        Vector2 coordinates)
    {
        GameObject figure = Instantiate(figurePrefab, parent.transform);
        figure.transform.localPosition = offsetAndSpacing.CalculatePosition(coordinates);
        //updating visuals is done afterwards via the parent
        return figure; 
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
    public Fraction debug_leftFraction;
    public Fraction debug_rightFraction;
    public Vector2 columnsAndRows;
    public Operation debug_operation;
    public int Editor_PrimeFactorGenerationMax;

    [ContextMenu("Editor_GeneratePrimeFactorsForNumbers")]
    public void Editor_GeneratePrimeFactorsForNumbers()
    {
        List<int> primeNumbers = new List<int>{2, 3};

        numbersToPrimeFactors = new Vector2Array[Editor_PrimeFactorGenerationMax];
        numbersToPrimeFactors[0] = new Vector2Array();
        numbersToPrimeFactors[1] = new Vector2Array(new Vector2[] { new(1, 1) });
        numbersToPrimeFactors[2] = new Vector2Array(new Vector2[] { new(1, 2) });
        numbersToPrimeFactors[3] = new Vector2Array(new Vector2[] { new(3, 1) });
        numbersToPrimeFactors[4] = new Vector2Array(new Vector2[] { new(1, 2), new (2, 1) });

        for (int i = 5; i < Editor_PrimeFactorGenerationMax; i++)
        {
            numbersToPrimeFactors[i] =
                new Vector2Array(FindPrimeFactorsForNumber(i, primeNumbers));
            if (numbersToPrimeFactors[i].factors.Length == 1)
            {
                primeNumbers.Add(i);
            }
        }

        List<Vector2> FindPrimeFactorsForNumber(int number, List<int> primeNumbersToCheck)
        {
            int dividedNumber = number;
            List<Vector2> primeFactors = new List<Vector2>();
            bool horizontal = true;
            //checking 2 seperately because it's the only one that starts vertically
            if (dividedNumber % 2 == 0)
            {
                dividedNumber /= 2;
                primeFactors.Add(new Vector2(1, 2));
                //horizontal = true; 
            }
            foreach (int prime in primeNumbersToCheck)
            {
                while (dividedNumber % prime == 0)
                {
                    dividedNumber /= prime;
                    primeFactors.Add(horizontal ? new Vector2(prime, 1) : new Vector2(1, prime));
                    horizontal = !horizontal;
                }
            }

            if (primeFactors.Count == 0)
            {
                primeFactors.Add(horizontal? new Vector2(number, 1) : new Vector2(1, number));
            }

            return primeFactors;
        }
    }    
    
    [ContextMenu("Debug_UpdateVisuals")]
    public void Debug_UpdateFraction()
    {
        transform.DestroyAllGrandChildren();
        _operation = debug_operation; 
        
        if (debug_leftFraction.Denominator == 0)
        {
            VisualiseFraction(null, VisualisationType.Left);
        }
        else
        {
            VisualiseFraction(debug_leftFraction, VisualisationType.Left);
        }
        
        if (debug_rightFraction.Denominator == 0)
        {
            VisualiseFraction(null, VisualisationType.Right);
        }
        else
        {
            VisualiseFraction(debug_rightFraction, VisualisationType.Right);
        }
    }

    [ContextMenu("Debug_PrintFractions")]
    public void Debug_PrintFractions()
    {
        foreach (FractionVisualisationData vd in VisualisationDataMap.Values)
        {
            Debug.Log(vd.Fraction);
        }
    }
}