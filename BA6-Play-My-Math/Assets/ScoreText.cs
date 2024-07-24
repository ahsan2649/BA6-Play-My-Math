using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Programming.Enemy;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScoreLabel()
    {
        textMeshProUGUI.text = "Score: " + Score.getScore();
    }
}
