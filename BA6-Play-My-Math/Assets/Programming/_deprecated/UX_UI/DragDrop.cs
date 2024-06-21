using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public Transform parentAfterDrag;

    private RectTransform rectTransform;
    private Vector3 offset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.parent.transform.parent);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        offset = transform.position - GetHitPoint();

        //highlight number cards  (needs cards )
        //if this is not a fraction, highlight all cards in hand that are not a fraction
    }

    public void OnDrag(PointerEventData eventData)
    {
        // rectTransform.anchoredPosition += eventData.delta;
        transform.position = GetHitPoint() + offset;


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);



        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "CardSlot")
            {
                transform.SetParent(hit.transform);
            }
            else if(hit.transform.tag == "Card")
            {
                transform.SetParent(hit.transform);
                //createfraction (still needs the card class )
            }
            else if (hit.transform.tag == "OperationBoardSlot")
            {
                // if card is a fraction set parent that slot otherwise go back to former parent
                transform.SetParent(hit.transform);
            }
            else if (hit.transform.tag == "Bin")
            {
                // if card is a fraction set parent that slot otherwise go back to former parent
                transform.SetParent(hit.transform);
            }
            else
            {
                transform.SetParent(parentAfterDrag);
            }
        }
        transform.localPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        image.raycastTarget = true;
    }

    Vector3 GetHitPoint()
    {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
}
 

