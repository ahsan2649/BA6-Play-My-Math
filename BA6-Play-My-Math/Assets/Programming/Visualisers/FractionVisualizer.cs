using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;

//WARNING: The expand / shorten Visualisation breaks down at 30 (or any other number with three distinct prime factors)

/* Coordinates & Dimensions: x->Layers, y->Columns, z->Rows
 * BoardSize: x->Width, y->Height, z->Depth
 */

namespace Programming.Visualisers
{
    public class FractionVisualizer : MonoBehaviour
    {
        #region SubClasses
        private enum BoardVisualisationMode
        {
            FullVisualisation, 
            OneFigureVisualisation, 
            OnlyText
        }
        
        [Serializable]
        public struct Vector2IntArray //needed to show stuff in the inspector
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

        public struct FigureOffsetAndSpacing
        {
            public FigureOffsetAndSpacing(Vector3Int dimensions, Vector3 baseOffset, Vector3 figureSpacing)
            {
                Dimensions = dimensions; 
                BaseOffset = baseOffset;
                FigureSpacing = figureSpacing;
            }
            
            public FigureOffsetAndSpacing(Vector3Int dimensions, Vector3 boardSize, float blockFigureHeight)
            {
                Dimensions = dimensions; 
                FigureSpacing = new Vector3(boardSize.x / dimensions.y, blockFigureHeight, -boardSize.z / dimensions.z);
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
                FractionVisualisationStyle visStyle, FigureOffsetAndSpacing figureOffsetAndSpacing, 
                List<Vector3Int> visualisedCoordinates)
            {
                VisualisedFraction = visualisedFraction; 
                VisStyle = visStyle;
                FigureOffsetAndSpacing = figureOffsetAndSpacing;
                VisualisedCoordinates = visualisedCoordinates;
                VisualisedFigures = new Dictionary<Vector3Int, GameObject>();
            }
            
            public readonly Fraction VisualisedFraction;
            public Vector3Int Dimensions => FigureOffsetAndSpacing.Dimensions; 
            public int Layers => FigureOffsetAndSpacing.Layers;
            public int Columns => FigureOffsetAndSpacing.Columns;
            public int Rows => FigureOffsetAndSpacing.Rows; 
            
            public readonly FractionVisualisationStyle VisStyle;
            public FigureOffsetAndSpacing FigureOffsetAndSpacing;
            public List<Vector3Int> VisualisedCoordinates;
            public Dictionary<Vector3Int, GameObject> VisualisedFigures; 
            
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
        [SerializeField] private FractionTextVisualiser leftFractionTextVisualiser;
        private Vector3 _leftTextVisualiserOriginPosition; 
        [SerializeField] private FractionTextVisualiser rightFractionTextVisualiser;
        private Vector3 _rightTextVisualiserOriginPosition; 
        
        [SerializeField] private FractionVisualisationStyle visStyle_Main; 
        [SerializeField] private FractionVisualisationStyle visStyle_Transparent;
        
        [SerializeField] private GameObject boardLayersParent; 
        [SerializeField] private GameObject[] boardLayers;
        #endregion
        
        #region EditorVariables
        [SerializeField] private GameObject[] figurePrefabs;
        [SerializeField] private GameObject blockFigure;
        [SerializeField] private GameObject bigDenominatorFigure; 
        [SerializeField] private Vector2IntArray[] numbersToPrimeFactors;
        [SerializeField] private Vector3 boardSize;
        [SerializeField] private float blockFigureHeight;
        [SerializeField] private float higherLayerFigureScaleFactor = 1.0f;
        [SerializeField] private float textVisualiserHeight = 5.0f; 
        #endregion
        
        #region CodeVariables
        //CODE VARIABLES
        private readonly Dictionary<OperandType, Tuple<Fraction, FractionVisualisationData>> _visualisationDataMap = new(){
            { OperandType.Left , null}, { OperandType.Right , null}};
        private readonly List<OperandType> _visualisationOrder = new List<OperandType>() 
            { OperandType.Left , OperandType.Right};
        private Operation _operation;
        private BoardVisualisationMode _boardVisualisationMode; 
        
        private int TopLayerFast => 
            (_visualisationDataMap.Values.Select(value => value.Item1))
            .Append(OperationBoardComponent.Instance.CalculateCombinedValue())
            .Aggregate(0, (maxLayer, currentFraction) => currentFraction is not null ? Mathf.Max(maxLayer, currentFraction.Numerator/currentFraction.Denominator) : maxLayer); 

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

            _leftTextVisualiserOriginPosition = leftFractionTextVisualiser.transform.position;
            _rightTextVisualiserOriginPosition = rightFractionTextVisualiser.transform.position; 
        }

        #endregion
        
        #region PublicFunctions
        public void SetSingleFractionValue(Fraction fraction, OperandType opType, int modifyFactor = 1,
            ModifyType modifyType = ModifyType.None)
        {
            _visualisationDataMap.TryGetValue(opType, out Tuple<Fraction, FractionVisualisationData> oldData);
            if (oldData is not null)
            {
                oldData.Item2.ClearFigures();
                _visualisationDataMap[opType] = null;
            }

            if (fraction is not null)
            {
                _visualisationDataMap[opType] = new(fraction, null); 
            }
        }   
        
        public void SetOperation(Operation operation)
        {
            _operation = operation;
        }
        
        public void VisualiseLayerOld(int layer)
        {
            foreach (Tuple<Fraction, FractionVisualisationData> tVisData in _visualisationDataMap.Values)
            {
                foreach (KeyValuePair<Vector3Int, GameObject> figure in tVisData.Item2.VisualisedFigures) 
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
        
        public void FullUpdateVisualisations()
        {
            int biggestDenominator = 1;
            Fraction biggestFraction = new Fraction(0, 1);
            
            foreach (Tuple<Fraction, FractionVisualisationData> tVisData in _visualisationDataMap.Values)
            {
                if (tVisData == null) { continue; }
                biggestDenominator = Mathf.Max(biggestDenominator, tVisData.Item1.Denominator);
                biggestFraction = biggestFraction > tVisData.Item1 ? biggestFraction : tVisData.Item1; 
            }

            Fraction combinedValue = OperationBoardComponent.Instance.CalculateCombinedValue(); 
            if (combinedValue is not null)
            {
                biggestFraction = biggestFraction > combinedValue ? biggestFraction : combinedValue;
                biggestDenominator = biggestDenominator > combinedValue.Denominator ? biggestDenominator : combinedValue.Denominator; 
            }

            bool bLeftAndRightOverOne = 
                (_visualisationDataMap[OperandType.Left]?.Item1 is not null && _visualisationDataMap[OperandType.Right]?.Item1 is not null) &&
                    _visualisationDataMap[OperandType.Left].Item1 > 1 &&
                     (
                         (_operation == Operation.Multiply && _visualisationDataMap[OperandType.Right]?.Item1 > 1) 
                         || 
                         (_operation == Operation.Divide && _visualisationDataMap[OperandType.Right]?.Item1 < 1)
                    ); 
            
            _boardVisualisationMode =
                biggestDenominator > numbersToPrimeFactors.Length ? BoardVisualisationMode.OnlyText :
                (biggestFraction > 2 || bLeftAndRightOverOne) ? BoardVisualisationMode.OneFigureVisualisation :
                BoardVisualisationMode.FullVisualisation;


            leftFractionTextVisualiser.gameObject.SetActive(false); //_boardVisualisationMode != BoardVisualisationMode.FullVisualisation
            rightFractionTextVisualiser.gameObject.SetActive(false);

            foreach (OperandType opTypeToUpdate in _visualisationOrder)
            {
                Tuple<Fraction, FractionVisualisationData> tVisData = _visualisationDataMap[opTypeToUpdate];
                if (tVisData is null) { continue; }
                tVisData.Item2?.ClearFigures();
                _visualisationDataMap[opTypeToUpdate] = new Tuple<Fraction, FractionVisualisationData>
                    (tVisData.Item1, VisualiseSingleFraction(opTypeToUpdate, _visualisationDataMap[opTypeToUpdate].Item1)); 
            }
           
            UpdateBoards(); 
        }
        #endregion
        
        #region VisualisationManagerFunctions
        private FractionVisualisationData VisualiseSingleFraction(OperandType opType, Fraction fraction)
        {
            if (fraction is null)
            {
                throw new NotSupportedException(); 
            }

            Tuple<Fraction, FractionVisualisationData> leftData = _visualisationDataMap[OperandType.Left]; 
            Tuple<Fraction, FractionVisualisationData> rightData = _visualisationDataMap[OperandType.Right]; 
            
            Operation operationCopy = 
                ((_operation == Operation.Add || _operation == Operation.Subtract) && leftData?.Item1.Denominator != rightData?.Item1.Denominator || 
                 (_operation == Operation.Subtract && rightData?.Item1 > leftData?.Item1)) ? 
                    Operation.Nop : 
                    _operation; 
            Fraction combinedFraction =  Fraction.CalculateOperation(leftData?.Item1, operationCopy, rightData?.Item1);
            Fraction visualisedFraction = CalcVisualisedFraction(); 
            FractionVisualisationStyle visStyle = CalcVisStyle();
            FigureOffsetAndSpacing figureOffsetAndSpacing = CalcOffsetAndSpacing();
            List<Vector3Int> visualisedCoordinates = CalcVisualisedCoordinates(); 
            FractionTextVisualiser fractionTextVisualiser = GetFractionTextVisualiser();
            Vector3 textVisualiserWorldPosition = CalcTextVisualiserWorldPosition(); 
            
            FractionVisualisationData visData = new FractionVisualisationData(
                visualisedFraction, visStyle, figureOffsetAndSpacing, visualisedCoordinates
                );

            switch (_boardVisualisationMode)
            {
                case BoardVisualisationMode.OnlyText:
                    fractionTextVisualiser.gameObject.SetActive(true);
                    fractionTextVisualiser.SetFraction(visualisedFraction, true);
                    fractionTextVisualiser.transform.position = textVisualiserWorldPosition;    
                    break; 
                case BoardVisualisationMode.OneFigureVisualisation:
                    fractionTextVisualiser.gameObject.SetActive(true);
                    fractionTextVisualiser.SetFraction(visualisedFraction, false);
                    fractionTextVisualiser.transform.position = textVisualiserWorldPosition; 
                    SpawnFigures(ref visData); 
                    break; 
                case BoardVisualisationMode.FullVisualisation:
                    fractionTextVisualiser.gameObject.SetActive(false);
                    SpawnFigures(ref visData);
                    break; 
            }
            
            return visData;

            Fraction CalcVisualisedFraction()
            {
                return opType switch
                {
                    OperandType.Left => fraction,
                    OperandType.Right => operationCopy switch
                    {
                        Operation.Nop or Operation.Add or Operation.Subtract => fraction, 
                        Operation.Multiply or Operation.Divide => combinedFraction ?? fraction, 
                        _ => throw new SwitchExpressionException()
                    },
                    OperandType.LeftModify or OperandType.RightModify or OperandType.None =>
                        throw new NotImplementedException(),
                    _ => throw new SwitchExpressionException()
                }; 
            }
            
            FractionVisualisationStyle CalcVisStyle()
            {
                return opType switch
                {
                    OperandType.Left => visStyle_Main,
                    OperandType.Right => visStyle_Transparent,
                    OperandType.LeftModify => visStyle_Transparent,
                    OperandType.RightModify => visStyle_Transparent,
                    _ => throw new SwitchExpressionException()
                }; 
            }
            
            FigureOffsetAndSpacing CalcOffsetAndSpacing()
            {
                return opType switch
                {
                    OperandType.Left or OperandType.LeftModify => new FigureOffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(fraction), boardSize, blockFigureHeight),
                    OperandType.Right or OperandType.RightModify => new FigureOffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(combinedFraction ?? fraction), boardSize, blockFigureHeight),
                    _ => throw new SwitchExpressionException()
                }; 
            }
            
            List<Vector3Int> CalcVisualisedCoordinates()
            {
                return _boardVisualisationMode switch
                {
                    BoardVisualisationMode.FullVisualisation =>
                        opType switch
                        {
                            OperandType.Left =>
                                operationCopy switch
                                {
                                    Operation.Add or Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(
                                        combinedFraction ?? fraction, 0, fraction.Numerator),
                                    Operation.Nop or Operation.Multiply or Operation.Divide =>
                                        CalculateVisualisedCoordinatesViaDivisors(fraction, 0, fraction.Numerator),
                                    _ => throw new SwitchExpressionException()
                                },
                            OperandType.Right =>
                                operationCopy switch
                                {
                                    Operation.Nop => CalculateVisualisedCoordinatesViaDivisors(
                                        combinedFraction ?? fraction, 0, fraction.Numerator),
                                    Operation.Add => CalculateVisualisedCoordinatesViaDivisors(
                                        combinedFraction ?? fraction, (leftData?.Item1.Numerator) ?? fraction.Numerator,
                                        fraction.Numerator),
                                    Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(
                                        combinedFraction ?? fraction, (combinedFraction ?? fraction).Numerator,
                                        fraction.Numerator),
                                    Operation.Multiply =>
                                        leftData is null
                                            ? CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0,
                                                (combinedFraction ?? fraction).Numerator)
                                            : MultiplyVisualisedCoordinates(leftData, fraction),
                                    Operation.Divide => 
                                        leftData is null
                                            ? CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0,
                                                (combinedFraction ?? fraction).Numerator)
                                            : MultiplyVisualisedCoordinates(leftData, new Fraction(fraction.Denominator, fraction.Numerator)),
                                    _ => throw new SwitchExpressionException()
                                },
                            OperandType.LeftModify or OperandType.RightModify or OperandType.None =>
                                throw new NotImplementedException(),
                            _ => throw new SwitchExpressionException()
                        },
                    BoardVisualisationMode.OneFigureVisualisation =>
                        new List<Vector3Int>() { new(0, 0, 0) },
                    BoardVisualisationMode.OnlyText =>
                        new List<Vector3Int>(), 
                    _ => throw new SwitchExpressionException()
                }; 
            }
            
            FractionTextVisualiser GetFractionTextVisualiser()
            {
                return opType switch
                {
                    OperandType.Left or OperandType.LeftModify => leftFractionTextVisualiser,
                    OperandType.Right or OperandType.RightModify => rightFractionTextVisualiser,
                    _ => throw new SwitchExpressionException()
                }; 
            }

            Vector3 CalcTextVisualiserWorldPosition()
            {
                if (visualisedCoordinates.Count == 0)
                {
                    return opType switch
                    {
                        OperandType.Left => _leftTextVisualiserOriginPosition,
                        OperandType.Right => _rightTextVisualiserOriginPosition,
                        _ => throw new SwitchExpressionException()
                    }; 
                }
                
                return _boardVisualisationMode switch
                {
                    BoardVisualisationMode.FullVisualisation or BoardVisualisationMode.OneFigureVisualisation =>
                        visStyle.figureParent.transform.TransformPoint
                        (figureOffsetAndSpacing.CalculatePosition(visualisedCoordinates.First()) +
                         Vector3.up * textVisualiserHeight),
                    BoardVisualisationMode.OnlyText =>
                        opType switch
                        {
                            OperandType.Left => _leftTextVisualiserOriginPosition, 
                            OperandType.Right => _rightTextVisualiserOriginPosition, 
                            _ => throw new SwitchExpressionException()
                        },
                    _ => throw new SwitchExpressionException()
                }; 
            }
        }
        #endregion
        
        #region InSceneVisualisation

        private void UpdateBoards()
        {
            Tuple<Fraction, FractionVisualisationData> leftData = _visualisationDataMap[OperandType.Left]; 
            Tuple<Fraction, FractionVisualisationData> rightData = _visualisationDataMap[OperandType.Right];
            if (leftData is null && rightData is null)
            {
                ResetBoards();
                return; 
            }

            Vector3Int dimensions = (leftData ?? rightData).Item2.Dimensions;
            foreach (Tuple<Fraction, FractionVisualisationData> tVisData in _visualisationDataMap.Values)
            {
                if (tVisData is null) { continue; }
                dimensions.x = Mathf.Max(tVisData.Item2.Layers, dimensions.x); 
            }
            
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(XAmount, dimensions.y);
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(YAmount, dimensions.z);
            //UpdateBoards(dimensions); deprecated, two lines above should be enough
        }

        private void UpdateBoards(Vector3Int dimensions)
        {
            for (int i = 0; i < dimensions.x; i++)
            {
                boardLayers[i].SetActive(true);
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
        
        private void SpawnFigures(ref FractionVisualisationData visData) //needs to be ref, because it should be the actual one from the Dictionary that get set to a new value in the function ZYKA!
        {
            visData.ClearFigures();
            visData.VisualisedFigures.Clear();

            if (visData.VisualisedFraction.Denominator > numbersToPrimeFactors.Length)
            {
                throw new NotSupportedException(); 
            }
            
            int coordinateIndex = 0; 
            foreach(Vector3Int coordinates in visData.VisualisedCoordinates)
            {
                GameObject figurePrefab;
                if (visData.VisualisedFraction.Denominator < figurePrefabs.Length && 
                    (coordinateIndex >= visData.VisualisedFraction.Numerator - visData.VisualisedFraction.Denominator || 
                     _boardVisualisationMode == BoardVisualisationMode.OneFigureVisualisation))
                {
                    figurePrefab = figurePrefabs[visData.VisualisedFraction.Denominator];
                }
                else if (visData.VisualisedFraction.Denominator > figurePrefabs.Length)
                {
                    figurePrefab = bigDenominatorFigure;
                }
                else
                {
                    figurePrefab = blockFigure;
                }
                
                GameObject newFigure = SpawnFigure(
                    visData.FigureParent.transform,
                    figurePrefab,
                    coordinates, visData);

                coordinateIndex++;

                if (figurePrefab == blockFigure)
                {
                    newFigure.transform.localScale = Vector3.Scale(visData.FigureOffsetAndSpacing.FigureSpacing, new Vector3(0.95f, 1.0f, 0.95f));
                }

                if (figurePrefab == bigDenominatorFigure)
                {
                    float minXY = Mathf.Min(visData.FigureOffsetAndSpacing.FigureSpacing.x, visData.FigureOffsetAndSpacing.FigureSpacing.y);
                    newFigure.transform.localScale = Vector3.one * minXY; 
                }

                newFigure.transform.localScale *= Mathf.Pow(higherLayerFigureScaleFactor, coordinates.x);
            }
        }

        private GameObject SpawnFigure(Transform parent, GameObject figurePrefab,
            Vector3Int coordinates, FractionVisualisationData visData)
        {
            GameObject newFigure = Instantiate(figurePrefab, parent.transform);
            newFigure.transform.localPosition = visData.FigureOffsetAndSpacing.CalculatePosition(coordinates);
            visData.VisStyle.ApplyToObject(newFigure);
            
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
        private Vector3Int GetDimensionOfFractionVisualisation(Fraction fraction, int startingIndex = 0)
        {
            if (fraction is null)
            {
                return Vector3Int.one; 
            }
            
            if (fraction.Denominator >= numbersToPrimeFactors.Length)
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
                                if (remainingFigureCount <= 0)
                                {
                                    goto finishedList; 
                                }
                                visualisedCoordinates.Add(new Vector3Int(layerIndex, hNotIndex * hDiv + hDivIndex, vNotIndex * vDiv + vDivIndex)); 
                                remainingFigureCount--;
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
                CalcFactorDifferenceBetweenNumbers(oldFraction.Denominator*multiplicationFraction.Denominator, oldFraction.Denominator);
            Vector2Int expandVector2 = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current);
            Vector3Int expandVector3 = new Vector3Int(0, expandVector2.x, expandVector2.y); 
            
            foreach (Vector3Int coordinate in tOldVisData.Item2.VisualisedCoordinates)
            {
                for (int i = 0, yOffset = 0, zOffset = 0, xOffset = 0; i < multiplicationFraction.Numerator; i++)
                {
                    newPackingCoordinates.Add(new Vector3Int(
                        coordinate.x + xOffset, 
                        coordinate.y * expandVector3.y + yOffset, 
                        coordinate.z * expandVector3.z + zOffset)
                    );
                    
                    yOffset++;
                    if (yOffset >= expandVector3.y)
                    {
                        yOffset = 0;
                        zOffset++; 
                    }

                    if (zOffset >= expandVector3.z)
                    {
                        zOffset = 0; 
                        xOffset++; 
                    }
                }
            
            }
            return newPackingCoordinates; 
        }
        
        private Vector2Int[] CalcFactorDifferenceBetweenNumbers(int number, int divisorOfNumber) //only works for if the second number actually divides the first one
        {
            List<Vector2Int> differentFactors = new List<Vector2Int>(); 
            List<Vector2Int> numberFactors = numbersToPrimeFactors[number].factors.ToList(); 
            List<Vector2Int> divisorFactors = numbersToPrimeFactors[divisorOfNumber].factors.ToList();

            foreach (Vector2Int factor in numberFactors)
            {
                if (divisorFactors.Contains(factor))
                {
                    divisorFactors.Remove(factor); 
                }
                else
                {
                    differentFactors.Add(factor);
                }
            }

            return differentFactors.ToArray(); 
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

            numbersToPrimeFactors[0] = new Vector2IntArray();
            numbersToPrimeFactors[1] = new Vector2IntArray(new Vector2Int[] { new(1, 1) });
            numbersToPrimeFactors[2] = new Vector2IntArray(new Vector2Int[] { new(1, 2) });
            numbersToPrimeFactors[3] = new Vector2IntArray(new Vector2Int[] { new(3, 1) });
            numbersToPrimeFactors[4] = new Vector2IntArray(new Vector2Int[] { new(1, 2), new(2, 1) });

            for (int number = 5; number < numbersToPrimeFactors.Length; number++)
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
            SetSingleFractionValue(debug_leftFraction, OperandType.Left);
        }
        
        void NotBetween01Error(Fraction fraction, out FigureOffsetAndSpacing figureOffsetAndSpacing, out List<Vector3Int> visualisedCoordinates)
        {
            visualisedCoordinates = new List<Vector3Int>(); 
            figureOffsetAndSpacing = Mathf.Abs(fraction.Denominator) < numbersToPrimeFactors.Length ? 
                new FigureOffsetAndSpacing(GetDimensionOfFractionVisualisation(fraction), boardSize, blockFigureHeight) :
                new FigureOffsetAndSpacing(Vector3Int.one, boardSize, blockFigureHeight);
                
            Debug.LogWarning("Visualised Fraction: " + fraction + " is not between 0 and 1 or has a too big Denominator");
        }

        [ContextMenu("Debug_VisualiseLayer")]
        public void Debug_VisualiseLayer()
        {
            VisualiseLayerOld(debug_VisualiseLayer);
        }
        #endregion
    }
}

#region deprecated
    
#endregion