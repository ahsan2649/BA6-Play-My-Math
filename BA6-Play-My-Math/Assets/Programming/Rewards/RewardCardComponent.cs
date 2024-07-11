using Programming.Card_Mechanism;
using UnityEngine;
using Programming.Fraction_Engine;
using UnityEngine.EventSystems;

namespace Programming.Rewards
{
    public class RewardCardComponent : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click!");
            DeckComponent.Instance.initDeck.Add(new Fraction(GetComponent<NumberCardComponent>().Value));
            RewardBoardComponent.Instance.GenerateRewards();
            
            foreach (Transform slot in RewardBoardComponent.Instance.slots)
            {
                if (slot.childCount > 0)
                {
                    Destroy(slot.GetChild(0).gameObject);
                }
            }
        }
    }
}