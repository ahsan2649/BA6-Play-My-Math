using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsSlider : MonoBehaviour {
    public RectTransform Slider;
    public RectTransform Counter;
    public TextMeshProUGUI Count;

    [SerializeField] private List<int> thresholdValues = new();
    [SerializeField] private List<GameObject> thresholds = new();
    public int maxValue;

    public float value;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountRewards(12));
        StartCoroutine(SlideRewards(12));

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnValidate()
    {
        
        for (int i = 0; i < thresholdValues.Count; i++)
        {
            thresholdValues[i] = Mathf.Clamp(thresholdValues[i], 0, maxValue);
        }
    }

    public IEnumerator CountRewards(int target)
    {
        while (value < target)
        {
            value++;
            Count.text = value.ToString();
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public IEnumerator SlideRewards(int target)
    {
        var targetPos = (target / maxValue) * Slider.sizeDelta.x;
        while (Counter.anchoredPosition.x < targetPos)
        {
            Counter.anchoredPosition += new Vector2(0.1f, 0);
            yield return new WaitForSeconds(0.005f);
        }
    }
}