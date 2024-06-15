using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class FractionDisplay : MonoBehaviour
{
    [FormerlySerializedAs("card")] public CardInfo cardInfo;
    void Awake()
    {
        if(cardInfo.value.Denominator == 1)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Numerator.ToString();
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Numerator.ToString();
            transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = cardInfo.value.Denominator.ToString();
        }

    }


}
