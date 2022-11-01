using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBtn : MonoBehaviour
{
    [SerializeField] public Button button;

    public static void OnAction()
    {
        ComDropdown.UpdateSerialPorts();
    }
}
