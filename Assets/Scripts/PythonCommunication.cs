using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.Experimental.AI;

public class PythonCommunication : MonoBehaviour
{
    private const int PACKET_SIZE_BYTES = 48;
    private const int PACKET_SIZE = 12;
    
    #region Viariaveis 

    private static Thread sThread;
    private static string connectionIP = "127.0.0.1";
    private static int connectionPort = 25001;
    static IPAddress localAdress;
    static TcpListener listener;
    static TcpClient client;
    static bool isRunning = true;

    static private float[] armVariables = new float[PACKET_SIZE];
    static public bool isConnected = false; 

    #endregion

    #region Metodos

    /// <summary>
    /// Este método lê diretamente os valroes binarios recebidos na conexão. 
    /// </summary>
    private static  void GetInfoBytes()
    {
        SetConnection();

        while (isRunning)
        {
            NetworkStream netStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            // Recebendo dados do host 
            int bytesRead = netStream.Read(buffer, 0, 12); // recebendo dados em bytes do Python
            string output = string.Empty;

            for(int i = 0; i < (12); i++)
            {
                output += buffer[i].ToString() + " | ";
            }

            print(buffer.Length + "-->" +  output);
        }
        listener.Stop();

    }

    private static void GetFloat()
    {
        SetConnection();

        while (isRunning)
        {   
            // Recebendo dados
            NetworkStream netStream = client.GetStream();
            byte[] buffer = new byte[4];
            netStream.Read(buffer, 0, 4); 

            // Convertendo array de dados em float
            float resultFloat = BitConverter.ToSingle(buffer, 0);

            // Exibindo número resultante 
            Debug.Log(resultFloat);

        }
        listener.Stop();
    }


    private static void GetArmInfo()
    {
        SetConnection();

        while (isRunning)
        {
            NetworkStream netStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            // Recebendo dados do host 
            int bytesRead = netStream.Read(buffer, 0, client.ReceiveBufferSize); // Lendo Bytes do Python
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); // Convertendo dados lidos para string 
            armVariables = String2Array(dataReceived); // Salvando dados no controlador do manipulador
            UpdateArmVariables();

            print(buffer.Length + "-->" + dataReceived);
        }
        listener.Stop();
    }

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

        while (isRunning)
        {
            // Recebendo dados: 12 numeros float, 4 bytes por numero 48 bytes por pacote
            NetworkStream netStream = client.GetStream();
            byte[] buffer = new byte[PACKET_SIZE_BYTES];
            netStream.Read(buffer, 0, PACKET_SIZE_BYTES);

            // Convertendo array de bytes em array de floats
            for (int i = 0; i < PACKET_SIZE; i++) { armVariables[i] = BitConverter.ToSingle(buffer, (i * 4)); }

            // Exibindo dados resultantes
            string output = string.Empty;
            for (int i = 0; i < (PACKET_SIZE); i++) { output += armVariables[i].ToString() + " | "; }
            UpdateArmVariables();
            //print(output);

        }
        listener.Stop();
    }

    private static void UpdateArmVariables()
    {

        // Atualizando posições (primeiros 6 valores da entrada)
        Arm_Controller.pythonPositions.isLocked = true;
        for (int i = 0; i < 4; i++) { Arm_Controller.pythonPositions.positions[i] = armVariables[i]; }
        Arm_Controller.pythonPositions.isLocked = false; 

        // Atualizando posição calculada por forward kinematics e inverse kinematics
        Arm_Controller.forwardKinematics.isLocked = true;
        Arm_Controller.forwardKinematics.Set(armVariables[6], armVariables[7], armVariables[8]);
        print("X: " + (Arm_Controller.forwardKinematics.position.x.ToString()) + "\nY: " + (Arm_Controller.forwardKinematics.position.y.ToString()) + "\nZ: " + (Arm_Controller.forwardKinematics.position.z.ToString()));
        Arm_Controller.inverseKinematics.position.Set(armVariables[9], armVariables[10], armVariables[11]);
        Arm_Controller.forwardKinematics.isLocked = false;

        // Enviando dados de posição das juntas para o python 
        // sendSimulatorData();

    }

    /// <summary>
    /// Envia posição das juntas para o programa 
    /// </summary>
    public static void sendSimulatorData()
    {
        NetworkStream netStream = client.GetStream();

        // Convertendo posição das juntas em array de bytes
        byte[] simData = new byte[Arm_Controller.jointPositions.Length*sizeof(float)];
        Buffer.BlockCopy(Arm_Controller.jointPositions, 0, simData, 0, simData.Length);

        // Enviando dados para o python
        netStream.Write(simData, 0, simData.Length);

        float[] teste = new float[4];

        // Convertendo array de bytes em array de floats
        for (int i = 0; i < 4; i++) { teste[i] = BitConverter.ToSingle(simData, (i * 4)); }

        // Exibindo dados resultantes
        string output = string.Empty;
        for (int i = 0; i < (4); i++) { output += teste[i].ToString() + " | "; }

        print(output);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        ThreadStart ts = new ThreadStart(GetFloatArray);
        sThread = new Thread(ts);
        sThread.Start();

    }
}