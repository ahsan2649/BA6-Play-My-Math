using Programming.Enemy;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.TeacherMode
{
    //attach to EnemySlotComponent -> onClick has to set the enemyComponent
    public class EnemySlotFractionChanger : FractionChanger
    {
        [SerializeField] private EnemySlotComponent enemySlotComponent; 
        private EnemyComponent enemyComponent;

        [SerializeField] private TMP_Text numeratorText; 
        [SerializeField] private TMP_Text denominatorText; 
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!enemySlotComponent.GetEnemy())
            {
                return; 
            }
            enemyComponent = enemySlotComponent.GetEnemy();
            numeratorText.text = enemyComponent.Value.Numerator.ToString(); 
            denominatorText.text = enemyComponent.Value.Denominator.ToString(); 
            
            base.OnPointerClick(eventData);
        }

        public override void FinaliseInput()
        {
            enemyComponent.Value = new Fraction(int.Parse(numeratorText.text), int.Parse(denominatorText.text)); 
        }

        public override Fraction GetChangeableFraction()
        {
            return enemyComponent.Value; 
        }
    }
}
