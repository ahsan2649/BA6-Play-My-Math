using System;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.EventSystems;

//TODO: @Frieda: this class is already added to EnemyDisplayArea and can be used in teacherMode to change the value of the enemy (though not yet tested whether the visualisation updates correctly)

namespace Programming.Enemy
{
    public class EnemyChangeCanvas : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject hitBox;
        [SerializeField] private GameObject changeCanvas; 
        [SerializeField] private EnemySlotComponent enemySlotComponent;

        private void Awake()
        {
            if (hitBox is null)
            {
                hitBox = this.gameObject; 
            }
        }

        public void OnPointerClick(PointerEventData eventData) //triggers upon actually clicking on the Canvas
        {
            changeCanvas.SetActive(!changeCanvas.activeSelf);
            //move cards to the center
         
        }

        public void SetEnemyValue(Fraction fraction)
        {
            if (!enemySlotComponent.TryGetEnemy(out EnemyComponent enemy))
            {
                return; 
            }

            enemy.Value = fraction; 
        }
        
        public void SetEnemyNumerator(int value)
        {
            if (!enemySlotComponent.TryGetEnemy(out EnemyComponent enemy))
            {
                return; 
            }

            enemy.Value = new Fraction(value, enemy.Value.Denominator); 
        }

        public void SetEnemyDenominator(int value)
        {
            if (!enemySlotComponent.TryGetEnemy(out EnemyComponent enemy))
            {
                return; 
            }

            enemy.Value = new Fraction(enemy.Value.Numerator, value); 
        }
    }
}
