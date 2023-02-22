using System.Diagnostics;
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

    public static void ExecIt(string scriptPath)
    {
        // Definindo caminho do interpretador 
        processStart.FileName = $@"{SettingsManager.settings.VenvPath}";

        // Definindo Scritp a ser executado
        processStart.Arguments = $"\"{scriptPath}\"";

        // Executando processo
        Process.Start(processStart);
    }

    void Start()
    {
        // Definindo configurações do processo 
        processStart.UseShellExecute = true;
        processStart.CreateNoWindow = false;
        processStart.WindowStyle = ProcessWindowStyle.Minimized;
        processStart.RedirectStandardOutput = false;
        processStart.RedirectStandardError = false;
    }
}
