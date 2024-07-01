using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

//WARNING: big parts of this visualisation system break down at 30

namespace Programming.Operation_Board
{
    public class FractionVisualizer : MonoBehaviour
    {
        #region SubClasses
        public enum VisualisationType
        {
            None,
            Left,
            LeftModify,
            Right,
            RightModify
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
            public OffsetAndSpacing(Vector2Int columnsAndRows, Vector3 baseOffset, Vector3 figureSpacing)
            {
                ColumnsAndRows = columnsAndRows;
                BaseOffset = baseOffset;
                FigureSpacing = figureSpacing;
            }
            
            public OffsetAndSpacing(Vector2Int[] primeFactors, Vector3 boardSize)
            {
                ColumnsAndRows = Vector2Int.one;
                for (int i = 0; i < primeFactors.Length; i++)
                {
                    ColumnsAndRows *= primeFactors[i];
                }

                FigureSpacing = new Vector3(boardSize.x / ColumnsAndRows.x, 0, -boardSize.z / ColumnsAndRows.y);
                BaseOffset = new Vector3(-boardSize.x / 2 + FigureSpacing.x / 2, 0, boardSize.z / 2 + FigureSpacing.z / 2);
            }

            public int Denominator => ColumnsAndRows.x * ColumnsAndRows.y; 
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
        #endregion
    
        #region Variables
        #region GlobalStatics
        private static readonly int XAmount = Shader.PropertyToID("_X_Amount");
        private static readonly int YAmount = Shader.PropertyToID("_Y_Amount");
        #endregion
        
        #region References
        [SerializeField] private FractionVisualiserFigureParent leftVisualParent;
        [FormerlySerializedAs("leftChangeVisualParent")] [SerializeField] private FractionVisualiserFigureParent leftModifyVisualisationParent;

        [SerializeField] private FractionVisualiserFigureParent rightVisualParent;
        [FormerlySerializedAs("rightChangeVisualParent")] [SerializeField] private FractionVisualiserFigureParent rightModifyVisualParent;

        [SerializeField] private MeshRenderer boardRenderer;
        #endregion
        
        #region EditorVariables
        [SerializeField] private GameObject[] figurePrefabs;
        [SerializeField] private Vector2IntArray[] numbersToPrimeFactors;
        [SerializeField] private Vector3 boardSize;
        #endregion
        
        #region CodeVariables
        //CODE VARIABLES
        private Dictionary<VisualisationType, FractionVisualisationData> _visualisationDataMap =
            new Dictionary<VisualisationType, FractionVisualisationData>();
        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;
        #endregion
        #endregion
        
        #region MonoBehaviourFunctions
        private void Awake()
        {
            transform.DestroyAllGrandChildren();
        }
        #endregion
        
        #region AccessorFunctions
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
        public void VisualiseFraction(Fraction fraction, VisualisationType visType, int modifyFactor = 1, ModifyType modifyType = ModifyType.Expand)
        {
            switch (visType)
            {
                case VisualisationType.Left:
                    VisualiseLeftCard(fraction);
                    if (_visualisationDataMap.ContainsKey(VisualisationType.LeftModify))
                    {
                        VisualiseModify(VisualisationType.LeftModify, ModifyType.None, 0);
                    }
                    if (_visualisationDataMap.ContainsKey(VisualisationType.Right))
                    {
                        goto case VisualisationType.Right; 
                    }
                    break;
                case VisualisationType.Right:
                    VisualiseRightCard(fraction);
                    if (_visualisationDataMap.ContainsKey(VisualisationType.RightModify))
                    {
                        VisualiseModify(VisualisationType.RightModify, ModifyType.None , 0);
                    }
                    break;
                case VisualisationType.LeftModify:
                case VisualisationType.RightModify:
                    VisualiseModify(visType, modifyType, modifyFactor);
                    break;
                case VisualisationType.None:
                    throw new NotSupportedException();
            }
        }
        #endregion
        
        #region VisualisationManagerFunctions
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

            if (!fraction.IsBetween(0, 1))
            {
                NotBetween01(fraction);
                return; 
            }
            
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    fraction,
                    Enumerable.Range(0, fraction.Numerator).ToArray(), 
                    leftVisualParent,
                    new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize),
                    CalculatePackingCoordinatesViaDivisors(fraction)
                );

            _visualisationDataMap.Add(VisualisationType.Left, visualisationData);

            SpawnFigures(visualisationData);
            UpdateBoard(visualisationData.OffsetAndSpacing.ColumnsAndRows);
        }

        private void VisualiseRightCard(Fraction rightFraction)
        {
            if (_visualisationDataMap.ContainsKey(VisualisationType.Right))
            {
                _visualisationDataMap[VisualisationType.Right].VisualisationParent.transform.DestroyAllChildren();
                _visualisationDataMap.Remove(VisualisationType.Right);
            }

            if (rightFraction is null || rightFraction == new Fraction(0, 1))
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
                leftFraction.Denominator != rightFraction.Denominator)
            {
                operationCopy = Operation.Nop;
            }

            FractionVisualiserFigureParent visualParent;
            OffsetAndSpacing offsetAndSpacing;
            Vector2Int[] packingCoordinates;

            switch (operationCopy)
            {
                case Operation.Nop:
                    visualisedIndeces = Enumerable.Range(0, rightFraction.Numerator).ToArray(); 
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[rightFraction.Denominator].factors,
                        boardSize);
                    packingCoordinates = CalculatePackingCoordinatesRecursively(rightFraction);
                    break;
                case Operation.Add:
                    if (rightFraction.Numerator < 0)
                    {
                        rightFraction = new Fraction(-rightFraction.Numerator, rightFraction.Denominator);
                        goto case Operation.Subtract; 
                    }
                    if (!(leftFraction + rightFraction).IsBetween(0, 1))
                    {
                        NotBetween01(leftFraction + rightFraction);
                        return;
                    }
                    visualisedIndeces = Enumerable.Range(leftFraction.Numerator, rightFraction.Numerator).ToArray(); 
                    offsetAndSpacing = leftData.OffsetAndSpacing;
                    packingCoordinates = leftData.PackingCoordinates;
                    break;
                case Operation.Subtract:
                    if (rightFraction.Numerator < 0)
                    {
                        rightFraction = new Fraction(-rightFraction.Numerator, rightFraction.Denominator);
                        goto case Operation.Add; 
                    }
                    if (!(leftFraction - rightFraction).IsBetween(0, 1))
                    {
                        NotBetween01(leftFraction + rightFraction);
                        return;
                    }
                    visualisedIndeces = Enumerable.Range(leftFraction.Numerator - rightFraction.Numerator, rightFraction.Numerator).ToArray(); 
                    offsetAndSpacing = leftData.OffsetAndSpacing;
                    packingCoordinates = leftData.PackingCoordinates;
                    break;
                case Operation.Multiply:
                    if (!(leftFraction * rightFraction).IsBetween(0, 1))
                    {
                        NotBetween01(leftFraction + rightFraction);
                        return;
                    }
                    visualisedIndeces = MultiplyVisualisedIndeces(leftData, rightFraction); 
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[leftFraction.Denominator * rightFraction.Denominator].factors.ToArray(),
                        boardSize);
                    packingCoordinates = ExpandPackingCoordinates(leftData.PackingCoordinates, rightFraction.Denominator); 
                    break;
                case Operation.Divide:
                    rightFraction = new Fraction(rightFraction.Denominator, rightFraction.Numerator);
                    goto case Operation.Multiply; 
                default:
                    throw new NotImplementedException();
            }

            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    rightFraction,
                    visualisedIndeces, 
                    rightVisualParent,
                    offsetAndSpacing,
                    packingCoordinates
                );

            _visualisationDataMap.Add(VisualisationType.Right, visualisationData);
            SpawnFigures(visualisationData);
        }

        private void VisualiseModify(VisualisationType visType, ModifyType modifyType, int factor)
        {
            VisualisationType unmodifiedVisType = visType == VisualisationType.LeftModify
                ? VisualisationType.Left
                : VisualisationType.Right;
            if (_visualisationDataMap.ContainsKey(visType))
            {
                _visualisationDataMap[visType].VisualisationParent.transform.DestroyAllChildren();
                _visualisationDataMap.Remove(visType);
            }
            if (modifyType == ModifyType.None || factor == 0)
            {
                return; 
            }
            if (!_visualisationDataMap.ContainsKey(unmodifiedVisType))
            {
                if (unmodifiedVisType == VisualisationType.Left)
                {
                    Debug.LogError("Can't visualise 'LeftModify' because 'Left' is not set");
                    leftVisualParent.transform.DestroyAllChildren();
                }
                if (unmodifiedVisType == VisualisationType.Right)
                {
                    Debug.LogError("Can't visualise 'LeftModify' because 'Left' is not set");
                    rightVisualParent.transform.DestroyAllChildren();
                }
                return; 
            }
            
            Fraction fraction = _visualisationDataMap[unmodifiedVisType].Fraction; 
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    modifyType == ModifyType.Expand ? fraction.ExpandBy(factor) : fraction.SimplifyBy(factor),
                    ModifyVisualisedIndeces(_visualisationDataMap[unmodifiedVisType], factor, modifyType), 
                    visType == VisualisationType.LeftModify ? leftModifyVisualisationParent : rightModifyVisualParent,
                    new OffsetAndSpacing(numbersToPrimeFactors[modifyType == ModifyType.Expand ? fraction.Denominator * factor : fraction.Denominator / factor].factors, boardSize),
                    ModifyPackingCoordinates(_visualisationDataMap[unmodifiedVisType].PackingCoordinates, factor, modifyType)
                );

            _visualisationDataMap.Add(visType, visualisationData);

            SpawnFigures(visualisationData);
        }
        
        public void MoveFiguresAfterOperation()
        {
            throw new NotImplementedException();
        }
        #endregion
        
        #region InSceneVisualisation
        private void UpdateBoard(Vector2Int columnsAndRows)
        {
            boardRenderer.materials[0].SetFloat(XAmount, columnsAndRows.x);
            boardRenderer.materials[0].SetFloat(YAmount, columnsAndRows.y);
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
        #endregion
        
        #region VisualisationCalculationFunctions
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
                                yDiv * notVerticalDivisorProduct + yND);
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
                            coordinates + (primeFactor.x > primeFactor.y ? Vector2Int.right : Vector2Int.up) * i,
                            dividedDenominator, ref packingOrder);
                    }
                }
            }
        }

        private int[] ModifyVisualisedIndeces(FractionVisualisationData leftData, int factor, ModifyType modifyType)
        {
            return modifyType == ModifyType.Expand ? 
                ExpandVisualisedIndeces(leftData, factor) : 
                ShortenVisualisedIndeces(leftData, factor); 
        }
        
        private int[] ExpandVisualisedIndeces(FractionVisualisationData leftData, int expandFactor)
        {
            return MultiplyVisualisedIndeces(leftData, new Fraction(expandFactor, expandFactor)); 
        }
        
        private int[] MultiplyVisualisedIndeces(FractionVisualisationData leftData,
            Fraction rightFraction) 
        {
            Fraction leftFraction = leftData.Fraction; 
            int[] visualisedIndeces = new int[leftFraction.Numerator * rightFraction.Numerator];

            int newIndex = 0; 
            for (int oldIndex = 0; oldIndex < leftFraction.Numerator; oldIndex++)
            {
                for (int added = 0; added < rightFraction.Numerator; added++)
                {
                    visualisedIndeces[newIndex] =
                        leftData.VisualisedIndeces[oldIndex] * rightFraction.Denominator;
                    visualisedIndeces[newIndex] += (added / rightFraction.Denominator) * leftFraction.Numerator * rightFraction.Denominator; 
                    visualisedIndeces[newIndex] += added%rightFraction.Denominator; 
                    newIndex++; 
                }
            }
            
            return visualisedIndeces; 
        }
        
        private int[] ShortenVisualisedIndeces(FractionVisualisationData leftData,
            int shortenFactor) 
        {
            Fraction leftFraction = leftData.Fraction; 
            int[] visualisedIndeces = new int[leftFraction.Numerator / shortenFactor];

            for (int i = 0; i < visualisedIndeces.Length; i++)
            {
                visualisedIndeces[i] = leftData.VisualisedIndeces[i*shortenFactor]; 
            }
            
            return visualisedIndeces; 
        }
        
        private Vector2Int[] ShortenPackingCoordinates(Vector2Int[] packingCoordinates, int shortenFactor)
        {
            Vector2Int[] FactorsDifference = 
                CalcFactorDifferenceBetweenNumbers(packingCoordinates.Length  * shortenFactor, packingCoordinates.Length);
            Vector2Int shortenVector = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current); 
            
            return ShortenPackingCoordinates(packingCoordinates, shortenVector); 
        }

        private Vector2Int[] ModifyPackingCoordinates(Vector2Int[] packingCoordinates, int factor, ModifyType modifyType)
        {
            return modifyType == ModifyType.Expand ? 
                ExpandPackingCoordinates(packingCoordinates, factor) : 
                ShortenPackingCoordinates(packingCoordinates, factor); 
        }
        
        private Vector2Int[] ShortenPackingCoordinates(Vector2Int[] packingCoordinates, Vector2Int shortenVector)
        {
            Vector2Int[] newPackingCoordinates = new Vector2Int[packingCoordinates.Length / (shortenVector.x * shortenVector.y)];
            int shortenFactor = shortenVector.x * shortenVector.y; 
            
            int oldIndex = 0; 
            for (int newIndex = 0; newIndex < newPackingCoordinates.Length; newIndex++)
            {
                newPackingCoordinates[newIndex] = packingCoordinates[newIndex * shortenFactor];
                newPackingCoordinates[newIndex].x /= shortenVector.x; 
                newPackingCoordinates[newIndex].y /= shortenVector.y; 
            }

            return newPackingCoordinates; 
        }
        
        private Vector2Int[] ExpandPackingCoordinates(Vector2Int[] packingCoordinates, int expandFactor)
        {
            Vector2Int[] FactorsDifference = 
                CalcFactorDifferenceBetweenNumbers(packingCoordinates.Length*expandFactor, packingCoordinates.Length);
            Vector2Int expandVector = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current); 
            
            return ExpandPackingCoordinates(packingCoordinates, expandVector); 
        }
        
        private Vector2Int[] CalcFactorDifferenceBetweenNumbers(int number, int divisorOfNumber) //only works for if the second number actually divides the first one
        {
            List<Vector2Int> factorsDifference = new List<Vector2Int>(); 
            Vector2Int[] numberFactors = numbersToPrimeFactors[number].factors; 
            Vector2Int[] divisorFactors = numbersToPrimeFactors[divisorOfNumber].factors;

            int divisorIndex = 0, numberIndex = 0; 
            while(divisorIndex < divisorFactors.Length) //Warning this only works for numbers <30
            {
                if (divisorFactors[numberIndex] == divisorFactors[divisorIndex])
                {
                    numberIndex++;
                    divisorIndex++; 
                }
                else
                {
                    factorsDifference.Add(numberFactors[numberIndex]); 
                    numberIndex++; 
                }
            }
            while (numberIndex < numberFactors.Length)
            {
                factorsDifference.Add(numberFactors[numberIndex]); 
                numberIndex++; 
            }

            return factorsDifference.ToArray(); 
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
        #endregion
        
        #region EditorHelpers
        public int Editor_PrimeFactorGenerationMax;
        
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
    
        #endregion

        #region Debug
        //DEBUG & EDITOR HELPERS
        public Vector2Int[] debug_divisorOrder;
        public Fraction debug_leftFraction;
        public ModifyType debug_leftModifyType; 
        public int debug_leftModifyFactor; 
        
        public Operation debug_operation;
        
        public Fraction debug_rightFraction;
        public ModifyType debug_rightModifyType; 
        public int debug_rightModifyFactor; 
        
        [SerializeField] private FractionVisualisationData[] debug_VisualisationData; //this is used for checking where smth went wrong
        
        [ContextMenu("Debug_UpdateVisuals")]
        public void Debug_UpdateVisuals()
        {
            transform.DestroyAllGrandChildren();
            _operation = debug_operation; 
            VisualiseFraction(debug_leftFraction, VisualisationType.Left);
            VisualiseFraction(debug_rightFraction, VisualisationType.Right);
            VisualiseModify(VisualisationType.LeftModify, debug_leftModifyType, debug_leftModifyFactor);
            VisualiseModify(VisualisationType.RightModify, debug_rightModifyType, debug_rightModifyFactor);
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

        public void NotBetween01(Fraction fraction)
        {
            UpdateBoard(numbersToPrimeFactors[fraction.Denominator].factors.Aggregate(Vector2Int.one, (product, factor) => product * factor));
            Debug.LogError("Visualised Fraction: " + fraction + " is not between 0 and 1 -> not implemented");
        }
        #endregion
    }
}