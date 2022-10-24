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
            //lastColor = cb.normalColor;
            //cb.normalColor = new Color(36F, 152F, 49F, 0.5F);
            lastColor = button.image.color;
            button.image.color = new Color(0.2834483F, 0.8113208F, 0.2347217F, 0.75F);

        } else
        {
            // Finalizando conexão
            SerialConnection.Stop();

            // Alterando Cor
            //ColorBlock cb = button.colors;
            //cb.normalColor = lastColor;
            button.image.color = lastColor;
        }
    }

}
