using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Rewards {
    public class RewardsGenerator : MonoBehaviour {
        public Transform[] slots;

        public GameObject cardPrefab;
        // Start is called before the first frame update
        void Start()
        {
            GenerateRewards();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void GenerateRewards()
        {
            foreach (Transform slot in slots)
            {
                var card = GameObject.Instantiate(cardPrefab, slot);
                var cardNumber = card.GetComponent<NumberCardComponent>();
                card.GetComponent<BaseCardComponent>().enabled = false;
                cardNumber.Value = new Fraction(Random.Range(1, 9), Random.Range(1, 9));
            
            }
        }
    }
}
