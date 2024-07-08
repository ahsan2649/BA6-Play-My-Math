using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Rewards {
    public class RewardsGenerator : MonoBehaviour {
        public Transform[] slots;
        public static RewardsGenerator Instance { get; private set; }

        public GameObject cardPrefab;
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
            GenerateRewards();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void GenerateRewards()
        {
            foreach (Transform slot in slots)
            {
                var card = GameObject.Instantiate(cardPrefab, slot);
                var cardNumber = card.GetComponent<NumberCardComponent>();
                card.GetComponent<BaseCardComponent>().enabled = false;
                card.AddComponent<RewardCardComponent>();
                cardNumber.Value = new Fraction(Random.Range(1, 9), Random.Range(1, 9));
            }
        }
    }
}
