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
        expand();
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

    public void returnToHand()
    {
        Debug.Log("return tohand");
        focusOpen = false;
        giveHint = true;
        transform.SetParent(parentSlot);
        transform.localScale = new Vector3(0.016f, 0.016f, 0.016f);
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

        //set other buttons

    }

    public void ExSimpl(int pValue)
    {

        //doesnt work
        if (bExpand)
        {
            Debug.Log("expand or simplify" + pValue);
            GetComponent<NumberCardComponent>().Value = GetComponent<NumberCardComponent>().Value.ExpandBy(pValue);

        }
        else GetComponent<NumberCardComponent>().Value = GetComponent<NumberCardComponent>().Value.SimplifyBy(pValue);
    }

    public void refreshSimplifyButtons()
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
