using System;
using System.IO;
using System.Threading;
using UnityEngine;

public  class LogData : MonoBehaviour
{
    /// <summary>
    /// Indica se log de informação já começou
    /// </summary>
    public static bool hasStarted = false;
    private static string currentFile = "";
    private static string persistentDir = "";
    private static string msg = string.Empty; 
    private static Thread sThread; 

    private static string GenerateNameFile()
    {
        if (TabManager.selectedTabIndex == 0) return persistentDir + "/log_fk_" + DateTime.Now.ToString("MM-dd_HH-mm-ss") + ".txt";
        return persistentDir + "/log_ik_" + DateTime.Now.ToString("MM-dd_HH-mm-ss") + ".txt";
    }

    private static void CreateLogFile(string message)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(currentFile))
            {
                sw.WriteLine($"{DateTime.Now.ToString()} - {message}");
            }
        }
        catch (IOException)
        {
            Debug.Log("Stream Closed");
            Stop();
        }
    }

    public static void StartProcess()
    {
        // Iniciar thread 
        persistentDir = Application.persistentDataPath;
        Debug.Log("Log Started");
        ThreadStart ts = new ThreadStart(logData);
        sThread = new Thread(ts);
        sThread.Start();
        hasStarted = true;
    }

    public static void logData()
    {
        // Gerar nome do log trabalhado 
        currentFile = GenerateNameFile();

        while (hasStarted)
        {
            // Definindo mensagem e enviando dados 
            string data;
            if (TabManager.selectedTabIndex == 0) 
            {
                data = Arm_Controller.fk_msg + " " + Arm_Controller.floatToString(Arm_Controller.forwardKinematics.position.x) + " " + 
                       Arm_Controller.floatToString(Arm_Controller.forwardKinematics.position.y) + " " + Arm_Controller.floatToString(Arm_Controller.forwardKinematics.position.z);
            }
            else
            {
                data = Arm_Controller.ik_msg + " " + Arm_Controller.floatToString(Arm_Controller.inverseKinematics.position.x) + " " + 
                       Arm_Controller.floatToString(Arm_Controller.inverseKinematics.position.y) + " " + 
                       Arm_Controller.floatToString(Arm_Controller.inverseKinematics.position.z);
            } 

            // Loggando dados 
            CreateLogFile(data);
            
            // Pausa da thread
            Thread.Sleep(200);
        }
    }

    public static void Stop()
    {
        hasStarted = false;
        Debug.Log("Log Stoped");
    }


}
