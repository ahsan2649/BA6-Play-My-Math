using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using UnityEngine;

//WARNING: big parts of this visualisation system break down at 30

/* ObjectData Hierarchy: 
 * FractionVisualisationData
 *      -> FractionVisualisationStyle
 *      -> OffsetAndSpacing
 *      -> activeFigures 
 */

/* Figure Hierarchy
 * Layers -> Figures (via transform.child)
 * visData.spawnedFigures[Layer].[Column].[Row]
 */

namespace Programming.Fraction_Engine
{
    public class FractionVisualiser : MonoBehaviour
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
            
            public Vector3 CalculatePosition(Vector3Int coordinates)
            {
                return CalculatePosition(new Vector2Int(coordinates.y, coordinates.z)); 
            }
        }
        
        [Serializable]
        public class FractionVisualisationStyle
        {
            [SerializeField] public Material material;
            [SerializeField] public GameObject figureParent;
            [SerializeField] public string figureName;
            [SerializeField] public bool isFlipped;
            public void ApplyToObject(GameObject gameObject)
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
                gameObject.name = figureName;
                if (isFlipped)
                {
                    gameObject.transform.Rotate(Vector3.up, 180.0f);
                }
            }
        }

        [Serializable] //ZyKaLater This is only Serializable for debugging purposes
        private struct FractionVisualisationData
        {
            public FractionVisualisationData(Fraction fraction, 
                FractionVisualisationStyle visStyle, OffsetAndSpacing offsetAndSpacing, 
                List<Vector3Int> visualisedCoordinates)
            {
                this.fraction = fraction;
                this.VisStyle = visStyle;
                this.OffsetAndSpacing = offsetAndSpacing;
                VisualisedCoordinates = visualisedCoordinates;
                VisualisedFigures = Array.Empty<GameObject[][]>(); 
            }
            
            public Fraction fraction;
            public int Layers => (fraction.Numerator / fraction.Denominator) + 1;
            public int Columns => OffsetAndSpacing.ColumnsAndRows.x;
            public int Rows => OffsetAndSpacing.ColumnsAndRows.y; 
            
            public FractionVisualisationStyle VisStyle;
            public OffsetAndSpacing OffsetAndSpacing;
            public List<Vector3Int> VisualisedCoordinates;
            public GameObject[][][] VisualisedFigures;
            
            public GameObject FigureParent => VisStyle.figureParent; 

            public void ClearFigures(bool safeDelete = false)
            {
                if (!safeDelete && safeDelete) //ZyKa!
                {
                    foreach (Vector3Int coordinate in VisualisedCoordinates)
                    {
                        Destroy(VisualisedFigures[coordinate.x][coordinate.y][coordinate.z]);
                    }
                }
                else
                {
                    foreach (Transform figure in FigureParent.transform)
                    {
                        Destroy(figure.gameObject);
                    }
                }
                VisualisedFigures = Array.Empty<GameObject[][]>(); 
            }
        }
        
        #endregion
    
        #region Variables
        #region GlobalStatics
        private static readonly int XAmount = Shader.PropertyToID("_X_Amount");
        private static readonly int YAmount = Shader.PropertyToID("_Y_Amount");
        #endregion
        
        #region References
        [SerializeField] private FractionVisualisationStyle visStyle_Main; 
        [SerializeField] private FractionVisualisationStyle visStyle_Transparent;

        [SerializeField] private GameObject[] boardLayers;
        [SerializeField] private GameObject subLayerPrefab;
        [SerializeField] private GameObject topLayerPrefab; 
        
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

            VisualiseFraction(_visualisationDataMap[VisualisationType.Right].fraction, VisualisationType.Right);
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
            ResetVisType(VisualisationType.Left);
            if (fraction is null || fraction == new Fraction(0, 1))
            {
                UpdateBoards(Vector2Int.one);
                return;
            }

            OffsetAndSpacing offsetAndSpacing;
            List<Vector3Int> visualisedCoordinates; 
            
            if (!fraction.IsBetween(0, 1)  || fraction.Denominator > numbersToPrimeFactors.Length)
            {
                NotBetween01Error(fraction, out offsetAndSpacing, out visualisedCoordinates);
                return; 
            }
            offsetAndSpacing =
                new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize);
            visualisedCoordinates = CalculateVisualisedCoordinatesViaDivisors(fraction, 0, fraction.Numerator); 
            
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    fraction,
                    visStyle_Main,
                    offsetAndSpacing,
                    visualisedCoordinates
                );

            _visualisationDataMap.Add(VisualisationType.Left, visualisationData);

            SpawnFigures(visualisationData);
            UpdateBoards(visualisationData.OffsetAndSpacing.ColumnsAndRows);
        }

        private void VisualiseRightCard(Fraction rightFraction)
        {
            ResetVisType(VisualisationType.Right);
            if (rightFraction is null || rightFraction == new Fraction(0, 1))
            {
                return; 
            }

            //Deleting for replacement / removing of cards
            Operation operationCopy = _operation;

            //checking for leftCard
            FractionVisualisationData leftData;
            Fraction leftFraction;
            if (_visualisationDataMap.TryGetValue(VisualisationType.Left, out leftData))
            {
                leftFraction = leftData.fraction;
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

            OffsetAndSpacing offsetAndSpacing;
            List<Vector3Int> visualisedCoordinates;

            switch (operationCopy)
            {
                case Operation.Nop:
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[rightFraction.Denominator].factors,
                        boardSize);
                    visualisedCoordinates = CalculateVisualisedCoordinatesViaDivisors(rightFraction, 0, rightFraction.Numerator);
                    break;
                case Operation.Add:
                    if (rightFraction.Numerator < 0)
                    {
                        rightFraction = new Fraction(-rightFraction.Numerator, rightFraction.Denominator);
                        goto case Operation.Subtract; 
                    }
                    if (!(leftFraction + rightFraction).IsBetween(0, 1) || leftFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction + rightFraction, out offsetAndSpacing, out visualisedCoordinates);
                        break; 
                    }
                    offsetAndSpacing = leftData.OffsetAndSpacing;
                    visualisedCoordinates = CalculateVisualisedCoordinatesViaDivisors(rightFraction, leftFraction.Numerator, rightFraction.Numerator); //TODO Calculate visualised Coordinates
                    break;
                case Operation.Subtract:
                    if (rightFraction.Numerator < 0)
                    {
                        rightFraction = new Fraction(-rightFraction.Numerator, rightFraction.Denominator);
                        goto case Operation.Add; 
                    }
                    if (!(leftFraction - rightFraction).IsBetween(0, 1)  || leftFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction - rightFraction, out offsetAndSpacing, out visualisedCoordinates);
                        break; 
                    }
                    visualisedCoordinates = CalculateVisualisedCoordinatesViaDivisors(leftFraction - rightFraction, leftFraction.Numerator - rightFraction.Numerator, rightFraction.Numerator); 
                    offsetAndSpacing = leftData.OffsetAndSpacing;
                    break;
                case Operation.Multiply:
                    if (!(leftFraction * rightFraction).IsBetween(0, 1)  || leftFraction.Denominator*rightFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction * rightFraction, out offsetAndSpacing, out visualisedCoordinates);
                        break; 
                    }
                    offsetAndSpacing = new OffsetAndSpacing(
                        numbersToPrimeFactors[leftFraction.Denominator * rightFraction.Denominator].factors.ToArray(),
                        boardSize);
                    visualisedCoordinates = MultiplyVisualisedCoordinates(leftData, rightFraction); 
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
                    visStyle_Transparent,
                    offsetAndSpacing,
                    visualisedCoordinates
                );

            _visualisationDataMap.Add(VisualisationType.Right, visualisationData);
            SpawnFigures(visualisationData);
        }

        private void VisualiseModify(VisualisationType visType, ModifyType modifyType, int factor)
        {
            VisualisationType unmodifiedVisType = visType == VisualisationType.LeftModify
                ? VisualisationType.Left
                : VisualisationType.Right;
            ResetVisType(visType);
            if (modifyType == ModifyType.None || factor == 0)
            {
                return; 
            }
            if (!_visualisationDataMap.ContainsKey(unmodifiedVisType))
            {
                if (unmodifiedVisType == VisualisationType.Left)
                {
                    Debug.LogError("Can't visualise 'LeftModify' because 'Left' is not set");
                }
                if (unmodifiedVisType == VisualisationType.Right)
                {
                    Debug.LogError("Can't visualise 'LeftModify' because 'Left' is not set");
                }
                return; 
            }
            
            Fraction fraction = _visualisationDataMap[unmodifiedVisType].fraction; 
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    modifyType == ModifyType.Expand ? fraction.ExpandBy(factor) : fraction.SimplifyBy(factor),
                    visType == VisualisationType.LeftModify ? visStyle_Main : visStyle_Transparent,
                    new OffsetAndSpacing(numbersToPrimeFactors[modifyType == ModifyType.Expand ? fraction.Denominator * factor : fraction.Denominator / factor].factors, boardSize),
                    ModifyVisualisedCoordinates(_visualisationDataMap[unmodifiedVisType], fraction, modifyType)
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
        private void UpdateBoards(Vector2Int columnsAndRows, int layers = 1)
        {
            for (int i = boardLayers.Length -1; i >= 0; i--)
            {
                Destroy(boardLayers[i]); 
            }
            boardLayers = new GameObject[layers]; 
            for (int i = 0; i < layers-1; i++)
            {
                boardLayers[i] = Instantiate(subLayerPrefab, this.transform); 
            }

            boardLayers[layers - 1] = Instantiate(topLayerPrefab, this.transform); 
            
            foreach (GameObject gO in boardLayers)
            {
                Material boardMaterial = gO.GetComponent<MeshRenderer>().materials[0]; 
                boardMaterial.SetFloat(XAmount, columnsAndRows.x);
                boardMaterial.SetFloat(YAmount, columnsAndRows.y);
            }
        }

        private void ResetVisType(VisualisationType visType, bool searchSceneHierarchy = false)
        {
            if (_visualisationDataMap.ContainsKey(visType))
            {
                _visualisationDataMap[visType].ClearFigures();
                _visualisationDataMap.Remove(visType);
            }

            if (Application.isEditor || searchSceneHierarchy)
            {
                //TODO search through the hierarchy in check whether any child is left that shouldn't exist
            }
        }
        
        private void SpawnFigures(FractionVisualisationData visData)
        {
            visData.ClearFigures(true);
            
            //initialise VisualisedFigures Array (do I need this as an Array?)
            visData.VisualisedFigures = new GameObject[visData.Layers][][];
            for (int layer = 0; layer < visData.Layers; layer++) 
            {
                visData.VisualisedFigures[layer] = new GameObject[visData.OffsetAndSpacing.ColumnsAndRows.x][];
                for (int column = 0; column < visData.OffsetAndSpacing.ColumnsAndRows.x; column++)
                {
                    visData.VisualisedFigures[layer][column] = new GameObject[visData.OffsetAndSpacing.ColumnsAndRows.y]; 
                }
            }
            
            GameObject figurePrefab; 
            Vector3 localScale;
            if (visData.fraction.Denominator < figurePrefabs.Length)
            {
                figurePrefab = figurePrefabs[visData.fraction.Denominator];
                localScale = Vector3.one; 
            }
            else
            {
                figurePrefab = figurePrefabs[0];
                localScale = Vector3.one * (0.75f * boardSize.x/Mathf.Max(visData.Columns, visData.Rows)); 
            }
            
            foreach(Vector3Int coordinates in visData.VisualisedCoordinates)
            {
                GameObject newFigure = SpawnFigure(
                    visData.FigureParent.transform,
                    figurePrefab,
                    coordinates, visData);
                if (localScale != Vector3.one) //TodoLater: Remove the condition
                {
                    newFigure.transform.localScale = localScale; 
                }
            }
        }

        private GameObject SpawnFigure(Transform parent, GameObject figurePrefab,
            Vector3Int coordinates, FractionVisualisationData visData)
        {
            GameObject newFigure = Instantiate(figurePrefab, parent.transform);
            newFigure.transform.localPosition = visData.OffsetAndSpacing.CalculatePosition(coordinates);
            visData.VisStyle.ApplyToObject(newFigure);

            if (visData.VisualisedFigures[coordinates.x][coordinates.y][coordinates.z] != null)
            {
                Destroy(visData.VisualisedFigures[coordinates.x][coordinates.y][coordinates.z]);
                Debug.LogWarning("Spawning new Figure on a coordinate which already contains a figure: " + coordinates); 
            }
            visData.VisualisedFigures[coordinates.x][coordinates.y][coordinates.z] = newFigure;
            
            return newFigure; 
        }
        #endregion
        
        #region VisualisationCalculationFunctions

        private List<Vector3Int> CalculateVisualisedCoordinatesViaDivisors(Fraction fraction, int startingIndex, int figureCount)
        {
            List<Vector3Int> visualisedCoordinates = new List<Vector3Int>();
        
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

            //TodoLater: Optimise this so that it doesn't generate a whole list, just to then take a range of the List
            for (int layers = 0; layers <= fraction.Wholes; layers++)
            {
                for (int xND = 0; xND < notHorizontalDivisorProduct; xND++)
                {
                    for (int yND = 0; yND < notVerticalDivisorProduct; yND++)
                    {
                        for (int xDiv = 0; xDiv < horizontalDivisorProduct; xDiv++)
                        {
                            for (int yDiv = 0; yDiv < verticalDivisorProduct; yDiv++)
                            {
                                visualisedCoordinates.Add(new Vector3Int(
                                    layers,
                                    xDiv * notHorizontalDivisorProduct + xND,
                                    yDiv * notVerticalDivisorProduct + yND)
                                );
                            }
                        }
                    }
                }
            }
            visualisedCoordinates = visualisedCoordinates.GetRange(startingIndex, figureCount); 
            
            return visualisedCoordinates; 
        }
        
        private List<Vector3Int> CalculateVisualisedCoordinatesRecursively(Fraction fraction)
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

            List<Vector3Int> visualisedCoordinates = new List<Vector3Int>(); 
            for (int i = 0; i < packingCoordinates.Length; i++)
            {
                visualisedCoordinates.Add(new Vector3Int(0, packingCoordinates[i].x, packingCoordinates[i].y)); 
            }
            
            return visualisedCoordinates;

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
        
        private List<Vector3Int> ModifyVisualisedCoordinates(FractionVisualisationData oldVisualisationData,
            Fraction fraction, ModifyType modifyType)
        {
            return modifyType == ModifyType.Expand ? 
                MultiplyVisualisedCoordinates(oldVisualisationData, fraction) : 
                ShortenVisualisedCoordinates(oldVisualisationData, fraction.Denominator); 
        }
        
        private List<Vector3Int> ShortenVisualisedCoordinates(FractionVisualisationData oldVisData, int shortenFactor)
        {
            List<Vector3Int> newPackingCoordinates = new List<Vector3Int>();
            
            /*
            Vector2Int[] FactorsDifference = 
                CalcFactorDifferenceBetweenNumbers(oldVisData.fraction.Denominator, oldVisData.fraction.Denominator / shortenFactor);
            Vector2Int shortenVector = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current); 
            */
            
            for (int i = 0; i < oldVisData.VisualisedCoordinates.Count; i += shortenFactor)
            {
                newPackingCoordinates.Add(oldVisData.VisualisedCoordinates[i]); 
            }

            return newPackingCoordinates; 
        }
        
        private List<Vector3Int> MultiplyVisualisedCoordinates(FractionVisualisationData oldVisData, Fraction multiplicationFraction)
        {
            List<Vector3Int> newPackingCoordinates = new List<Vector3Int>();
            
            Fraction oldFraction = oldVisData.fraction; 
            Vector2Int[] FactorsDifference = 
                CalcFactorDifferenceBetweenNumbers(oldFraction.Denominator, oldFraction.Denominator*multiplicationFraction.Denominator);
            Vector2Int expandVector2 = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current);
            Vector3Int expandVector3 = new Vector3Int(0, expandVector2.x, expandVector2.y); 
            
            if (multiplicationFraction.Numerator > multiplicationFraction.Denominator)
            {
                newPackingCoordinates = CalculateVisualisedCoordinatesViaDivisors(oldFraction * multiplicationFraction, 0, oldFraction.Numerator * multiplicationFraction.Numerator); 
            }
            else
            {
                foreach (Vector3Int coordinate in oldVisData.VisualisedCoordinates)
                {
                    for (int yExpand = 0; yExpand < expandVector3.y; yExpand++)
                    {
                        for (int zExpand = 0; zExpand < expandVector3.z; zExpand++)
                        {
                            newPackingCoordinates.Add(new Vector3Int(
                                coordinate.x, 
                                coordinate.y * expandVector3.y + yExpand, 
                                coordinate.z * expandVector3.z + zExpand)
                            ); 
                        }
                    }
                }
            }
            return newPackingCoordinates; 
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
                        Vector2Int[] newFactor = 
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

#pragma warning disable CS8321 // Local function is declared but never used
            List<Vector2Int> FindPrimeFactorsForNumber(int number, List<int> primeNumbersToCheck)
#pragma warning restore CS8321 // Local function is declared but never used
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
                Debug.Log(vd.fraction);
            }
        }

        [ContextMenu("Debug_GetVisualisationDataList")]
        public void Debug_GetVisualisationDataAsList()
        {
            debug_VisualisationData = _visualisationDataMap.Values.ToArray(); 
        }
        
        void NotBetween01Error(Fraction fraction, out OffsetAndSpacing offsetAndSpacing, out List<Vector3Int> visualisedCoordinates)
        {
            visualisedCoordinates = new List<Vector3Int>(); 
            offsetAndSpacing = Mathf.Abs(fraction.Denominator) < numbersToPrimeFactors.Length ? 
                new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors.ToArray(), boardSize) :
                new OffsetAndSpacing(Array.Empty<Vector2Int>(), boardSize);
                
            Debug.LogWarning("Visualised Fraction: " + fraction + " is not between 0 and 1 or has a too big Denominator");
        }
        #endregion
    }
}

#region deprecated
    
#endregion