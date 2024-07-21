using System;
using System.Collections;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Enemy
{
    public class EnemyComponent : MonoBehaviour
    {
        [SerializeField] Fraction value;

        [SerializeField] TextMeshProUGUI zahler;
        [SerializeField] TextMeshProUGUI nenner;

        [FormerlySerializedAs("onEnemyChange")] public UnityEvent onValueChange; 
        
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        public Fraction Value
        {
            get => value;
            set
            {
                this.value = value;
                onValueChange.Invoke();
            }
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

        public IEnumerator MoveToSpot(Transform spot)
        {
            while (Vector3.Distance(transform.position, spot.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, spot.position, 15f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}


