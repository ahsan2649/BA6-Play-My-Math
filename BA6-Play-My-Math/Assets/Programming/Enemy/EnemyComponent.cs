using System;
using System.Collections;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Enemy
{
    public class EnemyComponent : MonoBehaviour
    {
        [SerializeField] Fraction value;

        [SerializeField] TextMeshProUGUI zahler;
        [SerializeField] TextMeshProUGUI nenner;
        
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        public Fraction Value
        {
            get => value;
            set => this.value = value;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
        }

        public void UpdateDisplay()
        {
            zahler.text = Value.Numerator.ToString();
            nenner.text = Value.Denominator.ToString();
        }

        public IEnumerator MoveToNewParent()
        {
            while (Vector3.Distance(transform.position, transform.parent.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, 0.15f);
                yield return null;
            }
        }
    }
}


