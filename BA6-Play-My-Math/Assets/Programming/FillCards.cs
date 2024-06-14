using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCards : MonoBehaviour
{
    public Canvas parentCanvas;
    public GameObject card;
    [SerializeField] List<Card> cards = new List<Card>();
    List<Card> tempCards =new List<Card>();
    [SerializeField] Transform[] cardPlacement = new Transform[5]; 
    // Start is called before the first frame update
    void Start()
    {
        tempCards = cards;
        for(int i = 0; i< cardPlacement.Length; i++)
        {
            int randomSpawn = Random.Range(0, tempCards.Count);
            Instantiate(card, new Vector3(cardPlacement[i].position.x+ 960, -400+540, 0), Quaternion.identity, cardPlacement[i].gameObject.transform);
            card.GetComponent<FractionDisplay>().card = tempCards[randomSpawn];
            tempCards.RemoveAt(randomSpawn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
