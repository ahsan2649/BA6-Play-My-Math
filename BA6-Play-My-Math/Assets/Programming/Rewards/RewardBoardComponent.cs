using System;
using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Programming.Rewards
{
    public class RewardBoardComponent : MonoBehaviour
    {
        public static RewardBoardComponent Instance;
        public Transform start, end;
        public Transform[] slots;
        public GameObject cardPrefab;
        int _achievedValue;
        float _displayValue = 0;
        public int rewardCount = 0;
        public RectTransform Slider;
        public RectTransform Counter;
        public TextMeshProUGUI Count;

        [Tooltip(
            "DeckComponent.RebuildDeck()\n" +
            "PlayerHandComponent.ClearHand()\n" +
            "DiscardPileComponent.ClearPile()" +
            "EnemyLineupComponent.CreateEnemyLineup" +
            "EnemyZoneComponent.InitializeEnemies")]
        [FormerlySerializedAs("OnBoardExit")]
        public UnityEvent onBoardExit;

        [SerializeField] public List<int> thresholdValues = new();
        [SerializeField] private List<RectTransform> thresholds = new();
        public int maxValue;

        public int roundCounterTemp;
        [SerializeField] private TMP_Text roundCounterText;

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

        public IEnumerator AnimateCounter()
        {
            _displayValue = 0;
            while (_displayValue < _achievedValue)
            {
                _displayValue += 3f * Time.deltaTime;
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
                cardNumber.Value = LevelGeneration.GenerateReward(LevelGeneration.GameMode.easy23);
                // cardNumber.Value = new Fraction(Random.Range(1, Mathf.Min(roundCounterTemp + 4, 9)), 1); 
            }
        }

        private void ResetBoard()
        {
            Counter.anchoredPosition = new Vector2(0, Counter.anchoredPosition.y);
            Count.text = 0.ToString();
            BoardExit();
        }

        public void BoardEnter()
        {
            StartCoroutine(BoardEnterCoroutine());
        }

        public void BoardExit()
        {
            onBoardExit.Invoke();
            StartCoroutine(BoardExitCoroutine());
        }

        public IEnumerator BoardEnterCoroutine()
        {
            while (Vector3.Distance(transform.position, end.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, end.position, 50f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            StartRewarding(7);
        }

        public IEnumerator BoardExitCoroutine()
        {
            while (Vector3.Distance(transform.position, start.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, start.position, 50f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}