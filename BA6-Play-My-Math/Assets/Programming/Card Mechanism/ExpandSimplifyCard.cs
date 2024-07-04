using Programming.Card_Mechanism;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpandSimplifyCard : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public GameObject expandSimpObj;
    public GameObject hint;
    Transform parentSlot;

    bool giveHint = true;
    bool focusOpen = false;

    public GameObject btnExpand;
    public GameObject btnSimplify;
    public Color btnActiveColor;

    bool bExpand;


    void Start()
    {
        expand();
        bExpand = true;
        expandSimpObj.SetActive(false);
        hint.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       if(focusOpen == false)
        {
            //move cards to the center
            giveHint = false;
            expandSimpObj.SetActive(true);
            parentSlot = transform.parent;
            transform.parent = GameObject.FindGameObjectWithTag("Test").transform;

            transform.position = new Vector3(0, 6, -5);
            transform.localScale += new Vector3(.02f, .02f, .02f);
            focusOpen = true;
        }

    }
    public void OnPointerUp(PointerEventData eventData)
    {
       
    }
 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (giveHint)
        {
            hint.SetActive(true);
        }
 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hint.SetActive(false);
    }

    public void returnToHand()
    {
        focusOpen = false;
        giveHint = true;
        transform.SetParent(parentSlot);
        transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
        transform.localPosition = new Vector3(0, 0, 0);
        expandSimpObj.SetActive(false);
    }

    public void expand()
    {
        bExpand = true;
        btnExpand.GetComponent<Image>().color = btnActiveColor;
        btnSimplify.GetComponent<Image>().color = Color.white;
    }
    public void simplify()
    {
        bExpand = false;
        btnSimplify.GetComponent<Image>().color = btnActiveColor;
        btnExpand.GetComponent<Image>().color = Color.white;
    }

    public void ExSimpl (int pValue)
    {
        //doesnt work
        if (bExpand) GetComponent<NumberCardComponent>().Value.ExpandBy(pValue);
        else GetComponent<NumberCardComponent>().Value.SimplifyBy(pValue);
    }

  
}
