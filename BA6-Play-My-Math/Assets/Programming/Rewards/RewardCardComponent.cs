using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardCardComponent : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData)
    {
        Transform Deck = DeckComponent.Instance.transform;
        transform.SetParent(Deck);

        transform.SetPositionAndRotation(new Vector3(Deck.position.x, Deck.position.y, Deck.position.z),
            Quaternion.Euler(-90, 0, 0));

        foreach (Transform slot in RewardsGenerator.Instance.slots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
        
        RewardsGenerator.Instance.GenerateRewards();
    }
}