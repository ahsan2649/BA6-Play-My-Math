using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Visualisers
{
    public class EnemySlotVisualiser : MonoBehaviour
    {
        [SerializeField] private EnemySlotComponent enemySlot;
        [FormerlySerializedAs("visualiser")] [SerializeField] private FractionVisualizer visualizer;

        public void VisualiseEnemy()
        {
            visualizer.SetSingleFractionValue(enemySlot.GetEnemy()?.Value, OperandType.Left);
            visualizer.FullUpdateVisualisations();
        }
    }
}
