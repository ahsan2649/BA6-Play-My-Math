using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
    [Tooltip("Deck.CreateDeck")] [SerializeField ]private UnityEvent awakeEvent; 
    [Tooltip("EnemyLineUp.Create \n Tutorial.InitializeLevel")][SerializeField] UnityEvent initialiseLevelEvent;
    [Tooltip("Hand.Fill \n EnemyLineup.StartLineup \n Tutorial.Start")][SerializeField] UnityEvent startLevelEvent;
    [Tooltip("RewardBoard.Enter \n DiscardPile.Clear \n PlayerHand.Clear \n Deck.Rebuild \n TutorialLevel.Win")] [SerializeField] private UnityEvent winEvent;
    [Tooltip("")] [SerializeField] private UnityEvent loseEvent;

    private void Awake()
    {
        awakeEvent.Invoke();
    }

    private void Start()
     {
         StartCoroutine(StartGame()); 
     }

     private IEnumerator StartGame() //Ensures that all Singletons are set before starting anything
     {
         CallInitialiseLevel();
         yield return null;
         CallStartLevel();
         yield return null;
     }

    public void CallInitialiseLevel()
    {
        initialiseLevelEvent.Invoke();
    }

    public void CallStartLevel()
    {
        startLevelEvent.Invoke();
    }

    public void CallWin()
    {
        winEvent.Invoke();
    }

    public void CallLoose()
    {
        loseEvent.Invoke();
}
}
