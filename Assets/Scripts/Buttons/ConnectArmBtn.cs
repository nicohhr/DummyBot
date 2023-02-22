using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.IO;
using ColorUtility = UnityEngine.ColorUtility;

public class ConnectArmBtn : MonoBehaviour
{

    [SerializeField] public Button button;
    public static Button btn;
    private static Color lastColor = new Color(0.1254902F, 0.3098039F, 0.4588236F, 0.3411765F);
    private static Color greenColor = new Color(0.2834483F, 0.8113208F, 0.2347217F, 0.5F);

    public void PressButton()
    {
        if ((SerialConnection.serialPort == null || !SerialConnection.serialPort.IsOpen) && SerialPort.GetPortNames().Length > 0)
        {
            try
            {
                // Inicializando conexão
                SerialConnection.Start();
                //ColorBlock cb = button.colors;

                // Alterando Cor
                button.image.color = greenColor;
            }
            catch (IOException)
            {
                Debug.Log("Non COM Port Selected");
                button.interactable = false;
                ConnectArmBtn.TurnWhite();
            }

        } 
        else if ((SerialConnection.serialPort != null && SerialConnection.serialPort.IsOpen))
        {
            // Finalizando conexão
            SerialConnection.Stop();

            // Alterando Cor
            button.image.color = lastColor;
        }
    }

    public static void TurnWhite()
    {
        btn.image.color = lastColor;
    }

    public static void TurnGreen()
    {
        btn.image.color = greenColor;
    }

    public static void MakeNonIteracteble()
    {
        btn.interactable = false;
        ConnectArmBtn.TurnWhite();
    }
    
    public static void MakeInteracteble()
    {
        btn.interactable = true;
    }

    private void Start()
    {
        // Passando botão para variável estática
        btn = button;
    }

    private void Update()
    {
        if (SerialConnection.hasStarted)
        {
            TurnGreen();
        } else
        {
            ConnectArmBtn.TurnWhite();
        }
    }
}
