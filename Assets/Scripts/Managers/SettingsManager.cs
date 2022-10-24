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
    [SerializeField] TMP_Dropdown BaudRateDrop; 
    public static TMP_InputField sVenvInput;
    public static TMP_InputField sFkScriptInput;
    public static TMP_InputField sIkScriptInput;
    public static TMP_Dropdown sBaudRateDrop;

    /// <summary>
    /// Salva informacoes da instancia de configuracoes
    /// </summary>
    public static void Save()
    {
        // Repassando o valor dos campos
        settings.VenvPath = sVenvInput.text;
        settings.FkScriptPath = sFkScriptInput.text;
        settings.IkScriptPath = sIkScriptInput.text;
        settings.BaudRateValue = sBaudRateDrop.value;
        Debug.Log("setts -> " + settings.VenvPath + " | " + settings.FkScriptPath + " | " + settings.IkScriptPath);
        SaveSystem.SaveSettings(settings);
    }

    /// <summary>
    /// Atualizando valor de BaudRate no objeto settings sem serializar o valor 
    /// </summary>
    public static void ChangeBaudRate()
    {
        settings.BaudRateValue = sBaudRateDrop.value;
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
            this.BaudRateDrop.value = settings.BaudRateValue;
        } else
        {
            this.VenvInput.text = string.Empty;
            this.FkScriptInput.text = string.Empty;
            this.IkScriptInput.text = string.Empty;
            this.BaudRateDrop.value = 0;
        }
    }

    void Start()
    {
        // Definindo variaveis estaticas
        sVenvInput = VenvInput;
        sFkScriptInput = FkScriptInput;
        sIkScriptInput = IkScriptInput;
        sBaudRateDrop = BaudRateDrop;

        // Carregando instância de confirgurações
        Load();

        // Exibindo propriedades no componente 
        SetComponentsText();
    }
}
