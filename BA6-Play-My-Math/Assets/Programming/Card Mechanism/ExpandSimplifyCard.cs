using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandSimplifyCard : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject expandSimpObj;
    public GameObject hint;



    public void OnPointerDown(PointerEventData eventData)
    {
        expandSimpObj.SetActive(true);
        transform.parent = GameObject.FindGameObjectWithTag("Test").transform;

        transform.position = new Vector3(0,3,0);
        transform.localScale += new Vector3(.02f, .02f, .02f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       hint.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hint.SetActive(false);
    }



    // Start is called before the first frame update
    void Start()
    {
        expandSimpObj.SetActive(false);
        hint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
