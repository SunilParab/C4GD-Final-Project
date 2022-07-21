using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dirToMouse = (Input.mousePosition - posInScreen);
        transform.position = dirToMouse;
    }
}
