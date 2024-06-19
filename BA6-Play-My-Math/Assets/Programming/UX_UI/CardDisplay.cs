using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Programming.Card_Mechanism;

public class CardDisplay : MonoBehaviour
{

    [SerializeField] GameObject highlight;

    public NumberCard thisCard;
    void Awake()
    {
        switchHighlight();
        if(thisCard.GetValue().Denominator == 1)
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = thisCard.GetValue().Numerator.ToString();
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = thisCard.GetValue().Numerator.ToString();
            transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = thisCard.GetValue().Denominator.ToString(); 
        }

    }
    

    public void switchHighlight()
    {
        if(highlight.active == true)
        {
            highlight.SetActive(false);
        }
        else
        {
            highlight.SetActive(true);
        }
    }


}
