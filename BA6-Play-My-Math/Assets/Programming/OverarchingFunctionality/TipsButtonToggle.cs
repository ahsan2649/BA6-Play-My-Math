using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsButtonToggle : MonoBehaviour
{
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
            active = true;
        }
    }
}
