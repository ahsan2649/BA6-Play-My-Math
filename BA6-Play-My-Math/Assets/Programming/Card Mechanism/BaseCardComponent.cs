using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Programming.Card_Mechanism {
    public class BaseCardComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;

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
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, 0.15f);
                yield return null;
            }

            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator RotateToNewParent()
        {
            while (Quaternion.Angle(transform.rotation, transform.parent.rotation) > 0.1f)
            {
                _canvasGroup.blocksRaycasts = false;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, 2f);
                yield return null;
            }

            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator DiscardAnimation()
        {
            while (transform.localScale.magnitude > 0.1f)
            {
                _canvasGroup.blocksRaycasts = false;
                transform.localScale *= 0.95f;
                yield return null;
            }
        }

        #endregion
    }
}