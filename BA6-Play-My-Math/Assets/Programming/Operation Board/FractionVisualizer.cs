using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEditor;
using UnityEngine;

//WARNING: big parts of this visualisation system break down at 30

namespace Programming.Operation_Board
{
    public class FractionVisualizer : MonoBehaviour
    {
        public enum VisualisationType
        {
            None,
            Left,
            LeftChange,
            Right,
            RightChange
        }

        [Serializable]
        public struct Vector2IntArray
        {
            public Vector2IntArray(Vector2Int[] factors)
            {
                this.factors = factors;
            }

            public Vector2IntArray(List<Vector2Int> factors)
            {
                this.factors = factors.ToArray();
            }

            public Vector2Int[] factors;
        }

        public struct OffsetAndSpacing
        {
            public OffsetAndSpacing(Vector2Int[] primeFactors, Vector3 boardSize)
            {
                ColumnsAndRows = Vector2Int.one;
                for (int i = 0; i < primeFactors.Length; i++)
                {
                    ColumnsAndRows *= primeFactors[i];
                }

                FigureSpacing = new Vector3(boardSize.x / ColumnsAndRows.x, 0, boardSize.z / ColumnsAndRows.y);
                BaseOffset = new Vector3(-boardSize.x / 2 + FigureSpacing.x / 2, 0, boardSize.z / 2 - FigureSpacing.z / 2);
            }

            public Vector2Int ColumnsAndRows;
            public Vector3 BaseOffset;
            public Vector3 FigureSpacing;

            public Vector3 CalculatePosition(Vector2Int coordinates)
            {
                return BaseOffset + new Vector3(coordinates.x * FigureSpacing.x, 0, coordinates.y * FigureSpacing.z);
            }
        }

        [System.Serializable] //ZyKaLater This is only Serializable for debugging purposes
        private struct FractionVisualisationData
        {
            public FractionVisualisationData(Fraction fraction, int[] visualisedIndeces,
                FractionVisualiserFigureParent parent, OffsetAndSpacing offsetAndSpacing, Vector2Int[] packingCoordinates)
            {
                Fraction = fraction;
                VisualisedIndeces = visualisedIndeces; 
                VisualisationParent = parent;
                OffsetAndSpacing = offsetAndSpacing;
                PackingCoordinates = packingCoordinates;
            }

            public Fraction Fraction;
            public int[] VisualisedIndeces; 
            public FractionVisualiserFigureParent VisualisationParent;
            public OffsetAndSpacing OffsetAndSpacing;
            public Vector2Int[] PackingCoordinates;
        }
        
        //(GLOBAL) STATICS
        private static readonly int XAmount = Shader.PropertyToID("_X_Amount");
        private static readonly int YAmount = Shader.PropertyToID("_Y_Amount");

        //REFERENCES
        [SerializeField] private FractionVisualiserFigureParent leftVisualParent;
        [SerializeField] private FractionVisualiserFigureParent leftChangeVisualParent;

        [SerializeField] private FractionVisualiserFigureParent rightVisualParent;
        [SerializeField] private FractionVisualiserFigureParent rightChangeVisualParent;

        [SerializeField] private MeshRenderer boardRenderer;

        //EDITOR VARIABLES
        [SerializeField] private GameObject[] figurePrefabs;
        [SerializeField] private Vector2IntArray[] numbersToPrimeFactors;
        [SerializeField] private Vector3 boardSize;

        //CODE VARIABLES
        private Dictionary<VisualisationType, FractionVisualisationData> _visualisationDataMap =
            new Dictionary<VisualisationType, FractionVisualisationData>();

        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;

        //MONOBEHAVIOUR FUNCTIONS
        private void Awake()
        {
            transform.DestroyAllGrandChildren();
        }

        public void VisualiseOperation(Operation operation)
        {
            _operation = operation;

            if (!(_visualisationDataMap.ContainsKey(VisualisationType.Left) &&
                  _visualisationDataMap.ContainsKey(VisualisationType.Right)))
            {
                return;
            }

            VisualiseFraction(_visualisationDataMap[VisualisationType.Right].Fraction, VisualisationType.Right);
        }

        //OWN FUNCTIONS
        public void VisualiseFraction(Fraction fraction, VisualisationType visualisationType)
        {
            switch (visualisationType)
            {
                case VisualisationType.Left:
                    VisualiseLeftCard(fraction);
                    if (_visualisationDataMap.ContainsKey(VisualisationType.Right))
                    {
                        VisualiseRightCard(_visualisationDataMap[VisualisationType.Right].Fraction); 
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
                case VisualisationType.None:
                    throw new NotSupportedException();
                    break;
            }
        }

        private void VisualiseLeftCard(Fraction fraction)
        {
            if (_visualisationDataMap.ContainsKey(VisualisationType.Left))
            {
                _visualisationDataMap[VisualisationType.Left].VisualisationParent.transform.DestroyAllChildren();
                _visualisationDataMap.Remove(VisualisationType.Left);
            }

            if (fraction is null || fraction == new Fraction(0, 1))
            {
                UpdateBoard(Vector2Int.one);
                return;
            }
            
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    fraction,
                    Enumerable.Range(0, fraction.Numerator).ToArray(), 
                    leftVisualParent,
                    new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize),
                    CalculatePackingCoordinatesViaDivisors(fraction) //TODO change all CalculatePackingCoordinates to use the faster & more precise algorithms
                );

            _visualisationDataMap.Add(VisualisationType.Left, visualisationData);

            SpawnFigures(visualisationData);

            UpdateBoard(visualisationData.OffsetAndSpacing.ColumnsAndRows);
        }

        private void VisualiseRightCard(Fraction fraction)
        {
            if (_visualisationDataMap.ContainsKey(VisualisationType.Right))
            {
                _visualisationDataMap[VisualisationType.Right].VisualisationParent.transform.DestroyAllChildren();
                _visualisationDataMap.Remove(VisualisationType.Right);
            }

            if (fraction is null || fraction == new Fraction(0, 1))
            {
                return; 
            }

            //Deleting for replacement / removing of cards
            Operation operationCopy = _operation;
            int[] visualisedIndeces; 

            //checking for leftCard
            FractionVisualisationData leftData;
            Fraction leftFraction = null;
            if (_visualisationDataMap.TryGetValue(VisualisationType.Left, out leftData))
            {
                leftFraction = leftData.Fraction;
            }
            else
            {
                leftFraction = new Fraction(0, 1); //needs a value to avoid compilation error
                operationCopy = Operation.Nop;
            }

            if ((_operation == Operation.Add || _operation == Operation.Subtract) && 
                leftFraction.Denominator != fraction.Denominator)
            {
                operationCopy = Operation.Nop;
            }

            FractionVisualiserFigureParent visualParent;
            OffsetAndSpacing offsetAndSpacing;
            Vector2Int[] packingCoordinates;

            switch (operationCopy)
            {
                case Operation.Nop:
                    visualisedIndeces = Enumerable.Range(0, fraction.Numerator).ToArray(); 
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[fraction.Denominator].factors,
                        boardSize);
                    packingCoordinates = CalculatePackingCoordinatesRecursively(fraction);
                    break;
                case Operation.Add:
                    if (!(leftFraction + fraction).IsBetween(0, 1))
                    {
                        Debug.LogError(leftFraction + " + " + fraction + " = " + (leftFraction / fraction) +
                                       " is not between 0 and 1 -> not yet implemented");
                        return;
                    }
                    visualisedIndeces = Enumerable.Range(leftFraction.Numerator, fraction.Numerator).ToArray(); 
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
                    visualisedIndeces = Enumerable.Range(leftFraction.Numerator - fraction.Numerator, fraction.Numerator).ToArray(); 
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
                    visualisedIndeces = ExpandVisualisedIndeces(leftData, fraction); 
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[leftFraction.Denominator * fraction.Denominator].factors.ToArray(),
                        boardSize);
                    packingCoordinates = ExpandPackingCoordinates(leftData.PackingCoordinates, fraction.Denominator); 
                    break;
                case Operation.Divide: //TODO: adjust this similar to Multiply
                    if (!(leftFraction / fraction).IsBetween(0, 1))
                    {
                        Debug.LogError(leftFraction + "/ " + fraction + " = " + (leftFraction / fraction) +
                                       " is not between 0 and 1 -> not yet implemented");
                        return;
                    }
                    visualisedIndeces = new int[leftFraction.Numerator * fraction.Denominator];
                    for (int oldIndex = 0; oldIndex < leftFraction.Numerator; oldIndex++)
                    {
                        for (int added = 0; added < fraction.Denominator; added++)
                        {
                            visualisedIndeces[oldIndex + added] =
                                leftData.VisualisedIndeces[oldIndex] * fraction.Denominator + added; 
                        }
                    }
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[leftFraction.Denominator * fraction.Numerator].factors.ToArray(),
                        boardSize);
                    packingCoordinates = CalculatePackingCoordinatesRecursively(leftFraction / fraction);
                    break;
                default:
                    throw new NotImplementedException();
            }

            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    fraction,
                    visualisedIndeces, 
                    rightVisualParent,
                    offsetAndSpacing,
                    packingCoordinates
                );

            _visualisationDataMap.Add(VisualisationType.Right, visualisationData);
            SpawnFigures(visualisationData);
        }

        private void UpdateBoard(Vector2Int columnsAndRows)
        {
            boardRenderer.materials[0].SetFloat(XAmount, columnsAndRows.x);
            boardRenderer.materials[0].SetFloat(YAmount, columnsAndRows.y);
        }

        //TODO: Expand, Shorten & Operator

        private Vector2Int[] CalculatePackingCoordinatesViaDivisors(Fraction fraction)
        {
            Vector2Int[] packingCoordinates = new Vector2Int[fraction.Denominator];
        
            CheckNumberDivisors(numbersToPrimeFactors[fraction.Denominator].factors,
                fraction.Numerator, out List<Vector2Int> numeratorDivisors, out List<Vector2Int> notDivisors);

            int horizontalDivisorProduct = 1;
            int verticalDivisorProduct = 1;
            int notHorizontalDivisorProduct = 1;
            int notVerticalDivisorProduct = 1;

            foreach (Vector2Int divisor in numeratorDivisors)
            {
                if (divisor.x > divisor.y)
                {
                    horizontalDivisorProduct *= divisor.x; 
                }
                else
                {
                    verticalDivisorProduct *= divisor.y; 
                }
            }

            foreach (Vector2Int notDivisor in notDivisors)
            {
                if (notDivisor.x > notDivisor.y)
                {
                    notHorizontalDivisorProduct *= notDivisor.x; 
                }
                else
                {
                    notVerticalDivisorProduct *= notDivisor.y; 
                }
            }

            int index = 0; 
            for (int xDiv = 0; xDiv < horizontalDivisorProduct; xDiv++)
            {
                for (int yDiv = 0; yDiv < verticalDivisorProduct; yDiv++)
                {
                    for (int xND = 0; xND < notHorizontalDivisorProduct; xND++)
                    {
                        for (int yND = 0; yND < notVerticalDivisorProduct; yND++)
                        {
                            packingCoordinates[index] = new Vector2Int(
                                xDiv * notHorizontalDivisorProduct + xND,
                                -(yDiv * notVerticalDivisorProduct + yND));
                            index++; 
                        }
                    }
                }
            }

            return packingCoordinates; 
        }
    
        private Vector2Int[] CalculatePackingCoordinatesRecursively(Fraction fraction)
        {
            Vector2Int[] packingCoordinates = new Vector2Int[fraction.Denominator];
            CheckNumberDivisors(numbersToPrimeFactors[fraction.Denominator].factors,
                fraction.Numerator, out List<Vector2Int> numeratorDivisors, out List<Vector2Int> notDivisors); 
            Vector2Int[] divisorOrder = notDivisors.Concat(numeratorDivisors).ToArray();

            if (debug_divisorOrder.Length > 0)
            {
                divisorOrder = debug_divisorOrder;
            }

            RecursiveCalculateFigurePositions(divisorOrder, 0, 0, Vector2Int.zero, fraction.Denominator,
                ref packingCoordinates);

            return packingCoordinates;

            void RecursiveCalculateFigurePositions(Vector2Int[] divisorOrder, int recursionDepth,
                int figureIndex, Vector2Int coordinates,
                int dividedDenominator, ref Vector2Int[] packingOrder)
            {
                if (recursionDepth >= divisorOrder.Length)
                {
                    packingOrder[figureIndex] = coordinates;
                }
                else
                {
                    Vector2Int primeFactor = divisorOrder[recursionDepth];

                    // Debug.Log("coord" + coordinates + ", rD" + recursionDepth + ", pF" + primeFactor + " index" + figureIndex);

                    coordinates *= primeFactor;
                    dividedDenominator /= Mathf.Max(primeFactor.x, primeFactor.y);

                    for (int i = 0; i < Mathf.Max(primeFactor.x, primeFactor.y); i++)
                    {
                        RecursiveCalculateFigurePositions(divisorOrder, recursionDepth + 1,
                            figureIndex + dividedDenominator * i,
                            coordinates + (primeFactor.x > primeFactor.y ? Vector2Int.right : Vector2Int.down) * i,
                            dividedDenominator, ref packingOrder);
                    }
                }
            }
        }

        private int[] ExpandVisualisedIndeces(FractionVisualisationData leftData,
            Fraction rightFraction) //TODO make this usable for multiplication with any number & for division
        {
            Fraction leftFraction = leftData.Fraction; 
            int[] visualisedIndeces = new int[leftFraction.Numerator * rightFraction.Numerator];

            if (rightFraction <= 1)
            {
                int newIndex = 0; 
                for (int oldIndex = 0; oldIndex < leftFraction.Numerator; oldIndex++)
                {
                    for (int added = 0; added < rightFraction.Numerator; added++)
                    {
                        visualisedIndeces[newIndex] =
                            leftData.VisualisedIndeces[oldIndex] * rightFraction.Denominator;
                        visualisedIndeces[newIndex] += (added / rightFraction.Denominator) * rightFraction.Denominator; 
                        visualisedIndeces[newIndex] += added%rightFraction.Denominator; 
                        newIndex++; 
                    }
                }
            }
            else
            {
                visualisedIndeces = Enumerable.Range(0, leftFraction.Numerator * rightFraction.Numerator).ToArray();
            }
            
            return visualisedIndeces; 
        }
        
        private Vector2Int[] ExpandPackingCoordinates(Vector2Int[] packingCoordinates, int expandFactor)
        {
            Vector2Int expandVector = Vector2Int.one;
            Vector2Int[] oldFactors = numbersToPrimeFactors[packingCoordinates.Length].factors; 
            Vector2Int[] newFactors = numbersToPrimeFactors[packingCoordinates.Length * expandFactor].factors;

            int oldIndex = 0, newIndex = 0; 
            while(oldIndex < oldFactors.Length) //Warning this only works for numbers <30
            {
                if (newFactors[newIndex] == oldFactors[oldIndex])
                {
                    newIndex++;
                    oldIndex++; 
                }
                else
                {
                    expandVector *= newFactors[newIndex];
                    newIndex++; 
                }
            }
            while (newIndex < newFactors.Length)
            {
                expandVector *= newFactors[newIndex];
                newIndex++; 
            }
            
            /* nice try but this has problem with repeated factors
            Vector2Int expandVector =
                numbersToPrimeFactors[packingCoordinates.Length].factors.Aggregate(Vector2Int.one, 
                    (v2, factor) =>
                        numbersToPrimeFactors[packingCoordinates.Length * expandFactor].factors.Contains(factor)  ? v2 : v2*factor);
                        */
            return ExpandPackingCoordinates(packingCoordinates, expandVector); 
        }
        
        private Vector2Int[] ExpandPackingCoordinates(Vector2Int[] packingCoordinates, Vector2Int expandVector)
        {
            Vector2Int[] newPackingCoordinates = new Vector2Int[packingCoordinates.Length * expandVector.x * expandVector.y];

            int newIndex = 0; 
            for (int ogIndex = 0; ogIndex < packingCoordinates.Length; ogIndex++)
            { 
                for (int xExpand = 0; xExpand < expandVector.x; xExpand++)
                {
                    for (int yExpand = 0; yExpand < expandVector.y; yExpand++)
                    {
                        newPackingCoordinates[newIndex] =
                            packingCoordinates[ogIndex] * expandVector + new Vector2Int(xExpand, -yExpand);
                        newIndex++; 
                    }
                }
            }

            return newPackingCoordinates; 
        }
        
        private void SpawnFigures(FractionVisualisationData visualisationData)
        {
            visualisationData.VisualisationParent.transform.DestroyAllChildren();
            foreach(int index in visualisationData.VisualisedIndeces)
            {
                if (visualisationData.PackingCoordinates.Length <= 12)
                {
                    SpawnFigure(
                        visualisationData.VisualisationParent.transform,
                        figurePrefabs[visualisationData.PackingCoordinates.Length],
                        visualisationData.OffsetAndSpacing,
                        visualisationData.PackingCoordinates[index]);
                }
                else
                {
                    GameObject newFigure = SpawnFigure(
                        visualisationData.VisualisationParent.transform,
                        figurePrefabs[0],
                        visualisationData.OffsetAndSpacing,
                        visualisationData.PackingCoordinates[index]);
                    Vector3 spacing = visualisationData.OffsetAndSpacing.FigureSpacing; 
                    newFigure.transform.localScale = new Vector3(spacing.x * 0.75f / boardSize.x, 3, spacing.z * 0.75f / boardSize.z); 
                }
            }

            visualisationData.VisualisationParent.UpdateChildrenVisuals();
        }

        public GameObject SpawnFigure(Transform parent, GameObject figurePrefab, OffsetAndSpacing offsetAndSpacing,
            Vector2Int coordinates)
        {
            GameObject figure = Instantiate(figurePrefab, parent.transform);
            figure.transform.localPosition = offsetAndSpacing.CalculatePosition(coordinates);
            //updating visuals is done afterwards via the parent
            return figure; 
        }
    
        public void CheckNumberDivisors(Vector2Int[] toCheck, int number, out List<Vector2Int> divisors, out List<Vector2Int> notDivisors)
        {
            divisors = new List<Vector2Int>();
            notDivisors = new List<Vector2Int>(); 

            foreach (Vector2Int primeFactor in toCheck)
            {
                int factor = Mathf.Max(primeFactor.x, primeFactor.y);
                if (number % factor == 0)
                {
                    divisors.Add(primeFactor);
                    number /= factor;
                }
                else
                {
                    notDivisors.Add(primeFactor);
                }
            }
        }

        public void MoveFiguresAfterOperation()
        {
            throw new NotImplementedException();
        }

        //DEBUG & EDITOR HELPERS
        public Vector2Int[] debug_divisorOrder;
        public Fraction debug_leftFraction;
        public Fraction debug_rightFraction;
        public Vector2Int columnsAndRows;
        public Operation debug_operation;
        public int Editor_PrimeFactorGenerationMax;
        [SerializeField] private FractionVisualisationData[] debug_VisualisationData; 

        [ContextMenu("Editor_GeneratePrimeFactorsForNumbers")]
        public void Editor_GeneratePrimeFactorsForNumbers() //TODO: this doesn't work for all numbers e.g. the 3 in 12 is horizontal, but the 3 in 24 is vertical
        {
            List<int> primeNumbers = new List<int>{2, 3};

            numbersToPrimeFactors = new Vector2IntArray[Editor_PrimeFactorGenerationMax+1];
            numbersToPrimeFactors[0] = new Vector2IntArray();
            numbersToPrimeFactors[1] = new Vector2IntArray(new Vector2Int[] { new(1, 1) });
            numbersToPrimeFactors[2] = new Vector2IntArray(new Vector2Int[] { new(1, 2) });
            numbersToPrimeFactors[3] = new Vector2IntArray(new Vector2Int[] { new(3, 1) });
            numbersToPrimeFactors[4] = new Vector2IntArray(new Vector2Int[] { new(1, 2), new (2, 1) });

            for (int number = 5; number <= Editor_PrimeFactorGenerationMax; number++)
            {
                foreach (int prime in primeNumbers)
                {
                    if (number % prime == 0)
                    {
                        Vector2Int[] newFactor = new Vector2Int[1]
                            {numbersToPrimeFactors[number / prime].factors[0].x != 1
                            ? new Vector2Int(1, prime)
                            : new Vector2Int(prime, 1)}; 
                        numbersToPrimeFactors[number].factors = newFactor.Concat(numbersToPrimeFactors[number / prime].factors).ToArray();
                        break; 
                    }
                }

                if (numbersToPrimeFactors[number].factors == null)
                {
                    numbersToPrimeFactors[number].factors = new Vector2Int[] { new Vector2Int(number, 1) }; 
                    primeNumbers.Add(number);
                }
            }

            List<Vector2Int> FindPrimeFactorsForNumber(int number, List<int> primeNumbersToCheck)
            {
                int dividedNumber = number;
                List<Vector2Int> primeFactors = new List<Vector2Int>();
                bool horizontal = true;
                //checking 2 seperately because it's the only one that starts vertically
                if (dividedNumber % 2 == 0)
                {
                    dividedNumber /= 2;
                    primeFactors.Add(new Vector2Int(1, 2));
                    //horizontal = true; 
                }
                foreach (int prime in primeNumbersToCheck)
                {
                    while (dividedNumber % prime == 0)
                    {
                        dividedNumber /= prime;
                        primeFactors.Add(horizontal ? new Vector2Int(prime, 1) : new Vector2Int(1, prime));
                        horizontal = !horizontal;
                    }
                }

                if (primeFactors.Count == 0)
                {
                    primeFactors.Add(horizontal? new Vector2Int(number, 1) : new Vector2Int(1, number));
                }

                return primeFactors;
            }
        }    
    
        [ContextMenu("Debug_UpdateVisuals")]
        public void Debug_UpdateFraction()
        {
            transform.DestroyAllGrandChildren();
            _operation = debug_operation; 
            VisualiseFraction(debug_leftFraction, VisualisationType.Left);
            VisualiseFraction(debug_rightFraction, VisualisationType.Right);
        }

        [ContextMenu("Debug_PrintFractions")]
        public void Debug_PrintFractions()
        {
            foreach (FractionVisualisationData vd in _visualisationDataMap.Values)
            {
                Debug.Log(vd.Fraction);
            }
        }

        [ContextMenu("Debug_GetVisualisationDataList")]
        public void Debug_GetVisualisationDataAsList()
        {
            debug_VisualisationData = _visualisationDataMap.Values.ToArray(); 
        }
    }
}