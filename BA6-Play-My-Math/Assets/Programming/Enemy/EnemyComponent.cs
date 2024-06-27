using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Enemy
{
    public class EnemyComponent : MonoBehaviour
    {
        [SerializeField] Fraction value;

        public Fraction Value
        {
            get => value;
            set => this.value = value;
        }
    }
}


