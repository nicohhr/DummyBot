using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Armazena configuracao de scripts e venv do python.
/// </summary>
[Serializable]
public class Settings
{
    private string _VenvPath;
    private string _FkScriptPath;
    private string _IkScriptPath;
    private int _BaudRateValue; 

    public string VenvPath { get { return _VenvPath; } set { _VenvPath = value; } }
    public string FkScriptPath { get { return _FkScriptPath; } set { _FkScriptPath = value; } }
    public string IkScriptPath { get { return _IkScriptPath; } set { _IkScriptPath = value; } }
    public int BaudRateValue { get { return _BaudRateValue; } set { _BaudRateValue = value; } }

    public Settings()
    {
        _VenvPath = string.Empty;
        _FkScriptPath = string.Empty;
        _IkScriptPath = string.Empty;
        _BaudRateValue = 0;
    }
}
