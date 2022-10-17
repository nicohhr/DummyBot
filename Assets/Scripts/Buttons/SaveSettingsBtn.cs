using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSettingsBtn : MonoBehaviour
{
    // Instancia de botao 
    [SerializeField] public Button button;

    public void action()
    {
        SettingsManager.Save();
        button.interactable = false; 
    }

}
