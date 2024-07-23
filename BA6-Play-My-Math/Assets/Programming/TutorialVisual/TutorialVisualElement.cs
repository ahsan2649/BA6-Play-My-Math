using System;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public abstract class TutorialVisualElement : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialVisual;
        public UnityEvent onTutorialStepFinished;
        public bool bCanFinishBeforeActivate;
        public bool bFinished; 
        
        public virtual void Awake()
        {
            if (tutorialVisual is null)
            {
                tutorialVisual = this.gameObject; 
            }
        }

        public virtual void Start()
        {
            ResetEvents();
        }
        
        public virtual void Update()
        {
            
        }

        public void ResetEvents()
        {
            foreach (UnityEvent activationEvent in GetOpenEvents())
            {
                activationEvent.AddListener(Open);
            }
            foreach (UnityEvent finishEvent in GetCloseEvents())
            {
                finishEvent.AddListener(Close);
            }
            tutorialVisual.SetActive(false);
            bFinished = false; 
        }

        public void Open()
        {
            Open(true);
        }
        protected bool Open(bool checkCondition = true)
        {
            if (CheckOpenCondition() || !checkCondition)
            {
                tutorialVisual.SetActive(true); 
                foreach (UnityEvent activationEvent in GetOpenEvents())
                {
                    activationEvent.RemoveListener(Open);
                }
                OnOpen();
                return true; 
            }
            else
            {
                return false; 
            }
        }
        public virtual void OnOpen() { }

        public void Close(NumberCardComponent numberCard)
        {
            Close(); 
        }
        
        public void Close()
        {
            if (CheckCloseCondition() && (tutorialVisual.activeSelf || bCanFinishBeforeActivate))
            {
                tutorialVisual.SetActive(false);
                OnClose();
                Delete(); 
            }
        }
        public virtual void OnClose() { }

        public void Delete()
        {
            if (CheckDeleteCondition())
            {
                bFinished = true; 
                onTutorialStepFinished.Invoke();
                OnDelete();
                foreach (UnityEvent finishEvent in GetCloseEvents())
                {
                    finishEvent.RemoveListener(Close);
                }
            }
        }
        public virtual void OnDelete() { }
        
        protected virtual List<UnityEvent> GetOpenEvents()
        {
            return new List<UnityEvent>(); 
        }

        protected virtual List<UnityEvent> GetCloseEvents()
        {
            return new List<UnityEvent>(); 
        }
        
        protected virtual bool CheckOpenCondition()
        {
            return true; 
        }
        
        protected virtual bool CheckCloseCondition()
        {
            return true; 
        }

        protected virtual bool CheckDeleteCondition()
        {
            return true; 
        }

        public static bool CheckEnemyFormable(PlayerHandComponent playerHand, EnemyZoneComponent enemyZone, out Tuple<int, int> cardAndEnemyIndex)
        {
            for (int enemyIndex = 0; enemyIndex < enemyZone.enemySlots.Length; enemyIndex++)
            {
                if (!enemyZone.enemySlots[enemyIndex].TryGetEnemy(out EnemyComponent enemy)) { continue; }

                bool bNumeratorExists = false;
                bool bDenominatorExists = false; 
                
                for (int cardIndex = 0; cardIndex < playerHand.cardSlots.Length; cardIndex++)
                {
                    if (!playerHand.cardSlots[cardIndex].TryGetNumber(out NumberCardComponent playerNumber)) { continue; }

                    if (playerNumber.IsFraction)
                    {
                        if (playerNumber.Value.EqualsExact(enemy.Value))
                        {
                            cardAndEnemyIndex = new Tuple<int, int>(cardIndex, enemyIndex); 
                        }
                    }
                    else
                    {
                        if (playerNumber.Value.Numerator == enemy.Value.Numerator)
                        {
                            bNumeratorExists = true; 
                        }
                        if (playerNumber.Value.Numerator == enemy.Value.Denominator)
                        {
                            bDenominatorExists = true; 
                        }

                        if (bNumeratorExists && bDenominatorExists)
                        {
                            cardAndEnemyIndex = new Tuple<int, int>(-1, enemyIndex); 
                            return true; 
                        }
                    }
                }
            }

            cardAndEnemyIndex = new Tuple<int, int>(-1, -1);
            return false; 
        }

        public static bool CheckCardMatchesEnemy(PlayerHandComponent playerHand, EnemyZoneComponent enemyZone, out Tuple<int, int> cardAndEnemyIndex)
        {
            return CheckCardMatchesEnemy(playerHand, enemyZone, out cardAndEnemyIndex, (fractionA, fractionB) => fractionA.Equals(fractionB)); 
        }
        
        public static bool CheckCardMatchesEnemy(PlayerHandComponent playerHand, EnemyZoneComponent enemyZone, out Tuple<int, int> cardAndEnemyIndex, Func<Fraction, Fraction, bool> matchCondition)
        {
            for (int cardIndex = 0; cardIndex < playerHand.cardSlots.Length; cardIndex++)
            {
                if (!playerHand.cardSlots[cardIndex].TryGetNumber(out NumberCardComponent number)) { continue; }
                
                for (int enemyIndex = 0; enemyIndex < enemyZone.enemySlots.Length; enemyIndex++)
                {
                    if (!enemyZone.enemySlots[enemyIndex].TryGetEnemy(out EnemyComponent enemy)) { continue; }
                    
                    if (matchCondition(number.Value, enemy.Value))
                    {
                        cardAndEnemyIndex = new Tuple<int, int>(cardIndex, enemyIndex); 
                        return true; 
                    } 
                }
            }

            cardAndEnemyIndex = new Tuple<int, int>(-1, -1); 
            return false; 
        }
    }
}
