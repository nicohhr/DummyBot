using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using TMPro;
using System.Reflection;
using Unity.VisualScripting;

public class ComDropdown : MonoBehaviour
{

    [SerializeField] public TMP_Dropdown _DropDown;
    private static TMP_Dropdown Dropdown;
    private static string[] ports; 

    // Start is called before the first frame update
    void Start()
    {
        // Repassando variáveis
        Dropdown = _DropDown;

        // Atualizando lista de portas disponíveis
        UpdateSerialPorts();

        // Adicionando ouvinte 
        _DropDown.onValueChanged.AddListener(delegate { DropdownItemSelected(_DropDown); });

        // Conferindo se há portas disponíveis e selecionando primeiro valor da lista
        if (ports.Length > 0) SerialConnection.COMPort = _DropDown.options[0].text;

    }

    public void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        // Recuperando indice selecionado
        int index = _DropDown.value;

        // Atualizando valores selecionados para inicio de conexão 
        SerialConnection.COMPort = _DropDown.options[index].text;        
    }



    public static void UpdateSerialPorts()
    {
        // Limpando opções iniciais 
        if (Dropdown != null) Dropdown.options.Clear();

        // Recuperando lista de portas seriais disponíveis
        ports = SerialPort.GetPortNames();

        if (ports.Length == 1)
        {
            // Tornando botão de Conexão clicavel 
            ConnectArmBtn.MakeInteracteble();

            // Adicionando componente 
            Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = ports[0] });

            // Selecionando porta serial (por ser a única)
            SerialConnection.COMPort = Dropdown.options[0].text;
        }
        else if (ports.Length > 0)
        {
            // Adicionando componentes
            foreach (string port in ports)
            {
                Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = port });
            }

            // Tornando botão de Conexão clicavel 
            ConnectArmBtn.MakeInteracteble();

        } else
        {   
            // Desabilitando botão de conexão
            ConnectArmBtn.MakeNonIteracteble();
        }

        // Atualizando valor exibido 
        Dropdown.RefreshShownValue();
    }
}

