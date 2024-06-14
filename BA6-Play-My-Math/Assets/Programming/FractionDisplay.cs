using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FractionDisplay : MonoBehaviour
{
    public Card card;
    void Awake()
    {
        if(card.value.Denominator == 0)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = card.value.Numerator.ToString();
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = card.value.Numerator.ToString();
            transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = card.value.Denominator.ToString();
        }

    }


}
