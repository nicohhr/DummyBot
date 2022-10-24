using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConnectArmBtn : MonoBehaviour
{

    [SerializeField] public Button button;
    private Color lastColor;

    public void PressButton()
    {
        if (SerialConnection.serialPort == null || !SerialConnection.serialPort.IsOpen)
        {
            // Inicializando conexão
            SerialConnection.Start();
            ColorBlock cb = button.colors;

            // Alterando Cor
            lastColor = button.image.color;
            button.image.color = new Color(0.2834483F, 0.8113208F, 0.2347217F, 0.5F);

        } 
        else
        {
            // Finalizando conexão
            SerialConnection.Stop();

            // Alterando Cor
            button.image.color = lastColor;
        }
    }
}
