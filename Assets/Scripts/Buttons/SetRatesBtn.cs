using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRatesBtn : MonoBehaviour
{
    [SerializeField] Button button;
    
    public static void OnAction()
    {
        // Atualizar valor dos sliders
        SettingsManager.sFkRateSlider.value = float.Parse(SettingsManager.sFkRateField.text);
        SettingsManager.sIkRateSlider.value = float.Parse(SettingsManager.sIkRateField.text);
    }
}
