using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Visualisers
{
    public class EnemySlotVisualiser : MonoBehaviour
    {
        [SerializeField] private EnemySlotComponent enemySlot;
        [SerializeField] private FractionVisualiser.FractionVisualiser visualiser;

        public void VisualiseEnemy()
        {
            visualiser.SetSingleFractionValue(enemySlot.GetEnemy()?.Value, OperandType.Left);
            visualiser.FullUpdateVisualisations();
        }
    }
}
