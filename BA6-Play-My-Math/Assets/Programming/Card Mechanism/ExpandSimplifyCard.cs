using Programming.Card_Mechanism;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpandSimplifyCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject expandSimpObj;
    public GameObject hint;
    Transform parentSlot;

    bool giveHint = true;
    bool focusOpen = false;

    public GameObject btnExpand;
    public GameObject btnSimplify;
    public Color btnActiveColor;

    public GameObject btn2;
    public GameObject btn3;
    public GameObject btn5;
    public GameObject btn7;
    public Color btnIncorrectColor;


    bool bExpand;


    void Start()
    {
        Expand();
        bExpand = true;
        expandSimpObj.SetActive(false);
        hint.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (focusOpen == false && !GetComponent<NumberCardComponent>().Value.IsWhole())
        {
            //move cards to the center
            giveHint = false;
            expandSimpObj.SetActive(true);
            parentSlot = transform.parent;
            transform.parent = GameObject.FindGameObjectWithTag("Test").transform;

            transform.position = new Vector3(0, 6, -5);
            transform.localScale += new Vector3(.02f, .02f, .02f);
            transform.rotation = Quaternion.Euler(90, 0, 0);
            focusOpen = true;
            
            GetComponent<CardMovementComponent>().enabled = false; 
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (giveHint && !GetComponent<NumberCardComponent>().Value.IsWhole())
        {
            hint.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hint.SetActive(false);
    }

    public void ReturnToPreviousSlot()
    {
        Debug.Log("return toPreviousSlot");
        focusOpen = false;
        giveHint = true;
        transform.SetParent(parentSlot);
        transform.localScale = new Vector3(0.016f, 0.016f, 0.016f);
        transform.localPosition = new Vector3(0, 0, 0);
        expandSimpObj.SetActive(false);
        
        GetComponent<CardMovementComponent>().enabled = true; 
    }

    public void Expand()
    {
        bExpand = true;
        btnExpand.GetComponent<Image>().color = btnActiveColor;
        btnSimplify.GetComponent<Image>().color = Color.white;
    }
    public void Simplify()
    {
        bExpand = false;
        btnSimplify.GetComponent<Image>().color = btnActiveColor;
        btnExpand.GetComponent<Image>().color = Color.white;

        //set other buttons

        //TODO @Frieda: call FractionTextVisualiser.DisplayDecimals
    }

    public void ExSimpl(int pValue)
    {
        if (bExpand)
        {
            Debug.Log("expand or simplify" + pValue);
            NumberCardComponent numberCard = GetComponent<NumberCardComponent>(); 
            numberCard.Value = GetComponent<NumberCardComponent>().Value.ExpandBy(pValue);
            
        }
        else GetComponent<NumberCardComponent>().Value = GetComponent<NumberCardComponent>().Value.SimplifyBy(pValue);
    }

    public void RefreshSimplifyButtons()
    {

        btn2.GetComponent<Image>().color = Color.white; btn3.GetComponent<Image>().color = Color.white; btn5.GetComponent<Image>().color = Color.white; btn7.GetComponent<Image>().color = Color.white;
        if (!bExpand)
        {
            if (!GetComponent<NumberCardComponent>().Value.CanSimplifyBy(2)) btn2.GetComponent<Image>().color = btnIncorrectColor;
            if (!GetComponent<NumberCardComponent>().Value.CanSimplifyBy(3)) btn3.GetComponent<Image>().color = btnIncorrectColor;
            if (!GetComponent<NumberCardComponent>().Value.CanSimplifyBy(5)) btn5.GetComponent<Image>().color = btnIncorrectColor;
            if (!GetComponent<NumberCardComponent>().Value.CanSimplifyBy(7)) btn7.GetComponent<Image>().color = btnIncorrectColor;

        }


    }


}
