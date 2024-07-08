using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Rewards {
    public class RewardsGenerator : MonoBehaviour {
        public Transform[] slots;
        public static RewardsGenerator Instance { get; private set; }

        public GameObject cardPrefab;
        public int target;
        public int rewardCount = 0;

        [SerializeField] private RewardsSlider rewardsSlider;

        // Start is called before the first frame update
        private void OnEnable()
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

        void Start()
        {
            StartCoroutine(rewardsSlider.CountRewards(target));
            StartCoroutine(rewardsSlider.SlideRewards(target));
            
            foreach (int threshold in rewardsSlider.thresholdValues)
            {
                if (target >= threshold)
                {
                    rewardCount++;
                }
            }

            if (rewardCount > 0)
            {
                GenerateRewards();
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void GenerateRewards()
        {
            if (rewardCount <= 0)
            {
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
    }
}