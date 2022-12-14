using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using TMPro;
using System.Reflection;

public class ComDropdown : MonoBehaviour
{

    [SerializeField] public TMP_Dropdown _DropDown;
    private string[] ports; 

    // Start is called before the first frame update
    void Start()
    {
        // Atualizando lista de portas dispon?veis
        UpdateSerialPorts();

        // Adicionando ouvinte 
        _DropDown.onValueChanged.AddListener(delegate { DropdownItemSelected(_DropDown); });

        // Conferindo se h? portas dispon?veis e selecionando primeiro valor da lista
        if (ports.Length > 0) SerialConnection.COMPort = _DropDown.options[0].text;

    }

    public void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        // Recuperando indice selecionado
        int index = _DropDown.value;

        // Atualizando valores selecionados para inicio de conex?o 
        SerialConnection.COMPort = _DropDown.options[index].text;        
    }



    public void UpdateSerialPorts()
    {
        // Limpando op??es iniciais 
        _DropDown.options.Clear();

        // Recuperando lista de portas seriais dispon?veis
        ports = SerialPort.GetPortNames();

        // Adicionando componentes
        foreach (string port in ports)
        {
            _DropDown.options.Add(new TMP_Dropdown.OptionData() { text = port });
        }

        // Atualizando valor exibido 
        _DropDown.RefreshShownValue();
    }
}

