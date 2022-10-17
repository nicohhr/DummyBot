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
    /// Método a ser executado no momento de mudança de atividade no campo
    /// </summary>
    public void onValueChange()
    {
        // Reativando botão
        button.interactable = true;
    }

}
