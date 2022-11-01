using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelSettingsBtn : MonoBehaviour
{
    [SerializeField] public Button Button;
    public void OnAction() 
    { 
        SettingsManager.SetComponentsText(); 
    }
}
