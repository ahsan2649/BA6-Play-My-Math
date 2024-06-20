using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.UX_UI
{
    public class NumberCardDisplay : CardDisplay
    {
        private IFractionableCard cardBaseFraction;

        [SerializeField] private TMP_Text numeratorText;
        [SerializeField] private TMP_Text vinculum; 
        [SerializeField] private TMP_Text denominatorText;
        
        public override void UpdateVisual()
        {
            cardBaseFraction = (IFractionableCard) cardBaseObject;

            numeratorText.SetText(cardBaseFraction.GetValue().Numerator.ToString());
            denominatorText.SetText(cardBaseFraction.GetValue().Denominator.ToString());

            if(cardBaseFraction.GetValue().Denominator == 1)
            {
                numeratorText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
                vinculum.gameObject.SetActive(false);
                denominatorText.gameObject.SetActive(false);
            }
        }

        public override string GetName()
        {
            return numeratorText.text + "/" + denominatorText.text;
        }
    }
}
