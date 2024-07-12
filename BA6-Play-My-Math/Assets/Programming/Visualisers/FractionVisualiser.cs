using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using Programming.Visualisers;
using TMPro;
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

namespace Programming.FractionVisualiser
{
    public class FractionVisualiser : MonoBehaviour
    {
        #region SubClasses
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
            
            public OffsetAndSpacing(Vector3Int dimensions, Vector3 boardSize, float blockFigureHeight)
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
        [SerializeField] private FractionTextVisualiser rightFractionTextVisualiser;
        [SerializeField] private TMP_Text ExtraLayersText; //TODO: use this for fractions >3 (or >4)
        
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
        [SerializeField] private float higherLayerFigureScaleFactor = 1; 
        #endregion
        
        #region CodeVariables
        //CODE VARIABLES
        private Dictionary<OperandType, Tuple<Fraction, FractionVisualisationData>> _visualisationDataMap = new();
        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;
        
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
                _visualisationDataMap.Remove(opType);
            }
            if (fraction is not null)
            {
                _visualisationDataMap.Add(opType, new(fraction, null)); 
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
            List<OperandType> toUpdate = _visualisationDataMap.Keys.ToList();
            foreach (OperandType opTypeToUpdate in toUpdate)
            {
                Tuple<Fraction, FractionVisualisationData> tVisData = _visualisationDataMap[opTypeToUpdate]; 
                tVisData.Item2?.ClearFigures();
                _visualisationDataMap[opTypeToUpdate] = new Tuple<Fraction, FractionVisualisationData>(tVisData.Item1, VisualiseSingleFraction(opTypeToUpdate, _visualisationDataMap[opTypeToUpdate].Item1)); //ZyKa tracking error backwards, I'd guess it comes from here, because if I set the leftOperand while the right exists, than the right wants VisualisedFigures Data from the left coordinate, but doesn't get them
            }
           
            UpdateBoards(); 
            //VisualiseLayerOld(topLayer); //deprecated
        }
        #endregion
        
        #region VisualisationManagerFunctions
        private FractionVisualisationData VisualiseSingleFraction(OperandType opType, Fraction fraction)
        {
            if (fraction is null)
            {
                throw new NotSupportedException(); 
            }
            
            _visualisationDataMap.TryGetValue(OperandType.Left, out Tuple<Fraction, FractionVisualisationData> leftData);
            _visualisationDataMap.TryGetValue(OperandType.Right, out Tuple<Fraction, FractionVisualisationData> rightData);

            Operation operationCopy = 
                ((_operation == Operation.Add || _operation == Operation.Subtract) && leftData?.Item1.Denominator != rightData?.Item1.Denominator) ? 
                    Operation.Nop : 
                    _operation; 
            Fraction combinedFraction =  Fraction.CalculateOperation(leftData?.Item1, operationCopy, rightData?.Item1);
            Fraction visualisedFraction = CalcVisualisedFraction(); 
            FractionVisualisationStyle visStyle = CalcCardSlotType();
            OffsetAndSpacing offsetAndSpacing = CalcOffsetAndSpacing();
            List<Vector3Int> visualisedCoordinates = CheckTrueVisualisation() ? CalcVisualisedCoordinates() : new List<Vector3Int>(); //ZyKa tracking error backwards 2
            FractionTextVisualiser fractionTextVisualiser = GetFractionTextVisualiser(); 
            
            FractionVisualisationData visData = new FractionVisualisationData(
                visualisedFraction, visStyle, offsetAndSpacing, visualisedCoordinates
                );
            
            if (CheckTrueVisualisation())
            {
                fractionTextVisualiser.gameObject.SetActive(false);
                SpawnFigures(ref visData);
            }
            else
            {
                fractionTextVisualiser.gameObject.SetActive(true);
                fractionTextVisualiser.SetFraction(fraction);
            }
            
            return visData;

            bool CheckTrueVisualisation()
            {
                return visualisedFraction.Denominator < numbersToPrimeFactors.Length &&
                       visualisedFraction < 4; 
            }
            
            OffsetAndSpacing CalcOffsetAndSpacing()
            {
                return opType switch
                {
                    OperandType.Left or OperandType.LeftModify => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(fraction), boardSize, blockFigureHeight),
                    OperandType.Right or OperandType.RightModify => new OffsetAndSpacing(
                        GetDimensionOfFractionVisualisation(combinedFraction ?? fraction), boardSize, blockFigureHeight),
                    _ => throw new SwitchExpressionException()
                }; 
            }
            
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

            List<Vector3Int> CalcVisualisedCoordinates()
            {
                return opType switch
                {
                    OperandType.Left =>
                        operationCopy switch
                        {
                            Operation.Add or Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, fraction.Numerator),
                            Operation.Nop or Operation.Multiply or Operation.Divide => CalculateVisualisedCoordinatesViaDivisors(fraction, 0, fraction.Numerator), 
                            _ => throw new SwitchExpressionException()
                        }, 
                    OperandType.Right =>
                        operationCopy switch
                        {
                            Operation.Nop => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, fraction.Numerator), 
                            Operation.Add => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, (leftData?.Item1.Numerator) ?? fraction.Numerator, fraction.Numerator), 
                            Operation.Subtract => CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, (combinedFraction ?? fraction).Numerator, fraction.Numerator), 
                            Operation.Multiply or Operation.Divide => //ZyKa tracking error backwards 1
                                leftData is null || (rightData.Item1.Numerator > rightData.Item1.Denominator) ? 
                                CalculateVisualisedCoordinatesViaDivisors(combinedFraction ?? fraction, 0, (combinedFraction ?? fraction).Numerator) : 
                                MultiplyVisualisedCoordinates(leftData, fraction), 
                            _ => throw new SwitchExpressionException()
                        }, 
                    OperandType.LeftModify or OperandType.RightModify or OperandType.None => 
                        throw new NotImplementedException(), 
                    _ => throw new SwitchExpressionException()
                }; 
            }

            FractionVisualisationStyle CalcCardSlotType()
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

            FractionTextVisualiser GetFractionTextVisualiser()
            {
                return opType switch
                {
                    OperandType.Left or OperandType.LeftModify => leftFractionTextVisualiser,
                    OperandType.Right or OperandType.RightModify => leftFractionTextVisualiser,
                    _ => throw new SwitchExpressionException()
                }; 
            }
        }
        #endregion
        
        #region InSceneVisualisation

        private void UpdateBoards()
        {
            _visualisationDataMap.TryGetValue(OperandType.Left, out Tuple<Fraction, FractionVisualisationData> leftData);
            _visualisationDataMap.TryGetValue(OperandType.Right, out Tuple<Fraction, FractionVisualisationData> rightData); 
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
            
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(XAmount, dimensions.y);
            boardLayers[0].GetComponent<MeshRenderer>().materials[0].SetFloat(YAmount, dimensions.z);
            //UpdateBoards(dimensions); deprecated, two lines above should be enough
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
        
        private void SpawnFigures(ref FractionVisualisationData visData) //needs to be ref, because it should be the actual one from the Dictionary that get set to a new value in the function ZYKA!
        {
            visData.ClearFigures();
            visData.VisualisedFigures.Clear();
            
            int coordinateIndex = 0; 
            foreach(Vector3Int coordinates in visData.VisualisedCoordinates)
            {
                GameObject figurePrefab;
                bool bUseAdjustedScale = false; 
                if (visData.VisualisedFraction.Denominator < figurePrefabs.Length && 
                    coordinateIndex >= visData.VisualisedFraction.Numerator - visData.VisualisedFraction.Denominator)
                {
                    figurePrefab = figurePrefabs[visData.VisualisedFraction.Denominator];
                }
                else if (visData.VisualisedFraction.Denominator > figurePrefabs.Length)
                {
                    figurePrefab = bigDenominatorFigure;
                    bUseAdjustedScale = true; 
                }
                else //(coordinateIndex < visData.VisualisedFraction.Numerator - visData.VisualisedFraction.Denominator)
                {
                    figurePrefab = blockFigure;
                    bUseAdjustedScale = true; 
                }
                
                GameObject newFigure = SpawnFigure(
                    visData.FigureParent.transform,
                    figurePrefab,
                    coordinates, visData);

                coordinateIndex++;

                if (bUseAdjustedScale)
                {
                    newFigure.transform.localScale = Vector3.Scale(visData.OffsetAndSpacing.FigureSpacing, new Vector3(0.95f, 1.0f, 0.95f));
                    newFigure.transform.position += visData.OffsetAndSpacing.FigureSpacing.y * Vector3.up * 0.5f; //TODOLater: remove once the actual block exists 
                }

                newFigure.transform.localScale *= Mathf.Pow(higherLayerFigureScaleFactor, coordinates.x);
                newFigure.transform.localPosition += Vector3.forward * higherLayerFigureScaleFactor *0.5f * coordinates.x; 
            }
        }

        private GameObject SpawnFigure(Transform parent, GameObject figurePrefab,
            Vector3Int coordinates, FractionVisualisationData visData)
        {
            GameObject newFigure = Instantiate(figurePrefab, parent.transform);
            newFigure.transform.localPosition = visData.OffsetAndSpacing.CalculatePosition(coordinates);
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
                CalcFactorDifferenceBetweenNumbers(oldFraction.Denominator*multiplicationFraction.Denominator, oldFraction.Denominator);
            Vector2Int expandVector2 = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current);
            Vector3Int expandVector3 = new Vector3Int(0, expandVector2.x, expandVector2.y); 
            
            if (multiplicationFraction.Numerator > multiplicationFraction.Denominator)//ZyKa! move this up
            {
                newPackingCoordinates = CalculateVisualisedCoordinatesViaDivisors(oldFraction * multiplicationFraction, 0, oldFraction.Numerator * multiplicationFraction.Numerator); 
            }
            else
            {
                foreach (Vector3Int coordinate in tOldVisData.Item2.VisualisedCoordinates) //ZyKa tracking error back 0
                {
                    for (int i = 0, yOffset = 0, zOffset = 0; i < multiplicationFraction.Numerator; i++)
                    {
                        newPackingCoordinates.Add(new Vector3Int(
                            coordinate.x, 
                            coordinate.y * expandVector3.y + yOffset, 
                            coordinate.z * expandVector3.z + zOffset)
                        );
                        
                        yOffset++;
                        if (yOffset >= expandVector3.y)
                        {
                            yOffset = 0;
                            zOffset++; 
                        }
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
        
        void NotBetween01Error(Fraction fraction, out OffsetAndSpacing offsetAndSpacing, out List<Vector3Int> visualisedCoordinates)
        {
            visualisedCoordinates = new List<Vector3Int>(); 
            offsetAndSpacing = Mathf.Abs(fraction.Denominator) < numbersToPrimeFactors.Length ? 
                new OffsetAndSpacing(GetDimensionOfFractionVisualisation(fraction), boardSize, blockFigureHeight) :
                new OffsetAndSpacing(Vector3Int.one, boardSize, blockFigureHeight);
                
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