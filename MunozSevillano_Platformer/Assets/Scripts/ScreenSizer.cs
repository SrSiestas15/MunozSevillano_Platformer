using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(640, 360, true);
        Screen.fullScreen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
