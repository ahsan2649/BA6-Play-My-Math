using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class CardDisplay : MonoBehaviour
{
    [FormerlySerializedAs("card")] public CardInfo cardInfo;

    [SerializeField] GameObject highlight; 
    void Awake()
    {
        switchHighlight();
        if(cardInfo.value.Denominator == 1)
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Numerator.ToString();
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Numerator.ToString();
            transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Denominator.ToString();
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
