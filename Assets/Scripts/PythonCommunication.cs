using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System;
using Assets.Scripts;

public enum ConnectionMode
{
    FK,      // Forward Kinematics
    IK       // Inverse Kinematics 
}

public class PythonCommunication : MonoBehaviour
{
    private const int PACKET_SIZE_BYTES = 48;
    private const int PACKET_SIZE = 12;
    
    #region Viariaveis 

    private static Thread sThread;
    private static string connectionIP = "127.0.0.1";
    public static int connectionPort = 25001;
    static IPAddress localAdress;
    static TcpListener listener;
    static TcpClient client;

    static private float[] armVariables = new float[PACKET_SIZE];
    static public bool isConnected = false;
    private static bool hasStarted = false;
    public static bool validPositions;

    #endregion

    #region Metodos

    private static void SetConnection()
    {
        localAdress = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start(); 
        client = listener.AcceptTcpClient();
        isConnected = true; 
    }

    private static float[] String2Array(string inputString)
    {
        // Declarando vetor se saida
        float[] result = new float[inputString.Length];

        // Sperando elementos da entrada
        string[] sArray = inputString.Split(',');

        // Alocando elementos em array de floats
        for (int i = 0; i < sArray.Length; i++) { result[i] = float.Parse(sArray[i]); }

        return result;
    }

    private static void GetFloatArray()
    {
        SetConnection();

        while (isConnected)
        {
            try
            {
                // Recebendo dados: 12 numeros float, 4 bytes por numero 48 bytes por pacote
                NetworkStream netStream = client.GetStream();
                byte[] buffer = new byte[PACKET_SIZE_BYTES];
                netStream.Read(buffer, 0, PACKET_SIZE_BYTES);

                // Convertendo array de bytes em array de floats
                for (int i = 0; i < PACKET_SIZE; i++) { armVariables[i] = BitConverter.ToSingle(buffer, (i * 4)); }

                // Atualizando variaveis 
                UpdateArmVariables();
            }
            catch (Exception)
            {
                StopConnection();
            } 
        }
    }

    private static void UpdateArmVariables()
    {
        // Indica validade das posicoes
        validPositions = false;

        // Conferindo se calculo de IK é válido (inválido com todas as juntas em 0)
        for (int i = 0; i < 6; i++)
        {
            if (armVariables[i] != 0)
            {
                validPositions = true;
                break;
            } 
        }

        // Em caso das posicoes serem validas
        if (validPositions)
        {
            // Atualizando posições (primeiros 6 valores da entrada)
            Arm_Controller.pythonPositions.isLocked = true;
            for (int i = 0; i < 4; i++) { Arm_Controller.pythonPositions.positions[i] = armVariables[i]; }
            Arm_Controller.pythonPositions.isLocked = false;
        }

        // Exibindo resultados
        string outputText = string.Empty;
        for (int i = 0; i < armVariables.Length; i++)
        {
            outputText += armVariables[i] + " | ";
        }

        outputText += "\n";
        print(outputText);

        // Atualizando posição calculada por forward kinematics e inverse kinematics
        Arm_Controller.forwardKinematics.isLocked = true;
        Arm_Controller.forwardKinematics.Set(armVariables[6], armVariables[7], armVariables[8]);
        //print("X: " + (Arm_Controller.forwardKinematics.position.x.ToString()) + "\nY: " + (Arm_Controller.forwardKinematics.position.y.ToString()) + "\nZ: " + (Arm_Controller.forwardKinematics.position.z.ToString()));
        Arm_Controller.forwardKinematics.isLocked = false;

    }

    /// <summary>
    /// Envia posição das juntas para o programa 
    /// </summary>
    public static void sendSimulatorData(ConnectionMode mode)
    {

        switch (mode)
        {
            case ConnectionMode.FK:

                try
                {
                    // Inicializando stream de dados
                    NetworkStream netStream = client.GetStream();

                    // Convertendo posição das juntas em array de bytes
                    byte[] simData = new byte[Arm_Controller.jointPositions.Length * sizeof(float)];
                    Buffer.BlockCopy(Arm_Controller.jointPositions, 0, simData, 0, simData.Length);

                    // Enviando dados para o python
                    netStream.Write(simData, 0, simData.Length);
                }
                catch (InvalidOperationException)
                {
                    StopConnection();
                }
                break;

            case ConnectionMode.IK:

                try
                {
                    // Inicializando stream de dados
                    NetworkStream netStream = client.GetStream();

                    // Convertendo posição desejada em array de bytes
                    byte[] simData = new byte[3*sizeof(float)];
                    float[] desiredPos = { Arm_Controller.inverseKinematics.position.x, Arm_Controller.inverseKinematics.position.y, Arm_Controller.inverseKinematics.position.z };
                    Buffer.BlockCopy(desiredPos, 0, simData, 0, simData.Length);

                    // Enviando dados para o python
                    netStream.Write(simData, 0, simData.Length);

                }
                catch (InvalidOperationException)
                {
                    StopConnection();
                }
                break;
        } 
    }

    #endregion

    public static void StartConnection()
    {
        // Interrompendo conexao caso ja inicializada 
        if (!hasStarted)
        {
            // Inicializando novo socket
            print("Socket started");
            ThreadStart ts = new ThreadStart(GetFloatArray);
            sThread = new Thread(ts);
            sThread.Start();
            hasStarted = true;
        }
    }

    public static void StopConnection()
    {
        if (isConnected)
        {
            isConnected = false;
            hasStarted = false;
            print("Ending Connection []");
            client.Close();
            listener.Stop();
            sThread.Abort();
        }
    }

    private void Start()
    {
        StartConnection();
    }

    private void OnApplicationQuit()
    {
        isConnected = false;

        if (!isConnected)
        {
            print("Ending Connection");
            listener.Stop();
            client.Close();
        }
    }
}