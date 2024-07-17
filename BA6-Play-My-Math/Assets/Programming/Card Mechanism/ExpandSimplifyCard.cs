using Programming.Card_Mechanism;
using Programming.Visualisers;
using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
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

    public GameObject fractionTextVisualizer;

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
        if (!GetComponent<NumberCardComponent>().IsFraction)
        {
            return; 
        }

        if (focusOpen)
        {
            return; 
        }
        
        //move cards to the center
        giveHint = false;
        expandSimpObj.SetActive(true);
        parentSlot = transform.parent;
        transform.parent = GameObject.FindGameObjectWithTag("Test").transform;

        transform.localPosition = new Vector3(0, 6, -5);
        transform.localScale += new Vector3(.02f, .02f, .02f);
        transform.localRotation = Quaternion.Euler(90, 0, 0);
        focusOpen = true;
        
        GetComponent<CardMovementComponent>().enabled = false;
        transform.DoForAllDescendants(descendant => descendant.gameObject.layer = LayerMask.NameToLayer("3D_WorldSpace_UI"));
        // gameObject.layer = LayerMask.NameToLayer("3D_WorldSpace_UI"); //ZyKa!
        // foreach (Transform child in transform)
        // {
        //     child.gameObject.layer = LayerMask.NameToLayer("3D_WorldSpace_UI"); 
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (giveHint && GetComponent<NumberCardComponent>().IsFraction)
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
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        GetComponent<CardMovementComponent>().enabled = true; 
        
        transform.DoForAllDescendants(descendant => descendant.gameObject.layer = LayerMask.NameToLayer("UI"));
        // gameObject.layer = LayerMask.NameToLayer("UI"); //ZyKa!
        // foreach (Transform child in transform)
        // {
        //     child.gameObject.layer = LayerMask.NameToLayer("UI"); 
        // }
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
    }

    public void ExSimpl(int pValue)
    {
        NumberCardComponent numberCard = GetComponent<NumberCardComponent>();
        if (numberCard is null)
        {
            return; 
        }
        
        if (bExpand)
        {
            Debug.Log("expand or simplify" + pValue);
            numberCard.oldValue = numberCard.Value = numberCard.Value.ExpandBy(pValue); 
        }
        else
        {
            numberCard.oldValue = numberCard.Value = numberCard.Value.SimplifyBy(pValue);
        }
    }

    public void RefreshSimplifyButtons()
    {
        NumberCardComponent numberCardComponent = GetComponent<NumberCardComponent>();
        if (numberCardComponent is null)
        {
            return; 
        }
        
        btn2.GetComponent<Image>().color = Color.white; btn3.GetComponent<Image>().color = Color.white; btn5.GetComponent<Image>().color = Color.white; btn7.GetComponent<Image>().color = Color.white;
        if (!bExpand)
        {
            if (!numberCardComponent.Value.CanSimplifyBy(2)) btn2.GetComponent<Image>().color = btnIncorrectColor;
            if (!numberCardComponent.Value.CanSimplifyBy(3)) btn3.GetComponent<Image>().color = btnIncorrectColor;
            if (!numberCardComponent.Value.CanSimplifyBy(5)) btn5.GetComponent<Image>().color = btnIncorrectColor;
            if (!numberCardComponent.Value.CanSimplifyBy(7)) btn7.GetComponent<Image>().color = btnIncorrectColor;
        }
    }
}
