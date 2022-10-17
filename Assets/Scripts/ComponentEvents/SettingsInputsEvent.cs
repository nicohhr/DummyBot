using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsInputsEvent : MonoBehaviour
{

    // Armazenando componente de campo de entrada
    [SerializeField] Button button;

    /// <summary>
    /// M�todo a ser executado no momento de mudan�a de atividade no campo
    /// </summary>
    public void onValueChange()
    {
        // Reativando bot�o
        button.interactable = true;
    }

}
