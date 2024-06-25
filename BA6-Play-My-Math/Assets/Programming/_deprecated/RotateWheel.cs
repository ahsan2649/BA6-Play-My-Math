using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Mathematics;

public class RotateWheel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject wheel;

 
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("blaa");
        float mousePosY = Input.GetAxis("Mouse Y") * 50;
        wheel.transform.Rotate(new Vector3(0,1,0), -mousePosY, Space.Self);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float rotationx = wheel.transform.localEulerAngles.x; 
        if(rotationx >= 360)
        {
            rotationx -= 360;
        }
        
        Debug.Log(rotationx);
        if (rotationx >= 0 && rotationx <= 45)
        {
            Debug.Log("-");
            //minus
            wheel.transform.localRotation = Quaternion.Euler(45, 0, 90);

        }
        else
        if (rotationx > 45 && rotationx <= 180)
        {
            //division
            Debug.Log("/");
            wheel.transform.localRotation = Quaternion.Euler(135, 0, 90);
        }
        else if (rotationx > 180  && rotationx <= 270)
        {
            //multiplication 
            Debug.Log("*");
            wheel.transform.localRotation = Quaternion.Euler(225, 0, 90);
        }
        else if (rotationx > 270 && rotationx <= 360)
        {
            //plus
            Debug.Log("+");
            wheel.transform.localRotation = Quaternion.Euler(315, 0, 90);
        }
    }


}
