using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("drop");
        GameObject dropped = eventData.pointerDrag;
        DragDrop dragDrop = dropped.GetComponent<DragDrop>();
        dragDrop.parentAfterDrag = transform;
       
    }

 
}
