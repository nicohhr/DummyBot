using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Python_Executer : MonoBehaviour
{

    #region Variaveis
    /// <summary>
    /// Instanciando processo capaz de inicializar processo definido
    /// </summary>
    private static ProcessStartInfo processStart = new ProcessStartInfo();
    

    #endregion

    public static void ExecIt()
    {
        // Definindo caminho do interpretador 
        processStart.FileName = $@"{SettingsManager.settings.VenvPath}";

        // Definindo Scritp a ser executado
        processStart.Arguments = $"\"{SettingsManager.settings.FkScriptPath}\"";

        // Executando processo
        Process.Start(processStart);
    }

    void Start()
    {
        // Definindo configurações do processo 
        processStart.UseShellExecute = true;
        processStart.CreateNoWindow = false;
        processStart.RedirectStandardOutput = false;
        processStart.RedirectStandardError = false;
    }
}
