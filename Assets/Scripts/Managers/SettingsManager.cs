using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] TMP_InputField FkRateField;
    [SerializeField] TMP_InputField IkRateField;
    public Slider FkRateSlider; 
    public Slider IkRateSlider;

    // Campos estáticos para leitura em outras classes 
    public static TMP_InputField sVenvInput;
    public static TMP_InputField sFkScriptInput;
    public static TMP_InputField sIkScriptInput;
    public static TMP_Dropdown sBaudRateDrop;
    public static TMP_InputField sFkRateField;
    public static TMP_InputField sIkRateField;
    public static Slider sFkRateSlider;
    public static Slider sIkRateSlider;

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
        settings.FkRate = (float)(Math.Round((double)sFkRateSlider.value, 2));
        settings.IkRateDiv = (float)(Math.Round((double)sIkRateSlider.value, 2));

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
        if (settings == null) settings = new Settings();

        //Debug.Log("Fk: " + settings.FkRate.ToString() + " Ik: " + settings.IkRateDiv.ToString());
    }

    /// <summary>
    /// Atualiza texto no input field com valor atual do slider
    /// </summary>
    public void FkValueChange() 
    { 
        FkRateField.text = Arm_Controller.floatToString(FkRateSlider.value); 
    }

    /// <summary>
    /// Atualiza texto no input field com valor atual do slider
    /// </summary>
    public void IkValueChange() 
    { 
        IkRateField.text = Arm_Controller.floatToString(IkRateSlider.value); 
    }

    /// <summary>
    /// Le arquivo de configuracoes e define caminhos previamente salvos 
    /// </summary>
    public static void SetComponentsText()
    {
        if (settings != null)
        {
            sVenvInput.text = settings.VenvPath;
            sFkScriptInput.text = settings.FkScriptPath;
            sIkScriptInput.text = settings.IkScriptPath;
            sBaudRateDrop.value = settings.BaudRateValue;
            sFkRateSlider.value = settings.FkRate;
            sIkRateSlider.value = settings.IkRateDiv;
            sFkRateField.text = sFkRateSlider.value.ToString();
            sIkRateField.text = sIkRateSlider.value.ToString();

        } else
        {
            sVenvInput.text = string.Empty;
            sFkScriptInput.text = string.Empty;
            sIkScriptInput.text = string.Empty;
            sBaudRateDrop.value = 0;
            sFkRateSlider.value = 0;
            sIkRateSlider.value = 0;
            sFkRateField.text = string.Empty;
            sIkRateField.text = string.Empty;
        }
    }

    void Start()
    {
        // Definindo variaveis estaticas
        sVenvInput = VenvInput;
        sFkScriptInput = FkScriptInput;
        sIkScriptInput = IkScriptInput;
        sBaudRateDrop = BaudRateDrop;
        sFkRateSlider = FkRateSlider;
        sIkRateSlider = IkRateSlider;
        sFkRateField = FkRateField;
        sIkRateField = IkRateField;

        // Adicionando evento dos sliders
        FkRateSlider.onValueChanged.AddListener(delegate { FkValueChange(); });
        IkRateSlider.onValueChanged.AddListener(delegate { IkValueChange(); });

        // Carregando instância de confirgurações

        // Exibindo propriedades no componente 
        SetComponentsText();
    }
}
