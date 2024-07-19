using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Manager : MonoBehaviour
{
     [Tooltip("Initialize Deck and EnemyLineup")][SerializeField] UnityEvent awakeEvent;
     [Tooltip("Initialize PlayerHand and EnemyZone")][SerializeField] UnityEvent startEvent;
     [Tooltip("Hand and Discards to deck, bring in board")] [SerializeField] private UnityEvent winEvent;
     [Tooltip("")] [SerializeField] private UnityEvent loseEvent;

     private void Awake()
    {
        awakeEvent.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        startEvent.Invoke();
    }

    public void CallWin()
    {
        winEvent.Invoke();
    }

}
