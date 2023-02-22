using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

public static class SerialConnection
{
    public static string COMPort;
    public static int BaudRate;
    public static string[] data;
    public static bool hasStarted = false;

    [HideInInspector] public static SerialPort serialPort;
    private static Thread sThread;

    /// <summary>
    /// Converte valor de 0 a 180 para resolução de 0 a 255
    /// </summary>
    public static int conv(float x)
    {
        float in_min = 0;
        float in_max = 180;
        float out_min = 0;
        float out_max = 255; 

        float convAux = ((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
        return (int)Mathf.Clamp(convAux, out_min, out_max);
    }

    /// <summary>
    /// Inicializando conexao
    /// </summary>
    public static void Start()
    {
        if (!hasStarted)
        {
            Debug.Log("Port: " + COMPort + " BaudRate: " + BaudRate.ToString());
            serialPort = new SerialPort(COMPort, 115200);
            serialPort.Open();

            Debug.Log("Serial Started");
            ThreadStart ts = new ThreadStart(SendPositions);
            sThread = new Thread(ts);
            sThread.Start();
            hasStarted = true; 
        }
    }

    /// <summary>
    /// Enviando posição das juntas
    /// </summary>
    /// <param name="servoPos">Posição d ecada junta de 0 a 180</param>
    public static void SendJointPositions(float[] servoPos)
    {
        try
        {
            // Inicializar array de bytes com cabechalho informação e terminador de mensagem 
            byte[] msg = {79, 80, (byte)conv(servoPos[0]), (byte)conv(servoPos[1]), (byte)conv(servoPos[2]), (byte)conv(servoPos[3]), 90, 90, 69, 68};

            // Enviar mensagem para o controlador
            serialPort.Write(msg, 0, 10);

            // Printando valores enviados
            string auxOut = string.Empty;

            foreach (byte num in msg)
            {
                auxOut += num.ToString() + " | ";
            }

            Debug.Log("Serial Data: " + auxOut);
        }
        catch (IOException)
        {
            Debug.Log("Arm Disconnected");
            Stop();
        }
    }

    private static void SendPositions()
    {
        while(serialPort != null && serialPort.IsOpen)
        {
            SendJointPositions(Arm_Controller.jointPositions);
            Thread.Sleep(20);
        }
        hasStarted = false;
    }

    /// <summary>
    /// Finaliza conexão
    /// </summary>
    public static void Stop()
    {
        hasStarted = false;
        serialPort.Close();
    }
}
