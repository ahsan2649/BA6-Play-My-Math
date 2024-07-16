using System.Collections.Generic;
using System.Linq;
using Programming.Enemy;
using TMPro;
using UnityEngine;

namespace Programming.Visualisers
{
    public class UpcomingEnemyVisualiser : MonoBehaviour
    {
        [SerializeField] private FractionTextVisualiser textVisualiser;
        [SerializeField] private TMP_Text remainingEnemiesText;

        public void UpdateVisuals()
        {
            List<EnemyComponent> Enemies = EnemyLineupComponent.Instance.EnemiesInCurrentLineup; 
            textVisualiser.SetFraction(Enemies.Count > 0 ? Enemies.First().Value : null);
            remainingEnemiesText.text = Enemies.Count.ToString(); 
        }
    }
}
