using Programming.Card_Mechanism;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCards : MonoBehaviour
{
    public Canvas parentCanvas;
    public GameObject card;
    [SerializeField] List<NumberCard> cards = new List<NumberCard>();
    List<NumberCard> tempCards =new List<NumberCard>();
    [SerializeField] Transform[] cardPlacement = new Transform[5]; 
    // Start is called before the first frame update
    void Start()
    {
        tempCards = cards;
        for(int i = 0; i< cardPlacement.Length; i++)
        {
            int randomSpawn = Random.Range(0, tempCards.Count);
            Instantiate(card, cardPlacement[i].position, cardPlacement[i].rotation, cardPlacement[i].gameObject.transform);
            card.GetComponent<CardDisplay>().thisCard = tempCards[randomSpawn];
            tempCards.RemoveAt(randomSpawn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
