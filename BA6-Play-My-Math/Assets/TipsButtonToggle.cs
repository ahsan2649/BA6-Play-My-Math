using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Programming.Enemy;
using UnityEngine.SocialPlatforms.Impl;

public class TipsButtonToggle : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    bool active;

    public void toggleCanvasActive(GameObject canvas)
    {
        if (active)
        {
            canvas.SetActive(false);
            active = false;
        }
        else
        {
            canvas.SetActive(true);
            textMeshProUGUI.text = "Score: " + Score.getScore();
            active = true;
            
        }
    }
}
