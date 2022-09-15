using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextScript : MonoBehaviour
{
    TMP_InputField depthInput;

    public void InputName() {
        string str = depthInput.text;

        try {
            int depth = Int32.Parse(str);
            AIAlgorithms.depthOfTree = depth;
            Debug.Log(depth);
        }
        catch (FormatException) {
            AIAlgorithms.depthOfTree = -1;
            Debug.Log($"Unable to parse '{str}'");
        }

        //depthInput.text = "";

    }

    // Start is called before the first frame update
    void Start()
    {
        depthInput = GameObject.Find("DepthOfTree").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
