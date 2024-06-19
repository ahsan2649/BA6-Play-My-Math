using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillEnemy : MonoBehaviour
{
    public Canvas parentCanvas;
    public GameObject card;
  //  [SerializeField] List<CardInfo> enemies = new List<CardInfo>();
    //List<CardInfo> tempEnemies= new List<CardInfo>();
    [SerializeField] int[] enemyPlacement = new int[2];
    // Start is called before the first frame update
    void Start()
    {
        //tempEnemies = enemies;
        //for (int i = 0; i < enemyPlacement.Length; i++)
        //{
        //    int randomSpawn = Random.Range(0, tempEnemies.Count);
        //    Instantiate(card, new Vector3(enemyPlacement[i] + 960, 300 + 540, 0), Quaternion.identity, parentCanvas.transform);
        //    card.GetComponent<CardDisplay>().cardInfo = tempEnemies[randomSpawn];
        //    tempEnemies.RemoveAt(randomSpawn);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
