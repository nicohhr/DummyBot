using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPosWriter : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textMesh;
    public Transform endEffector; 

    // Update is called once per frame
    void Update()
    {
        if (TabManager.selectedTabIndex == 1 && PythonCommunication.isConnected)
        {
            textMesh.text = "X: " + Arm_Controller.floatToString(-endEffector.position.x) + "\nY: " + Arm_Controller.floatToString((endEffector.position.z)) + "\nZ: " + Arm_Controller.floatToString(endEffector.position.y);
        }
    }
}
