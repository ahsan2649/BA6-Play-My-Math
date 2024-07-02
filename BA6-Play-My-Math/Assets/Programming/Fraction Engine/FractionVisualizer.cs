using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using UnityEngine;

namespace Programming.Fraction_Engine
{
    #region PublicSubClasses
    public enum FractionVisualisationType
    {
        None,
        Left,
        LeftModify,
        Right,
        RightModify
    }
    
    [CreateAssetMenu(fileName = "FractionVisualisationStyle", menuName = "ScriptableObjects/FractionVisualisationStyle", order = 2)]
    public class FractionVisualisationStyle : ScriptableObject
    {
        public Material figureMaterial;
        public string figureType;

        public void ApplyToObject(GameObject gO)
        {
            gO.GetComponent<MeshRenderer>().material = figureMaterial;
            gO.name = figureType; 
        }
    }
    #endregion
    
    public class FractionVisualizer : MonoBehaviour
    {
        #region PrivateDataCollectionSubClasses
        [Serializable]
        private struct Vector2IntArray
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
        
        private struct OffsetAndSpacing
        {
            public OffsetAndSpacing(Vector2Int columnsAndRows, Vector3 baseOffset, Vector3 figureSpacing, float boardHeight)
            {
                ColumnsAndRows = columnsAndRows;
                BaseOffset = baseOffset;
                FigureSpacing = figureSpacing;
                BoardHeight = boardHeight; 
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
                BoardHeight = boardSize.y; 
            }

            public int Denominator => ColumnsAndRows.x * ColumnsAndRows.y; 
            public Vector2Int ColumnsAndRows;
            public float BoardHeight; 
            public Vector3 BaseOffset;
            public Vector3 FigureSpacing;

            public Vector3 CalculatePosition(Vector2Int coordinates)
            {
                return BaseOffset + new Vector3(coordinates.x * FigureSpacing.x, 0, coordinates.y * FigureSpacing.z);
            }
        }

        private struct FractionVisualisationData
        {
            public FractionVisualisationData(Fraction fraction, int[] visualisedIndices,
                FractionVisualisationStyle style, OffsetAndSpacing offsetAndSpacing, Vector2Int[] packingCoordinates)
            {
                Fraction = fraction;
                VisualisedIndices = visualisedIndices; 
                VisualisationStyle = style;
                OffsetAndSpacing = offsetAndSpacing;
                PackingCoordinates = packingCoordinates;
                spawnedFigures = new List<GameObject>(); 
            }

            public Fraction Fraction;
            public int[] VisualisedIndices; 
            public FractionVisualisationStyle VisualisationStyle;
            public OffsetAndSpacing OffsetAndSpacing;
            public Vector2Int[] PackingCoordinates;

            public int TopLayerIndex => VisualisedIndices.Length > 0 ? VisualisedIndices[VisualisedIndices.Length-1] / Fraction.Denominator : 0;
            public int MaxFiguresPerLayer => Fraction.Denominator; 
            
            public List<GameObject> spawnedFigures; 
        }
        #endregion
    
        #region Variables
        #region GlobalStatics
        private static readonly int XAmount = Shader.PropertyToID("_X_Amount");
        private static readonly int YAmount = Shader.PropertyToID("_Y_Amount");
        #endregion
        
        #region References
        [SerializeField] private FractionVisualisationStyle leftStyle; 
        [SerializeField] private FractionVisualisationStyle leftModifyStyle; 
        
        [SerializeField] private FractionVisualisationStyle rightStyle; 
        [SerializeField] private FractionVisualisationStyle rightModifyStyle; 
        
        [SerializeField] private GameObject boardPrefab; 
        [SerializeField] private MeshRenderer topLayerBoardRenderer;
        [SerializeField] private List<GameObject> boardLayers; 
        
        #endregion
        
        #region EditorVariables
        [SerializeField] private GameObject[] figurePrefabs;
        [SerializeField] private Vector2IntArray[] numbersToPrimeFactors;
        [SerializeField] private Vector3 boardSize;
        #endregion
        
        #region CodeVariables
        //CODE VARIABLES
        private Dictionary<FractionVisualisationType, FractionVisualisationData> _visualisationDataMap =
            new Dictionary<FractionVisualisationType, FractionVisualisationData>();
        private Vector2Int[] _fractionADivisorOrder;
        private Operation _operation;
        #endregion
        #endregion
        
        #region MonoBehaviourFunctions
        private void Awake()
        {
            transform.DestroyAllChildren();
        }
        #endregion
        
        #region AccessorFunctions
        public void VisualiseOperation(Operation operation)
        {
            _operation = operation;

            if (!(_visualisationDataMap.ContainsKey(FractionVisualisationType.Left) &&
                  _visualisationDataMap.ContainsKey(FractionVisualisationType.Right)))
            {
                return;
            }

            VisualiseFraction(_visualisationDataMap[FractionVisualisationType.Right].Fraction, FractionVisualisationType.Right);
        }

        //OWN FUNCTIONS
        public void VisualiseFraction(Fraction fraction, FractionVisualisationType visType, int modifyFactor = 1, ModifyType modifyType = ModifyType.Expand)
        {
            switch (visType)
            {
                case FractionVisualisationType.Left:
                    VisualiseLeftCard(fraction);
                    if (_visualisationDataMap.ContainsKey(FractionVisualisationType.LeftModify))
                    {
                        VisualiseModify(FractionVisualisationType.LeftModify, ModifyType.None, 0);
                    }
                    if (_visualisationDataMap.ContainsKey(FractionVisualisationType.Right))
                    {
                        goto case FractionVisualisationType.Right; 
                    }
                    break;
                case FractionVisualisationType.Right:
                    VisualiseRightCard(fraction);
                    if (_visualisationDataMap.ContainsKey(FractionVisualisationType.RightModify))
                    {
                        VisualiseModify(FractionVisualisationType.RightModify, ModifyType.None , 0);
                    }
                    break;
                case FractionVisualisationType.LeftModify:
                case FractionVisualisationType.RightModify:
                    VisualiseModify(visType, modifyType, modifyFactor);
                    break;
                case FractionVisualisationType.None:
                    throw new NotSupportedException();
            }
        }
        #endregion
        
        #region VisualisationManagerFunctions
        private void VisualiseLeftCard(Fraction fraction)
        {
            DeleteVisualsIfExistent(FractionVisualisationType.Left);

            if (fraction is null || fraction == new Fraction(0, 1))
            {
                ResetBoard();
                return;
            }

            int[] visualisedIndeces;
            OffsetAndSpacing offsetAndSpacing;
            Vector2Int[] packingCoordinates; 
            
            if (fraction.Denominator > numbersToPrimeFactors.Length)
            {
                NotBetween01Error(fraction, out offsetAndSpacing, out visualisedIndeces, out packingCoordinates);
                return; 
            }
            else
            {
                visualisedIndeces = Enumerable.Range(0, fraction.Numerator).ToArray(); 
                offsetAndSpacing =
                    new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors, boardSize);
                packingCoordinates = CalculatePackingCoordinatesViaDivisors(fraction); 
            }
            
            FractionVisualisationData visualisationData =
                new FractionVisualisationData(
                    fraction,
                    visualisedIndeces, 
                    leftStyle,
                    offsetAndSpacing,
                    packingCoordinates
                );

            _visualisationDataMap.Add(FractionVisualisationType.Left, visualisationData);

            UpdateBoard(visualisationData);
            SpawnFiguresForLayer(visualisationData, visualisationData.TopLayerIndex);
        }

        private void VisualiseRightCard(Fraction rightFraction)
        {
            DeleteVisualsIfExistent(FractionVisualisationType.Right);

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
            if (_visualisationDataMap.TryGetValue(FractionVisualisationType.Left, out leftData))
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
                    if (leftFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction + rightFraction, out offsetAndSpacing, out visualisedIndeces, out packingCoordinates);
                        break; 
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
                    if (leftFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction - rightFraction, out offsetAndSpacing, out visualisedIndeces, out packingCoordinates);
                        break; 
                    }
                    visualisedIndeces = Enumerable.Range(leftFraction.Numerator - rightFraction.Numerator, rightFraction.Numerator).ToArray(); 
                    offsetAndSpacing = leftData.OffsetAndSpacing;
                    packingCoordinates = leftData.PackingCoordinates;
                    break;
                case Operation.Multiply:
                    if (leftFraction.Denominator*rightFraction.Denominator > numbersToPrimeFactors.Length)
                    {
                        NotBetween01Error(leftFraction * rightFraction, out offsetAndSpacing, out visualisedIndeces, out packingCoordinates);
                        break; 
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
                    rightStyle,
                    offsetAndSpacing,
                    packingCoordinates
                );

            _visualisationDataMap.Add(FractionVisualisationType.Right, visualisationData);
            SpawnFiguresForLayer(visualisationData, visualisationData.TopLayerIndex);
        }

        private void VisualiseModify(FractionVisualisationType visType, ModifyType modifyType, int factor)
        {
            FractionVisualisationType unmodifiedVisType = visType == FractionVisualisationType.LeftModify
                ? FractionVisualisationType.Left
                : FractionVisualisationType.Right;
            DeleteVisualsIfExistent(visType);
            if (modifyType == ModifyType.None || factor == 0)
            {
                return; 
            }
            if (!_visualisationDataMap.ContainsKey(unmodifiedVisType))
            {
                if (unmodifiedVisType == FractionVisualisationType.Left)
                {
                    Debug.LogError("Can't visualise 'LeftModify' because 'Left' is not set");
                    _visualisationDataMap[FractionVisualisationType.Left].spawnedFigures.ForEach(Application.isEditor ? DestroyImmediate : Destroy); //ZYKA!
                }
                if (unmodifiedVisType == FractionVisualisationType.Right)
                {
                    Debug.LogError("Can't visualise 'RightModify' because 'Right' is not set");
                    _visualisationDataMap[FractionVisualisationType.Right].spawnedFigures.ForEach(Application.isEditor ? DestroyImmediate : Destroy); 
                }
                return; 
            }

            Fraction fraction = _visualisationDataMap[unmodifiedVisType].Fraction;
            Fraction modifiedFraction;
            int[] visualisedIndices;
            FractionVisualisationStyle visStyle;
            OffsetAndSpacing os;
            Vector2Int[] packingCoordinates; 
            if (modifyType == ModifyType.Expand)
            {
                modifiedFraction = fraction.ExpandBy(factor);
                visualisedIndices = ExpandVisualisedIndeces(_visualisationDataMap[unmodifiedVisType], factor);
                visStyle = visType == FractionVisualisationType.Left ? leftModifyStyle : rightModifyStyle;
                os = new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator * factor].factors, boardSize); 
                packingCoordinates = ExpandPackingCoordinates(_visualisationDataMap[unmodifiedVisType].PackingCoordinates, factor);
            }
            else //TODO: this is not correctly working
            {
                modifiedFraction = fraction.SimplifyBy(factor);
                visualisedIndices = SimplifyVisualisedIndeces(_visualisationDataMap[unmodifiedVisType], factor);
                visStyle = visType == FractionVisualisationType.Left ? leftModifyStyle : rightModifyStyle;
                os = new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator / factor].factors, boardSize); 
                packingCoordinates = SimplifyPackingCoordinates(_visualisationDataMap[unmodifiedVisType].PackingCoordinates, factor);
            }
            
            FractionVisualisationData visualisationData = new FractionVisualisationData(
                        modifiedFraction,
                        visualisedIndices,
                        visStyle,
                        os,
                        packingCoordinates); 

            _visualisationDataMap.Add(visType, visualisationData);

            SpawnFiguresForLayer(visualisationData, visualisationData.TopLayerIndex);
        }

        private void DeleteVisualsIfExistent(FractionVisualisationType visType)
        {
            if (_visualisationDataMap.ContainsKey(visType))
            {
                _visualisationDataMap[visType].spawnedFigures.ForEach(Application.isEditor ? DestroyImmediate : Destroy); //ZyKa! idk whether this works
                _visualisationDataMap.Remove(visType);
            }
        }
        
        public void MoveFiguresAfterOperation()
        {
            throw new NotImplementedException();
        }
        #endregion
        
        #region InSceneVisualisation

        private void ResetBoard()
        {
            for (int i = boardLayers.Count; i > 0; i--)
            {
                Destroy(boardLayers[i]);
                boardLayers.RemoveAt(i); 
            }
            
            topLayerBoardRenderer.materials[0].SetFloat(XAmount, 1);
            topLayerBoardRenderer.materials[0].SetFloat(YAmount, 1);
        }
        
        private void UpdateBoard(FractionVisualisationData visData)
        {
            for (int i = boardLayers.Count-1; i >= 0; i--)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(boardLayers[i]);
                }
                else
                {
                    Destroy(boardLayers[i]);
                }
                boardLayers.RemoveAt(i); 
            }
            for (int i = 0; i < visData.TopLayerIndex; i++)
            {
                GameObject newBoard = Instantiate(boardPrefab, transform);
                newBoard.transform.localPosition = Vector3.up * visData.OffsetAndSpacing.BoardHeight * (-i-1); 
                boardLayers.Add(newBoard);
            }

            topLayerBoardRenderer.transform.localPosition = Vector3.up * visData.OffsetAndSpacing.BoardHeight * visData.TopLayerIndex;
            
            topLayerBoardRenderer.materials[0].SetFloat(XAmount, visData.OffsetAndSpacing.ColumnsAndRows.x);
            topLayerBoardRenderer.materials[0].SetFloat(YAmount, visData.OffsetAndSpacing.ColumnsAndRows.y);
        }

        private void SpawnFiguresForLayer(FractionVisualisationData visualisationData, int layer = -1)
        {
            //ZyKa! there should be no more figures at this point, but I'm not sure about it
            //visualisationData.VisualisationParent.transform.DestroyAllChildren();
            if (layer == -1)
            {
                layer = visualisationData.TopLayerIndex; 
            }
            
            Fraction visFraction = visualisationData.Fraction; 
            int figuresPerLayer = visualisationData.OffsetAndSpacing.ColumnsAndRows.x *
                                  visualisationData.OffsetAndSpacing.ColumnsAndRows.y; 
            GameObject figurePrefab;
            Vector3 localScale;
            GameObject figureParent = layer == visualisationData.TopLayerIndex ? topLayerBoardRenderer.gameObject : boardLayers[layer]; 
                
            if (visFraction.Denominator <= figurePrefabs.Length)
            {
                figurePrefab = figurePrefabs[visFraction.Denominator];
                localScale = default; 
            }
            else
            {
                figurePrefab = figurePrefabs[0];
                float smallerDimension = Mathf.Min(
                    visualisationData.OffsetAndSpacing.FigureSpacing.x,
                    visualisationData.OffsetAndSpacing.FigureSpacing.z) * 0.9f;
                localScale = new Vector3(smallerDimension, 5, smallerDimension); 
            }

            foreach (int index in visualisationData.VisualisedIndices) //TODOLater: maybe this algorithm can be made more efficient
            {
                if (index / visualisationData.MaxFiguresPerLayer < layer)
                {
                    continue; 
                }
                if (index / visualisationData.MaxFiguresPerLayer > layer)
                {
                    break; 
                }
                
                visualisationData.spawnedFigures.Add(SpawnFigure(
                    figureParent.transform,
                    figurePrefab,
                    visualisationData.OffsetAndSpacing,
                    visualisationData.PackingCoordinates[index % figuresPerLayer], 
                    visualisationData.VisualisationStyle, 
                    localScale));
            }
        }

        private GameObject SpawnFigure(Transform parent, GameObject figurePrefab, OffsetAndSpacing offsetAndSpacing,
            Vector2Int coordinates, FractionVisualisationStyle visStyle, Vector3 localScale = default(Vector3))
        {
            GameObject figure = Instantiate(figurePrefab, parent.transform);
            figure.transform.localPosition = offsetAndSpacing.CalculatePosition(coordinates);
            if (localScale != default)
            {
                figure.transform.localScale = localScale; 
            }
            visStyle.ApplyToObject(figure);
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
            for (int yND = 0; yND < notVerticalDivisorProduct; yND++)
            {
                for (int xND = 0; xND < notHorizontalDivisorProduct; xND++)
                {
                    for (int yDiv = 0; yDiv < verticalDivisorProduct; yDiv++)
                    {
                        for (int xDiv = 0; xDiv < horizontalDivisorProduct; xDiv++)
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
                SimplifyVisualisedIndeces(leftData, factor); 
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
                        leftData.VisualisedIndices[oldIndex] * rightFraction.Denominator;
                    visualisedIndeces[newIndex] += (added / rightFraction.Denominator) * leftFraction.Numerator * rightFraction.Denominator; 
                    visualisedIndeces[newIndex] += added%rightFraction.Denominator; 
                    newIndex++; 
                }
            }
            
            return visualisedIndeces; 
        }
        
        private int[] SimplifyVisualisedIndeces(FractionVisualisationData leftData,
            int simplifyFactor) 
        {
            Fraction leftFraction = leftData.Fraction; 
            int[] visualisedIndeces = new int[leftFraction.Numerator / simplifyFactor];

            for (int i = 0; i < visualisedIndeces.Length; i++)
            {
                visualisedIndeces[i] = leftData.VisualisedIndices[i]; 
            }
            
            return visualisedIndeces; 
        }
        
        private Vector2Int[] SimplifyPackingCoordinates(Vector2Int[] packingCoordinates, int SimplifyFactor)
        {
            Vector2Int[] FactorsDifference = 
                CalcFactorDifferenceBetweenNumbers(packingCoordinates.Length, packingCoordinates.Length / SimplifyFactor);
            Vector2Int SimplifyVector = FactorsDifference.Aggregate(new Vector2Int(1, 1), (product, current) => product * current); 
            
            return SimplifyPackingCoordinates(packingCoordinates, SimplifyVector); 
        }
        
        private Vector2Int[] ModifyPackingCoordinates(Vector2Int[] packingCoordinates, int factor, ModifyType modifyType)
        {
            return modifyType == ModifyType.Expand ? 
                ExpandPackingCoordinates(packingCoordinates, factor) : 
                packingCoordinates; //visualisedIndices is already set to every x/factor, thus the packingCoordinates should stay the same 
        }
        
        private Vector2Int[] SimplifyPackingCoordinates(Vector2Int[] packingCoordinates, Vector2Int SimplifyVector)
        {
            Vector2Int[] newPackingCoordinates = new Vector2Int[packingCoordinates.Length / (SimplifyVector.x * SimplifyVector.y)];
            int SimplifyFactor = SimplifyVector.x * SimplifyVector.y; 
            
            for (int newIndex = 0; newIndex < newPackingCoordinates.Length; newIndex++)
            {
                newPackingCoordinates[newIndex] = packingCoordinates[newIndex * SimplifyFactor];
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
            List<Vector2Int> numberFactors = numbersToPrimeFactors[number].factors.ToList(); 
            List<Vector2Int> divisorFactors = numbersToPrimeFactors[divisorOfNumber].factors.ToList();

            foreach (Vector2Int factor in divisorFactors)
            {
                if (numberFactors.Contains(factor))
                {
                    numberFactors.Remove(factor); 
                }
            }

            return numberFactors.ToArray(); 
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
            VisualiseFraction(debug_leftFraction, FractionVisualisationType.Left);
            VisualiseFraction(debug_rightFraction, FractionVisualisationType.Right);
            VisualiseModify(FractionVisualisationType.LeftModify, debug_leftModifyType, debug_leftModifyFactor);
            VisualiseModify(FractionVisualisationType.RightModify, debug_rightModifyType, debug_rightModifyFactor);
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
        
        void NotBetween01Error(Fraction fraction, out OffsetAndSpacing offsetAndSpacing, out int[] visualisedIndeces, out Vector2Int[] packingCoordinates)
        {
            visualisedIndeces = Array.Empty<int>(); 
            offsetAndSpacing = Mathf.Abs(fraction.Denominator) < numbersToPrimeFactors.Length ? 
                new OffsetAndSpacing(numbersToPrimeFactors[fraction.Denominator].factors.ToArray(), boardSize) :
                new OffsetAndSpacing(Array.Empty<Vector2Int>(), boardSize);
            packingCoordinates = Array.Empty<Vector2Int>(); 
                
            Debug.LogWarning("Visualised Fraction: " + fraction + " is not between 0 and 1 or has a too big Denominator");
        }
        #endregion
    }
}