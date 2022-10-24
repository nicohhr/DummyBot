using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using TMPro;

public class ComDropdown : MonoBehaviour
{

    [SerializeField] public TMP_Dropdown _DropDown; 

    // Start is called before the first frame update
    void Start()
    {
        // Atualizando lista de portas disponíveis
        UpdateSerialPorts();

        // Adicionando ouvinte 
        _DropDown.onValueChanged.AddListener(delegate { DropdownItemSelected(_DropDown); });

    }

    public void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        // Recuperando indice selecionado
        int index = _DropDown.value;

        // Atualizando valores selecionados para inicio de conexão 
        SerialConnection.COMPort = _DropDown.options[index].text;        
    }



    public void UpdateSerialPorts()
    {
        // Limpando opções iniciais 
        _DropDown.options.Clear();

        // Recuperando lista de portas seriais disponíveis
        string[] ports = SerialPort.GetPortNames();

        // Adicionando componentes
        foreach (string port in ports)
        {
            _DropDown.options.Add(new TMP_Dropdown.OptionData() { text = port });
        }

        // Atualizando valor exibido 
        _DropDown.RefreshShownValue();
    }
}

