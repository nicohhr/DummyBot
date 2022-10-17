using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System;

public class PythonCommunication : MonoBehaviour
{
    private const int PACKET_SIZE_BYTES = 48;
    private const int PACKET_SIZE = 12;
    
    #region Viariaveis 

    private static Thread sThread;
    private static string connectionIP = "127.0.0.1";
    [SerializeField] public static int connectionPort = 25001;
    static IPAddress localAdress;
    static TcpListener listener;
    static TcpClient client;
    static bool isRunning = true;

    static private float[] armVariables = new float[PACKET_SIZE];
    static public bool isConnected = false; 

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

        while (true)
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
    }

    private static void UpdateArmVariables()
    {

        // Atualizando posi��es (primeiros 6 valores da entrada)
        Arm_Controller.pythonPositions.isLocked = true;
        for (int i = 0; i < 4; i++) { Arm_Controller.pythonPositions.positions[i] = armVariables[i]; }
        Arm_Controller.pythonPositions.isLocked = false; 

        // Atualizando posi��o calculada por forward kinematics e inverse kinematics
        Arm_Controller.forwardKinematics.isLocked = true;
        Arm_Controller.forwardKinematics.Set(armVariables[6], armVariables[7], armVariables[8]);
        //print("X: " + (Arm_Controller.forwardKinematics.position.x.ToString()) + "\nY: " + (Arm_Controller.forwardKinematics.position.y.ToString()) + "\nZ: " + (Arm_Controller.forwardKinematics.position.z.ToString()));
        Arm_Controller.inverseKinematics.position.Set(armVariables[9], armVariables[10], armVariables[11]);
        Arm_Controller.forwardKinematics.isLocked = false;

    }

    /// <summary>
    /// Envia posi��o das juntas para o programa 
    /// </summary>
    public static void sendSimulatorData()
    {
        NetworkStream netStream = client.GetStream();

        // Convertendo posi��o das juntas em array de bytes
        byte[] simData = new byte[Arm_Controller.jointPositions.Length*sizeof(float)];
        Buffer.BlockCopy(Arm_Controller.jointPositions, 0, simData, 0, simData.Length);

        // Enviando dados para o python
        netStream.Write(simData, 0, simData.Length);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        print("Socket started");
        ThreadStart ts = new ThreadStart(GetFloatArray);
        sThread = new Thread(ts);
        sThread.Start();

    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        if (!isRunning)
        {
            print("Ending Connection");
            listener.Stop();
            client.Close();
        }
    }
}