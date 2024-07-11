using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Rewards
{
    public class RewardCardComponent : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click!");
            Transform deck = DeckComponent.Instance.transform;
            transform.SetParent(deck);

            transform.SetPositionAndRotation(new Vector3(deck.position.x, deck.position.y, deck.position.z),
                Quaternion.Euler(-90, 0, 0));

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