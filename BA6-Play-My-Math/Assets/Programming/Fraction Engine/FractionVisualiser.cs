using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Programming.ExtensionMethods;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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

/* Coordinates & Dimensions: x->Layers, y->Columns, z->Rows
 * BoardSize: x->Width, y->Height, z->Depth
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
            public OffsetAndSpacing(Vector3Int dimensions, Vector3 baseOffset, Vector3 figureSpacing)
            {
                Dimensions = dimensions; 
                BaseOffset = baseOffset;
                FigureSpacing = figureSpacing;
            }
            
            public OffsetAndSpacing(Vector3Int dimensions, Vector3 boardSize)
            {
                Dimensions = dimensions; 
                FigureSpacing = new Vector3(boardSize.x / dimensions.y, boardSize.y, -boardSize.z / dimensions.z);
                BaseOffset = new Vector3(-boardSize.x / 2 + FigureSpacing.x / 2, 0.25f, boardSize.z / 2 + FigureSpacing.z / 2);
            }

            public static OffsetAndSpacing CreateOffsetAndSpacing(Vector3Int dimensions, Vector3 boardSize)
            {
                return new OffsetAndSpacing(dimensions, boardSize); 
            }

            public int Denominator => Dimensions.y * Dimensions.z; 
            public Vector3Int Dimensions;
            public int Layers => Dimensions.x; 
            public int Columns => Dimensions.y; 
            public int Rows => Dimensions.z; 
            
            public Vector3 BaseOffset;
            public Vector3 FigureSpacing;

            public Vector3 CalculatePosition(Vector3Int coordinates)
            {
                //coordinates.x->layer, coordinates.y->column, coordiantes.z->row
                return BaseOffset + new Vector3(coordinates.y * FigureSpacing.x, coordinates.x * FigureSpacing.y, coordinates.z * FigureSpacing.z);
            }
        }
        
        [Serializable]
        public class FractionVisualisationStyle
        {
            [SerializeField] public Material material;
            [SerializeField] public GameObject figureParent;
            [SerializeField] public string figureName;
            [SerializeField] public bool isFlipped;
            [SerializeField] public Vector3 scale = Vector3.one * 0.9f;
            [SerializeField] public float yOffset; 
            
            public void ApplyToObject(GameObject gameObject)
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
                gameObject.name = figureName;
                gameObject.transform.localScale = scale;
                gameObject.transform.localPosition += Vector3.up * yOffset; 
                
                if (isFlipped)
                {
                    gameObject.transform.Rotate(Vector3.up, 180.0f);
                }
            }
        }

        private class FractionVisualisationData
        {
            public FractionVisualisationData(Fraction storedFraction, Fraction visualisedFraction,  
                FractionVisualisationStyle visStyle, OffsetAndSpacing offsetAndSpacing, 
                List<Vector3Int> visualisedCoordinates)
            {
                StoredFraction = storedFraction;
                VisualisedFraction = visualisedFraction; 
                VisStyle = visStyle;
                OffsetAndSpacing = offsetAndSpacing;
                VisualisedCoordinates = visualisedCoordinates;
                VisualisedFigures = new Dictionary<Vector3Int, GameObject>();
            }

            public FractionVisualisationData(Fraction fraction, 
                FractionVisualisationStyle visStyle, OffsetAndSpacing offsetAndSpacing,
                List<Vector3Int> visualisedCoordinates) : this(fraction, fraction, visStyle, offsetAndSpacing, visualisedCoordinates)
            { }
            
            public Fraction StoredFraction;
            public Fraction VisualisedFraction;
            public Vector3Int Dimensions => OffsetAndSpacing.Dimensions; 
            public int Layers => OffsetAndSpacing.Layers;
            public int Columns => OffsetAndSpacing.Columns;
            public int Rows => OffsetAndSpacing.Rows; 
            
            public FractionVisualisationStyle VisStyle;
            public OffsetAndSpacing OffsetAndSpacing;
            public List<Vector3Int> VisualisedCoordinates;
            public Dictionary<Vector3Int, GameObject> VisualisedFigures; //TODO: check whether one variable for VisualisedCoordinates & VisualisedFigures could be enough
            
            public GameObject FigureParent => VisStyle.figureParent; 

            public void ClearFigures()
            {
                foreach (KeyValuePair<Vector3Int, GameObject> figure in VisualisedFigures)
                {
                    Destroy(figure.Value);
                }
                    
                foreach (Transform figure in FigureParent.transform)
                {
                    Destroy(figure.gameObject);
                }
                
                VisualisedFigures.Clear();
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

        private Dictionary<VisualisationType, Tuple<Fraction, FractionVisualisationData>> _visualisationDataMapNew =
            new Dictionary<VisualisationType, Tuple<Fraction, FractionVisualisationData>>();
        
        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;
        #endregion
        #endregion
        
        #region MonoBehaviourFunctions

        private void Awake()
        {
            
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

            VisualiseFraction(_visualisationDataMap[VisualisationType.Right].StoredFraction, VisualisationType.Right);
        }

        public void VisualiseFraction(Fraction fraction, VisualisationType visType, int modifyFactor = 1,
            ModifyType modifyType = ModifyType.None)
        {
            ResetVisType(visType);
            _visualisationDataMap.Add(visType, VisualiseSingleFraction(visType, fraction));

            foreach (KeyValuePair<VisualisationType, FractionVisualisationData> typeDataPair in _visualisationDataMap)
            {
                _visualisationDataMap[visType] = VisualiseSingleFraction(visType, _visualisationDataMap[visType].StoredFraction); 
            }
            
            UpdateBoards();
        }

        private FractionVisualisationData VisualiseSingleFraction(VisualisationType visType, Fraction fraction)
        {
            _visualisationDataMap.TryGetValue(VisualisationType.Left, out FractionVisualisationData leftData);
            _visualisationDataMap.TryGetValue(VisualisationType.Right, out FractionVisualisationData rightData);
            
            Fraction combinedFraction = CalculateCombinedFraction();
            Fraction visualisedFraction = GetVisualisedFraction(); 
            FractionVisualisationStyle visStyle = GetVisualisationType();
            OffsetAndSpacing offsetAndSpacing = new OffsetAndSpacing(GetDimensionOfFractionVisualisation(combinedFraction), boardSize);
            List<Vector3Int> visualisedCoordinates = GetVisualisedCoordinates(); 
            
            FractionVisualisationData visData = new FractionVisualisationData(
                fraction, visualisedFraction, visStyle, offsetAndSpacing, visualisedCoordinates
                ); 
            SpawnFigures(ref visData);

            return visData; 
            
            Fraction GetVisualisedFraction()
            {
                return visType switch
                {
                    VisualisationType.Left => fraction,
                    VisualisationType.Right => _operation switch
                    {
                        Operation.Nop or Operation.Add or Operation.Subtract => fraction, 
                        Operation.Multiply or Operation.Divide => combinedFraction ?? fraction, 
                        _ => throw new SwitchExpressionException()
                    },
                    VisualisationType.LeftModify or VisualisationType.RightModify or VisualisationType.None =>
                        throw new NotImplementedException(),
                    _ => throw new SwitchExpressionException()
                }; 
            }

            List<Vector3Int> GetVisualisedCoordinates()
            {
                return visType switch
                {
                    VisualisationType.Left =>
                        CalculateVisualisedCoordinatesViaDivisors(fraction, 0, fraction.Numerator),
                    VisualisationType.Right => 
                        _operation switch
                        {
                            Operation.Nop => CalculateVisualisedCoordinatesViaDivisors(fraction, 0, fraction.Numerator), 
                            Operation.Add => CalculateVisualisedCoordinatesViaDivisors(fraction, leftData.StoredFraction.Numerator, fraction.Numerator), 
                            Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(fraction, combinedFraction.Numerator, rightData.StoredFraction.Numerator), 
                            Operation.Multiply or Operation.Divide => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, (combinedFraction ?? fraction).Numerator)
                        }, 
                    VisualisationType.LeftModify or VisualisationType.RightModify or VisualisationType.None => 
                        throw new NotImplementedException(), 
                    _ => throw new SwitchExpressionException()
                }; 
            }

            FractionVisualisationStyle GetVisualisationType()
            {
                return visType switch
                {
                    VisualisationType.Left => visStyle_Main,
                    VisualisationType.Right => visStyle_Transparent,
                    VisualisationType.LeftModify => visStyle_Transparent,
                    VisualisationType.RightModify => visStyle_Transparent,
                    VisualisationType.None or _ => throw new SwitchExpressionException()
                }; 
            }
        }
        #endregion
        
        #region VisualisationManagerFunctions
        
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
            
            Fraction fraction = _visualisationDataMap[unmodifiedVisType].StoredFraction; 
            
            //TODO: Check whether a planned visualisation is valid
            Fraction modifiedFraction;
            FractionVisualisationStyle visStyle = visType == VisualisationType.LeftModify ? visStyle_Main : visStyle_Transparent;
            OffsetAndSpacing offsetAndSpacing;
            List<Vector3Int> visCoordinates;
            switch (modifyType)
            {
                case ModifyType.Expand:
                    modifiedFraction = fraction.ExpandBy(factor);
                    
                    break; 
                case ModifyType.Simplify:
                    modifiedFraction = fraction.SimplifyBy(factor); 
                    
                    break; 
                default: 
                case ModifyType.None:
                    throw new NotSupportedException(); 
                    break;
            }
            
            
            FractionVisualisationData visData =
                new FractionVisualisationData(
                    modifiedFraction,
                    visStyle,
                    new OffsetAndSpacing(GetDimensionOfFractionVisualisation(modifiedFraction), boardSize),
                    ModifyVisualisedCoordinates(_visualisationDataMap[unmodifiedVisType], fraction, modifyType)
                );

            _visualisationDataMap[visType] = visData; 

            SpawnFigures(ref visData);
        }
        #endregion
        
        #region InSceneVisualisation

        private void UpdateBoards()
        {
            if (!_visualisationDataMap.TryGetValue(VisualisationType.Left, out FractionVisualisationData leftData))
            {
                ResetBoards();
                return; 
            }

            Vector3Int dimensions = Vector3Int.one;
            dimensions = leftData.Dimensions; 
            foreach (FractionVisualisationData visData in _visualisationDataMap.Values)
            {
                dimensions.x = Mathf.Max(visData.Layers, dimensions.x); 
            }
            
            UpdateBoards(dimensions);
        }

        private void UpdateBoards(Vector3Int dimensions)
        {
            for (int i = 0; i < dimensions.x; i++)
            {
                boardLayers[i].SetActive(true);
                //TODO: Set BoardLayer visuals
            }
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(XAmount, dimensions.y);
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(YAmount, dimensions.z);

            for (int i = dimensions.x; i < boardLayers.Length; i++)
            {
                boardLayers[i].SetActive(false);
            }
        }

        private void ResetBoards()
        {
            UpdateBoards(Vector3Int.one);
        }

        private void VisualiseLayer(int layer)
        {
            foreach (FractionVisualisationData visData in _visualisationDataMap.Values)
            {
                foreach (KeyValuePair<Vector3Int, GameObject> figure in visData.VisualisedFigures) //TODO: visData.VisualisedFigures.Count is zero
                {
                    if (figure.Key.x == layer || figure.Key.x == boardLayers.Length-1)
                    {
                        figure.Value.SetActive(true);
                    }
                    else
                    {
                        figure.Value.SetActive(false);
                    }
                }
            }
        }

        private void ResetVisType(VisualisationType visType)
        {
            if (_visualisationDataMap.ContainsKey(visType))
            {
                _visualisationDataMap[visType].ClearFigures();
                _visualisationDataMap.Remove(visType); 
            } 
        }
        
        private void SpawnFigures(ref FractionVisualisationData visData) //needs to be ref, because it should be the actual one from the Dictionary that get sset to a new value in the function
        {
            visData.ClearFigures();
            visData.VisualisedFigures.Clear();
            
            GameObject figurePrefab; 
            Vector3 adjustedScale;
            if (visData.VisualisedFraction.Denominator < figurePrefabs.Length)
            {
                figurePrefab = figurePrefabs[visData.VisualisedFraction.Denominator];
                adjustedScale = Vector3.one; 
            }
            else
            {
                figurePrefab = figurePrefabs[0];
                adjustedScale = Vector3.one * (0.75f * boardSize.x/Mathf.Max(visData.Columns, visData.Rows)); 
            }
            
            foreach(Vector3Int coordinates in visData.VisualisedCoordinates)
            {
                GameObject newFigure = SpawnFigure(
                    visData.FigureParent.transform,
                    figurePrefab,
                    coordinates, visData);
                if (adjustedScale != Vector3.one) //TodoLater: Remove the condition
                {
                    newFigure.transform.localScale = adjustedScale; 
                }
            }
        }

        private GameObject SpawnFigure(Transform parent, GameObject figurePrefab,
            Vector3Int coordinates, FractionVisualisationData visData)
        {
            GameObject newFigure = Instantiate(figurePrefab, parent.transform);
            newFigure.transform.localPosition = visData.OffsetAndSpacing.CalculatePosition(coordinates);
            if (coordinates.x != boardLayers.Length-1)
            {
                newFigure.SetActive(false);
            }
            visData.VisStyle.ApplyToObject(newFigure);
            //ensure there is no other figure at the same position
            if (visData.VisualisedFigures.TryGetValue(coordinates, out GameObject blockingFigure))
            {
                Destroy(blockingFigure);
                Debug.LogWarning("Spawning new Figure on a coordinate which already contains a figure: " + coordinates); 
            }
            visData.VisualisedFigures.Add(coordinates, newFigure);
            
            return newFigure; 
        }
        #endregion
        
        #region VisualisationCalculationFunctions

        private Fraction CalculateCombinedFraction()
        {
            if (!(_visualisationDataMap.TryGetValue(VisualisationType.Left, out FractionVisualisationData leftData) &&
                  _visualisationDataMap.TryGetValue(VisualisationType.Right, out FractionVisualisationData rightData)))
            {
                return null; 
            }

            switch (_operation)
            {
                case Operation.Add:
                    if (leftData.StoredFraction.Denominator != rightData.StoredFraction.Denominator) { goto case Operation.Nop; }
                    return leftData.StoredFraction + rightData.StoredFraction; 
                    break; 
                case Operation.Subtract:
                    if (leftData.StoredFraction.Denominator != rightData.StoredFraction.Denominator) { goto case Operation.Nop; }
                    return leftData.StoredFraction - rightData.StoredFraction; 
                    break; 
                case Operation.Multiply:
                    return leftData.StoredFraction * rightData.StoredFraction; 
                    break; 
                case Operation.Divide:
                    return leftData.StoredFraction / rightData.StoredFraction; 
                    break; 
                case Operation.Nop:
                    return null; 
                    break; 
            }
            
            return null; 
        }
        
        private Vector3Int GetDimensionOfFractionVisualisation(Fraction fraction, int startingIndex = 0)
        {
            Vector2Int ColumnsAndRows = numbersToPrimeFactors[fraction.Denominator].factors
                .Aggregate(Vector2Int.one, (product, factor)=>product*factor);
            return new Vector3Int(
                Mathf.CeilToInt((float)(fraction.Numerator + startingIndex) / fraction.Denominator), 
                ColumnsAndRows.x, 
                ColumnsAndRows.y
                ); 
        }
        
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
                                    layers, //layer
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
            
            Fraction oldFraction = oldVisData.StoredFraction; //ZyKa TODO: figure out which fraction you need here 
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
        public void Editor_GeneratePrimeFactorsForNumbers() //TodoLater stuff so that it also works for e.g. 3*5, 3*7, 5*7, ...
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
                Debug.Log(vd.VisualisedFraction);
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
                new OffsetAndSpacing(GetDimensionOfFractionVisualisation(fraction), boardSize) :
                new OffsetAndSpacing(Vector3Int.one, boardSize);
                
            Debug.LogWarning("Visualised Fraction: " + fraction + " is not between 0 and 1 or has a too big Denominator");
        }
        #endregion
    }
}

#region deprecated
    
#endregion