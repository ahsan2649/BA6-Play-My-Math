using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace Programming.Card_Mechanism {
    public class CardMovementComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;

        [SerializeField] private float MoveDelta, RotateDelta, MoveSpeed, RotateSpeed, ScaleSpeed;

        public UnityEvent onCardChange; 
        
        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
        }

        #region Drag

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _canvas.worldCamera, out var pos);
            _rectTransform.position = _canvas.transform.TransformPoint(pos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StartCoroutine(MoveToNewParent());
            StartCoroutine(RotateToNewParent());
            _canvasGroup.blocksRaycasts = true;
        }

        #endregion

        #region Animations

        public IEnumerator MoveToNewParent()
        {
            while (Vector3.Distance(transform.position, transform.parent.position) > 0.01f)
            {
                _canvasGroup.blocksRaycasts = false;
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, MoveDelta);
                yield return new WaitForSeconds(1f/MoveSpeed);
            }

            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator RotateToNewParent()
        {
            while (Quaternion.Angle(transform.rotation, transform.parent.rotation) > 0.1f)
            {
                _canvasGroup.blocksRaycasts = false;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, RotateDelta);
                yield return new WaitForSeconds(1f/RotateSpeed);
            }

            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator DiscardAnimation()
        {
            while (transform.localScale.magnitude > 0.1f)
            {
                _canvasGroup.blocksRaycasts = false;
                transform.localScale *= 0.95f;
                yield return new WaitForSeconds(1f/ScaleSpeed);
            }
        }

        #endregion
    }
}