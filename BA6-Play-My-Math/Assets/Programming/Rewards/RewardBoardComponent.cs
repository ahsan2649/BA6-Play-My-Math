using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using Programming.OverarchingFunctionality;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Programming.Rewards
{
    public enum RewardGenerationType
    {
        Tutorial, 
        Roguelite, 
    }
    
    public class RewardBoardComponent : MonoBehaviour
    {
        public static RewardBoardComponent Instance;
        
        [Header("Values")]
        int _achievedValue;
        float _displayValue = 0;
        [HideInInspector] public int rewardCount = 0;
        public int maxValue;
        [SerializeField] public List<int> thresholdValues = new();
        [HideInInspector] public int roundCounterTemp;
        public RewardGenerationType rewardGenerationType = RewardGenerationType.Roguelite;
        [SerializeField] private float rewardCounterSpeed = 5f; 
        
        [Header("References")] 
        public Transform start; 
        public Transform end;
        public Transform[] slots;
        public RectTransform Slider;
        public RectTransform Counter;
        public TextMeshProUGUI Count;
        [FormerlySerializedAs("onBoardExit")]
        [Tooltip("Call GameManager.InitialiseLevel, which will will handle the rest")]
        [FormerlySerializedAs("OnBoardExit")]
        public UnityEvent onBoardStartExit;
        [Tooltip("Call GameManager.StartLevel, which will handle the rest")]
        public UnityEvent onBoardExited; 
        public UnityEvent onRewardsSpawned; //needed for Tutorial
        public GameObject cardPrefab;
        [SerializeField] private TMP_Text roundCounterText;
        [SerializeField] private List<RectTransform> thresholds = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void StartRewarding(int score)
        {
            _achievedValue = score;
            ArrangeThresholds();
            CountRewards();
            StartCoroutine(AnimateCounter());
            StartCoroutine(AnimateSlider());

            roundCounterTemp++;
            roundCounterText.text = "Round: " + roundCounterTemp.ToString();

        }

        public void CountRewards()
        {
            foreach (int threshold in thresholdValues)
            {
                if (_achievedValue > threshold)
                {
                    rewardCount++;
                }
            }
        }

        void ArrangeThresholds()
        {
            int count = Mathf.Min(thresholdValues.Count, thresholds.Count);
            for (int i = 0; i < count; i++)
            {
                var thresh = thresholds[i];
                thresh.anchoredPosition = new Vector2(((float)thresholdValues[i] / maxValue) * Slider.sizeDelta.x,
                    thresh.anchoredPosition.y);
                thresh.GetComponent<TextMeshProUGUI>().text = thresholdValues[i].ToString();
            }
        }

        //TODO: merge the following two functions into one function, so that the slider goes at the same speed as the counter
        public IEnumerator AnimateCounter()
        {
            _displayValue = 0;
            while (_displayValue < _achievedValue)
            {
                _displayValue += rewardCounterSpeed * Time.deltaTime;
                Count.text = ((int)_displayValue).ToString();
                yield return new WaitForEndOfFrame();
            }
            GenerateRewards();
        }

        public IEnumerator AnimateSlider()
        {
            float targetPos = ((float)_achievedValue / maxValue) * Slider.sizeDelta.x;
            while (Counter.anchoredPosition.x < targetPos)
            {
                Counter.anchoredPosition += new Vector2(10f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }

        public void GenerateRewards()
        {
            if (rewardCount <= 0)
            {
                ResetBoard();
                return;
            }

            rewardCount--;
            foreach (Transform slot in slots)
            {
                var card = GameObject.Instantiate(cardPrefab, slot);
                var cardNumber = card.GetComponent<NumberCardComponent>();
                card.GetComponent<CardMovementComponent>().enabled = false;
                card.GetComponent<ExpandSimplifyCard>().enabled = false;
                card.AddComponent<RewardCardComponent>();
                cardNumber.Value = GenerateReward(); 
                onRewardsSpawned.Invoke();
            }
        }

        public Fraction GenerateReward()
        {
            return rewardGenerationType switch
            {
                RewardGenerationType.Tutorial => TutorialLevelAndRewards.Instance.GenerateReward(),  
                RewardGenerationType.Roguelite => LevelGeneration.GenerateReward(SceneManaging.gameMode), 
                _ => throw new SwitchExpressionException()
            }; 
        }

        private void ResetBoard()
        {
            Counter.anchoredPosition = new Vector2(0, Counter.anchoredPosition.y);
            Count.text = 0.ToString();
            BoardExit();
        }

        public void BoardEnter()
        {
            Debug.Log("Board Enter");
            StartCoroutine(BoardEnterCoroutine());
        }

        public void BoardExit()
        {
            onBoardStartExit.Invoke();
            StartCoroutine(BoardExitCoroutine());
        }

        public IEnumerator BoardEnterCoroutine()
        {
            while (Vector3.Distance(transform.position, end.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, end.position, 50f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            StartRewarding(DeckComponent.Instance._cardsInDeck.Count);
        }

        public IEnumerator BoardExitCoroutine()
        {
            float distance; 
            while (Vector3.Distance(transform.position, start.position) > 7.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, start.position, 50f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            onBoardExited.Invoke();
            while (Vector3.Distance(transform.position, start.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, start.position, 50f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}