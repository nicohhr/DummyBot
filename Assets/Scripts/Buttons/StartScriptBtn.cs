using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScriptBtn : MonoBehaviour
{
    [SerializeField] public Button button; 

    public void StartScript()
    {
        PythonCommunication.StartConnection();

        // Conferindo aba selecionada 
        switch (TabManager.selectedTabIndex)
        {
            case 0:
                Python_Executer.ExecIt(SettingsManager.settings.FkScriptPath);
                break;
            case 1:
                Python_Executer.ExecIt(SettingsManager.settings.IkScriptPath);
                break;
        }
    }

    public static void StartIt()
    {
        PythonCommunication.StartConnection();

        // Conferindo aba selecionada 
        switch (TabManager.selectedTabIndex)
        {
            case 0:
                Python_Executer.ExecIt(SettingsManager.settings.FkScriptPath);
                break;
            case 1:
                Python_Executer.ExecIt(SettingsManager.settings.IkScriptPath);
                break;
        }
    }
}
