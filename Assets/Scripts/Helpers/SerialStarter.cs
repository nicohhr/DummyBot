using System.IO.Ports;
using UnityEngine;

public class SerialStarter : MonoBehaviour
{
    private string[] ports;

    void Start()
    {
        // Recuperando lista de portas seriais disponíveis
        ports = SerialPort.GetPortNames();

        // Conferindo se há portas disponíveis e selecionando primeiro valor da lista
        if (ports.Length > 0) SerialConnection.COMPort = ports[0];

    }
}
 