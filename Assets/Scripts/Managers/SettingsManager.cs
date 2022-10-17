using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Armazena instância de configuracoes
    /// </summary>

    public static Settings settings; 

    // Entradas de texto para usuário definir caminho dos arquivos
    [SerializeField] TMP_InputField VenvInput;
    [SerializeField] TMP_InputField FkScriptInput;
    [SerializeField] TMP_InputField IkScriptInput;
    public static TMP_InputField sVenvInput;
    public static TMP_InputField sFkScriptInput;
    public static TMP_InputField sIkScriptInput;

    /// <summary>
    /// Salva informacoes da instancia de configuracoes
    /// </summary>
    public static void Save()
    {
        // Repassando o valor dos campos
        settings.VenvPath = sVenvInput.text;
        settings.FkScriptPath = sFkScriptInput.text;
        settings.IkScriptPath = sIkScriptInput.text;
        Debug.Log("setts -> " + settings.VenvPath + " | " + settings.FkScriptPath + " | " + settings.IkScriptPath);
        SaveSystem.SaveSettings(settings);
    }

    /// <summary>
    /// Carrega instancia de configuracoes a partir de arquivo binario
    /// </summary>
    public static void Load() 
    { 
        settings = SaveSystem.LoadSettings(); 
        if (settings == null)
        {
            settings = new Settings();
        }
    }

    /// <summary>
    /// Le arquivo de configuracoes e define caminhos previamente salvos 
    /// </summary>
    public void SetComponentsText()
    {
        if (settings != null)
        {
            this.VenvInput.text = settings.VenvPath;
            this.FkScriptInput.text = settings.FkScriptPath;
            this.IkScriptInput.text = settings.IkScriptPath;
        } else
        {
            this.VenvInput.text = string.Empty;
            this.FkScriptInput.text = string.Empty;
            this.IkScriptInput.text = string.Empty;
        }
    }

    void Start()
    {
        // Definindo variaveis estaticas
        sVenvInput = VenvInput;
        sFkScriptInput = FkScriptInput;
        sIkScriptInput = IkScriptInput;

        // Carregando instância de confirgurações
        Load();

        // Exibindo propriedades no componente 
        SetComponentsText();
    }
}
