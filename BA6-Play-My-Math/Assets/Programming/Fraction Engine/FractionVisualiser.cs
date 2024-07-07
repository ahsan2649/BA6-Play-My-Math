using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Programming.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

//WARNING: big parts of this visualisation system break down at 30

/* ObjectData Hierarchy: 
 * FractionVisualisationData
 *      -> FractionVisualisationStyle
 *      -> OffsetAndSpacing
 *      -> activeFigures 
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
                BaseOffset = new Vector3(-boardSize.x / 2 + FigureSpacing.x / 2, boardSize.y, boardSize.z / 2 + FigureSpacing.z / 2);
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
            [SerializeField] public bool isFlipped = true;
            [SerializeField] public Vector3 scale = Vector3.one * 0.9f;
            [SerializeField] public Vector3 offset = Vector3.zero; 
            
            public void ApplyToObject(GameObject gameObject)
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
                gameObject.name = figureName;
                gameObject.transform.localScale = scale;
                gameObject.transform.localPosition += offset; 
                
                if (isFlipped)
                {
                    gameObject.transform.Rotate(Vector3.up, 180.0f);
                }
            }
        }

        private class FractionVisualisationData
        {
            public FractionVisualisationData(Fraction visualisedFraction,  
                FractionVisualisationStyle visStyle, OffsetAndSpacing offsetAndSpacing, 
                List<Vector3Int> visualisedCoordinates)
            {
                VisualisedFraction = visualisedFraction; 
                VisStyle = visStyle;
                OffsetAndSpacing = offsetAndSpacing;
                VisualisedCoordinates = visualisedCoordinates;
                VisualisedFigures = new Dictionary<Vector3Int, GameObject>();
            }
            
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

        [SerializeField] private GameObject boardLayersParent; 
        [SerializeField] private GameObject[] boardLayers;
        [SerializeField] internal int topLayer; 
        #endregion
        
        #region EditorVariables
        [SerializeField] private GameObject[] figurePrefabs;
        [SerializeField] private Vector2IntArray[] numbersToPrimeFactors;
        [SerializeField] private Vector3 boardSize;
        #endregion
        
        #region CodeVariables
        //CODE VARIABLES
        private Dictionary<VisualisationType, Tuple<Fraction, FractionVisualisationData>> _visualisationDataMap = new();
        
        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;
        #endregion
        #endregion
        
        #region MonoBehaviourFunctions

        private void Awake()
        {
            GameObject boardLayer0 = boardLayers[0];
            boardLayers = new GameObject[boardLayersParent.transform.childCount];
            boardLayers[0] = boardLayer0; 
            for (int i = 1; i < boardLayersParent.transform.childCount; i++)
            {
                boardLayers[i] = boardLayersParent.transform.GetChild(i-1).gameObject; 
            }
        }

        #endregion
        
        #region PublicFunctions
        public void SetOperation(Operation operation)
        {
            _operation = operation;
            FullUpdateVisualisations();
        }

        public void SetFractionVisualisation(Fraction fraction, VisualisationType visType, int modifyFactor = 1,
            ModifyType modifyType = ModifyType.None)
        {
           _visualisationDataMap.TryGetValue(visType, out Tuple<Fraction, FractionVisualisationData> oldData);
           if (oldData is not null)
           {
               oldData.Item2.ClearFigures();
               _visualisationDataMap.Remove(visType);
           }
           if (fraction is not null)
           {
               _visualisationDataMap.Add(visType, new(fraction, null)); 
           }
           
           FullUpdateVisualisations();
        }
        
        public void VisualiseLayer(int layer)
        {
            foreach (Tuple<Fraction, FractionVisualisationData> tVisData in _visualisationDataMap.Values)
            {
                foreach (KeyValuePair<Vector3Int, GameObject> figure in tVisData.Item2.VisualisedFigures) //TODO: visData.VisualisedFigures.Count is zero
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
        #endregion
        
        #region VisualisationManagerFunctions

        private void FullUpdateVisualisations()
        {
            List<VisualisationType> toUpdate = _visualisationDataMap.Keys.ToList();
            foreach (VisualisationType visTypeToUpdate in toUpdate)
            {
                Tuple<Fraction, FractionVisualisationData> tVisData = _visualisationDataMap[visTypeToUpdate]; 
                tVisData.Item2?.ClearFigures();
                _visualisationDataMap[visTypeToUpdate] = new Tuple<Fraction, FractionVisualisationData>(tVisData.Item1, VisualiseSingleFraction(visTypeToUpdate, _visualisationDataMap[visTypeToUpdate].Item1));
            }
           
            UpdateBoards();
            VisualiseLayer(topLayer);
        }
        
        private FractionVisualisationData VisualiseSingleFraction(VisualisationType visType, Fraction fraction)
        {
            if (fraction is null)
            {
                throw new NotSupportedException(); 
            }
            
            _visualisationDataMap.TryGetValue(VisualisationType.Left, out Tuple<Fraction, FractionVisualisationData> leftData);
            _visualisationDataMap.TryGetValue(VisualisationType.Right, out Tuple<Fraction, FractionVisualisationData> rightData);

            Operation operationCopy = 
                ((_operation == Operation.Add || _operation == Operation.Subtract) && leftData?.Item1.Denominator != rightData?.Item1.Denominator) ? 
                    Operation.Nop : 
                    _operation; 
            Fraction combinedFraction = CalculateCombinedFraction();
            Fraction visualisedFraction = CalcVisualisedFraction(); 
            FractionVisualisationStyle visStyle = CalcVisualisationType();
            OffsetAndSpacing offsetAndSpacing = CalcOffsetAndSpacing();
            List<Vector3Int> visualisedCoordinates = CalcVisualisedCoordinates(); 
            
            FractionVisualisationData visData = new FractionVisualisationData(
                visualisedFraction, visStyle, offsetAndSpacing, visualisedCoordinates
                ); 
            SpawnFigures(ref visData);

            return visData;

            OffsetAndSpacing CalcOffsetAndSpacing()
            {
                /*
                return visType switch
                {
                    VisualisationType.Left => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(fraction), boardSize),
                    VisualisationType.LeftModify => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(leftData!.Item1), boardSize),
                    VisualisationType.Right => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(leftData?.Item1 ?? fraction), boardSize),
                    VisualisationType.RightModify => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(leftData?.Item1 ?? rightData!.Item1 ?? fraction), boardSize),
                    _ => throw new SwitchExpressionException()
                };
                */
                return new OffsetAndSpacing(GetDimensionOfFractionVisualisation(combinedFraction ?? fraction), boardSize); 
            }
            
            Fraction CalcVisualisedFraction()
            {
                return visType switch
                {
                    VisualisationType.Left => fraction,
                    VisualisationType.Right => operationCopy switch
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

            List<Vector3Int> CalcVisualisedCoordinates()
            {
                return visType switch
                {
                    VisualisationType.Left =>
                        CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, fraction.Numerator),
                    VisualisationType.Right => 
                        operationCopy switch
                        {
                            Operation.Nop => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, fraction.Numerator), 
                            Operation.Add => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, (leftData?.Item1.Numerator) ?? fraction.Numerator, fraction.Numerator), 
                            Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, (combinedFraction ?? fraction).Numerator, fraction.Numerator), 
                            Operation.Multiply or Operation.Divide => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, (combinedFraction ?? fraction).Numerator), 
                            _ => throw new SwitchExpressionException()
                        }, 
                    VisualisationType.LeftModify or VisualisationType.RightModify or VisualisationType.None => 
                        throw new NotImplementedException(), 
                    _ => throw new SwitchExpressionException()
                }; 
            }

            FractionVisualisationStyle CalcVisualisationType()
            {
                return visType switch
                {
                    VisualisationType.Left => visStyle_Main,
                    VisualisationType.Right => visStyle_Transparent,
                    VisualisationType.LeftModify => visStyle_Transparent,
                    VisualisationType.RightModify => visStyle_Transparent,
                    _ => throw new SwitchExpressionException()
                }; 
            }
        }
        #endregion
        
        #region InSceneVisualisation

        private void UpdateBoards()
        {
            _visualisationDataMap.TryGetValue(VisualisationType.Left, out Tuple<Fraction, FractionVisualisationData> leftData);
            _visualisationDataMap.TryGetValue(VisualisationType.Right, out Tuple<Fraction, FractionVisualisationData> rightData); 
            if (leftData is null && rightData is null)
            {
                ResetBoards();
                return; 
            }

            Vector3Int dimensions = (leftData ?? rightData).Item2.Dimensions;
            foreach (Tuple<Fraction, FractionVisualisationData> tVisData in _visualisationDataMap.Values)
            {
                dimensions.x = Mathf.Max(tVisData.Item2.Layers, dimensions.x); 
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

            topLayer = dimensions.x-1; 
        }

        private void ResetBoards()
        {
            UpdateBoards(Vector3Int.one);
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
            if (!(_visualisationDataMap.TryGetValue(VisualisationType.Left, out Tuple<Fraction, FractionVisualisationData> leftData) &&
                  _visualisationDataMap.TryGetValue(VisualisationType.Right, out Tuple<Fraction, FractionVisualisationData> rightData)))
            {
                return null; 
            }

            switch (_operation)
            {
                case Operation.Add:
                    if (leftData.Item1.Denominator != rightData.Item1.Denominator) { goto case Operation.Nop; }
                    return leftData.Item1 + rightData.Item1; 
                case Operation.Subtract:
                    if (leftData.Item1.Denominator != rightData.Item1.Denominator) { goto case Operation.Nop; }
                    return leftData.Item1 - rightData.Item1; 
                case Operation.Multiply:
                    return leftData.Item1 * rightData.Item1; 
                case Operation.Divide:
                    return leftData.Item1 / rightData.Item1; 
                case Operation.Nop:
                    return null; 
            }
            
            return null; 
        }
        
        private Vector3Int GetDimensionOfFractionVisualisation(Fraction fraction, int startingIndex = 0)
        {
            if (fraction is null)
            {
                return Vector3Int.one; 
            }
            
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

            int hDiv = 1;
            int vDiv = 1;
            int hNot = 1;
            int vNot = 1;

            foreach (Vector2Int divisor in numeratorDivisors)
            {
                if (divisor.x > divisor.y)
                {
                    hDiv *= divisor.x; 
                }
                else
                {
                    vDiv *= divisor.y; 
                }
            }

            foreach (Vector2Int notDivisor in notDivisors)
            {
                if (notDivisor.x > notDivisor.y)
                {
                    hNot *= notDivisor.x; 
                }
                else
                {
                    vNot *= notDivisor.y; 
                }
            }

            //visual example of algorithm on Miro->Programming->FractionVisualiser -> bottom middle
            int remainingFigureCount = figureCount;
            int layerIndex = startingIndex / fraction.Denominator;
            startingIndex %= fraction.Denominator;
            int notIndex = startingIndex / (hDiv*vDiv); 
            int divIndex = startingIndex % (hDiv*vDiv);
            int vNotIndex = notIndex / hNot;  
            int hNotIndex = notIndex % hNot;
            int vDivIndex = divIndex / hDiv;
            int hDivIndex = divIndex % hDiv;

            while (true)
            {
                while (vNotIndex < vNot)
                {
                    while (hNotIndex < hNot)
                    {
                        while (vDivIndex < vDiv)
                        {
                            while (hDivIndex < hDiv)
                            {
                                visualisedCoordinates.Add(new Vector3Int(layerIndex, hNotIndex * hDiv + hDivIndex, vNotIndex * vDiv + vDivIndex)); 
                                remainingFigureCount--;
                                if (remainingFigureCount == 0)
                                {
                                    goto finishedList; 
                                }
                                hDivIndex++; 
                            }
                            hDivIndex = 0; 
                            vDivIndex++; 
                        }
                        vDivIndex = 0; 
                        hNotIndex++; 
                    }
                    hNotIndex = 0; 
                    vNotIndex++; 
                }
                vNotIndex = 0; 
                layerIndex++; 
            }
            
            finishedList: 
            return visualisedCoordinates; 
        }
        
        private List<Vector3Int> ModifyVisualisedCoordinates(Tuple<Fraction, FractionVisualisationData> tOldVisData,
            Fraction fraction, ModifyType modifyType)
        {
            return modifyType == ModifyType.Expand ? 
                MultiplyVisualisedCoordinates(tOldVisData, fraction) : 
                ShortenVisualisedCoordinates(tOldVisData, fraction.Denominator); 
        }
        
        private List<Vector3Int> ShortenVisualisedCoordinates(Tuple<Fraction, FractionVisualisationData> oldVisData, int shortenFactor)
        {
            List<Vector3Int> newPackingCoordinates = new List<Vector3Int>();
            
            for (int i = 0; i < oldVisData.Item2.VisualisedCoordinates.Count; i += shortenFactor)
            {
                newPackingCoordinates.Add(oldVisData.Item2.VisualisedCoordinates[i]); 
            }

            return newPackingCoordinates; 
        }
        
        private List<Vector3Int> MultiplyVisualisedCoordinates(Tuple<Fraction, FractionVisualisationData> tOldVisData, Fraction multiplicationFraction)
        {
            List<Vector3Int> newPackingCoordinates = new List<Vector3Int>();
            
            Fraction oldFraction = tOldVisData.Item1;  
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
                foreach (Vector3Int coordinate in tOldVisData.Item2.VisualisedCoordinates)
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

            for(int i = toCheck.Length-1; i >= 0; i--)
            {
                Vector2Int primeFactor = toCheck[i]; 
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

        [ContextMenu("Editor_GeneratePrimeFactorsForNumbers")]
        public void
            Editor_GeneratePrimeFactorsForNumbers() //TodoLater stuff so that it also works for e.g. 3*5, 3*7, 5*7, ...
        {
            List<int> primeNumbers = new List<int> { 2, 3 };

            numbersToPrimeFactors = new Vector2IntArray[30];
            numbersToPrimeFactors[0] = new Vector2IntArray();
            numbersToPrimeFactors[1] = new Vector2IntArray(new Vector2Int[] { new(1, 1) });
            numbersToPrimeFactors[2] = new Vector2IntArray(new Vector2Int[] { new(1, 2) });
            numbersToPrimeFactors[3] = new Vector2IntArray(new Vector2Int[] { new(3, 1) });
            numbersToPrimeFactors[4] = new Vector2IntArray(new Vector2Int[] { new(1, 2), new(2, 1) });

            for (int number = 5; number <= 30; number++)
            {
                foreach (int prime in primeNumbers)
                {
                    if (number % prime == 0)
                    {
                        int dividedNumber = number / prime;
                        Vector2Int[] dividedNumberFactors = numbersToPrimeFactors[number / prime].factors;
                        Vector2Int[] newFactor =
                        {
                            dividedNumberFactors[^1].x != 1
                                ? new Vector2Int(1, prime)
                                : new Vector2Int(prime, 1)
                        };
                        numbersToPrimeFactors[number].factors = (numbersToPrimeFactors[number / prime].factors)
                            .Concat(newFactor).ToArray();
                        break;
                    }
                }

                if (numbersToPrimeFactors[number].factors == null)
                {
                    numbersToPrimeFactors[number].factors = new Vector2Int[] { new Vector2Int(number, 1) };
                    primeNumbers.Add(number);
                }
            }
        }
        
        /*
        [SerializeField] private int editor_amountBoardLayers;
        [SerializeField] private GameObject editor_BoardLayerPrefab;
        [SerializeField] private GameObject editor_BoardLayersParent; 
        [SerializeField] private Vector3 editor_LayersDistance;
        [SerializeField] private Vector3 editor_BasicLayerOffset; 
        
        [ContextMenu("Editor_CreateBoardLayers")]
        public void Editor_CreateBoardLayers()
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
                
            GameObject boardLayerZero = boardLayers[0];
            boardLayers = new GameObject[editor_amountBoardLayers]; 
            
            editor_BoardLayersParent.transform.DestroyAllChildren();
            for (int i = 1; i < editor_amountBoardLayers; i++)
            {
                boardLayers[i] = Instantiate(editor_BoardLayerPrefab, editor_BoardLayersParent.transform);
                boardLayers[i].transform.localPosition = editor_BasicLayerOffset + editor_LayersDistance * i; 
            }
        }
        */
        
        #endregion

        #region Debug
        //DEBUG
        public Vector2Int[] debug_divisorOrder;
        public Fraction debug_leftFraction;
        public ModifyType debug_leftModifyType; 
        public int debug_leftModifyFactor; 
        
        public Operation debug_operation;
        
        public Fraction debug_rightFraction;
        public ModifyType debug_rightModifyType; 
        public int debug_rightModifyFactor;

        public int debug_VisualiseLayer; 
        
        [SerializeField] private FractionVisualisationData[] debug_VisualisationData; //this is used for checking where smth went wrong
        
        [ContextMenu("Debug_UpdateVisuals")]
        public void Debug_UpdateVisuals()
        {
            transform.DestroyAllGrandChildren();
            _operation = debug_operation; 
            SetFractionVisualisation(debug_leftFraction, VisualisationType.Left);
        }
        
        void NotBetween01Error(Fraction fraction, out OffsetAndSpacing offsetAndSpacing, out List<Vector3Int> visualisedCoordinates)
        {
            visualisedCoordinates = new List<Vector3Int>(); 
            offsetAndSpacing = Mathf.Abs(fraction.Denominator) < numbersToPrimeFactors.Length ? 
                new OffsetAndSpacing(GetDimensionOfFractionVisualisation(fraction), boardSize) :
                new OffsetAndSpacing(Vector3Int.one, boardSize);
                
            Debug.LogWarning("Visualised Fraction: " + fraction + " is not between 0 and 1 or has a too big Denominator");
        }

        [ContextMenu("Debug_VisualiseLayer")]
        public void Debug_VisualiseLayer()
        {
            VisualiseLayer(debug_VisualiseLayer);
        }
        #endregion
    }
}

#region deprecated
    
#endregion