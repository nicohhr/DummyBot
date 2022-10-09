using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class Python_Executer : MonoBehaviour
{

    #region Variaveis

    #endregion

    public static void ExecIt()
    {
        // 1) Criar instância de ProcessInfo informando caminho do interpretador 
        ProcessStartInfo processStart = new ProcessStartInfo();
        processStart.FileName = @"G:\Meu Drive\Faculdade\11ro Periodo\TCC\Codes\TCC\venv\Scripts\python.exe";

        // 2) Prover script e argumentos se necessário 
        //string path = EditorUtility.OpenFilePanel("Select Python Interpreter", "", "exe");
        string path = @"G:\Meu Drive\Faculdade\11ro Periodo\TCC\Codes\TCC\denavit_model.py";
        processStart.Arguments = $"\"{path}\"";

        // 3) Processo de configuração 
        processStart.UseShellExecute = true;
        processStart.CreateNoWindow = false;
        processStart.RedirectStandardOutput = false;
        processStart.RedirectStandardError = false;

        // 4) Executar processo 
        Process.Start(processStart);
    }
}
