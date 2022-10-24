using System.IO.Ports;
using TMPro;
using UnityEngine;

public class BaudRateDropdown : MonoBehaviour
{

    [SerializeField] public TMP_Dropdown _DropDown;

    // Start is called before the first frame update
    void Start()
    {
        // Passando valor selecionado 
        GetSelectedRate();

        // Adicionando ouvinte 
        _DropDown.onValueChanged.AddListener(delegate { DropdownItemSelected(_DropDown); });

    }

    public void DropdownItemSelected(TMP_Dropdown dropdown) { GetSelectedRate(); }

    public void GetSelectedRate()
    {
        // Atualizando valor na inst�ncia de configura��es
        SettingsManager.ChangeBaudRate();

        // Atualizando valores selecionados para inicio de conex�o 
        int index = _DropDown.value;
        SerialConnection.BaudRate = int.Parse(_DropDown.options[index].text);
    }
}
