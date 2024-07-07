using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * [PLEASE IGNORE THIS SCRIPT]
 * THIS CODE IS ONLY ADDED TO MEASURE THE FPS PERFORMANCE OF THE GAME WHILE IN GAME PLAY MODE
 * THIS CODE BELONGS TO UNITY (C) TECHNOLOGIES.
 * IT IS NOT OURS.
 */

public class DeviceTypeUI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        string deviceName = SystemInfo.graphicsDeviceName;
        string deviceTypeName = SystemInfo.graphicsDeviceType.ToString();
        GetComponent<Text>().text = deviceName + "\n" + deviceTypeName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
