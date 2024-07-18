using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Rewards
{
    public class RewardCardComponent : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click!");
            DeckComponent.Instance.initDeck.Add(new Fraction(GetComponent<NumberCardComponent>().Value));
            transform.SetParent(DeckComponent.Instance.transform);
            GetComponent<CardMovementComponent>().TransformToNewParentCoroutines();
            
            foreach (Transform slot in RewardBoardComponent.Instance.slots)
            {
                if (slot.childCount > 0)
                {
                    Destroy(slot.GetChild(0).gameObject);
                }
            }
            RewardBoardComponent.Instance.GenerateRewards();
        }
    }
}