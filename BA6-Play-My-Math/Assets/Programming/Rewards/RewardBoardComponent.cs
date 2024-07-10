using System;
using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Programming.Rewards {
    public class RewardBoardComponent : MonoBehaviour {
        public static RewardBoardComponent Instance;
        public Transform[] slots;
        public GameObject cardPrefab;
        int _achievedValue;
        float _displayValue = 0;
        public int rewardCount = 0;
        public RectTransform Slider;
        public RectTransform Counter;
        public TextMeshProUGUI Count;

        [SerializeField] public List<int> thresholdValues = new();
        [SerializeField] private List<RectTransform> thresholds = new();
        public int maxValue;

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

        private void Start()
        {
        }

        public void StartRewarding(int score)
        {
            _achievedValue = score;
            ArrangeThresholds();
            CountRewards();
            StartCoroutine(AnimateCounter());
            StartCoroutine(AnimateSlider());
            GenerateRewards();
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
                thresh.anchoredPosition = new Vector2(((float)thresholdValues[i]/maxValue)*Slider.sizeDelta.x , thresh.anchoredPosition.y);
                thresh.GetComponent<TextMeshProUGUI>().text = thresholdValues[i].ToString();
            }
        }

        public IEnumerator AnimateCounter()
        {
            _displayValue = 0;
            while (_displayValue < _achievedValue)
            {
                _displayValue++;
                Count.text = _displayValue.ToString();
                yield return new WaitForSeconds(0.3f);
            }
        }

        public IEnumerator AnimateSlider()
        {
            float targetPos = ((float)_achievedValue / maxValue) * Slider.sizeDelta.x;
            while (Counter.anchoredPosition.x < targetPos)
            {
                Counter.anchoredPosition += new Vector2(0.05f, 0);
                yield return new WaitForSeconds(0.005f);
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
                card.GetComponent<BaseCardComponent>().enabled = false;
                card.AddComponent<RewardCardComponent>();
                cardNumber.Value = new Fraction(Random.Range(1, 9), 1);
            }
        }

        private void ResetBoard()
        {
            Counter.anchoredPosition = new Vector2(0, Counter.anchoredPosition.x);
            Count.text = 0.ToString();
            gameObject.SetActive(false);
        }
    }
}