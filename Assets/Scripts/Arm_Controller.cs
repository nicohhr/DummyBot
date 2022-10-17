using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using Assets.Scripts;

public class Arm_Controller : MonoBehaviour
{
    enum AxisSelection {X, Y, Z}

    #region Variaveis

    // Definindo colecao de juntas e de controles de juntas 
    public List<Transform> joints = new List<Transform>();
    public static List<Transform> simJoints;
    public List<UnityEngine.UI.Slider> sliders = new List<UnityEngine.UI.Slider>();
    public List<GameObject> inputFields = new List<GameObject>();
    public Transform EndEffector;
    public List<object> rotationReferences = new List<object>() { AxisSelection.Z, AxisSelection.Y, AxisSelection.Y, AxisSelection.Y};
    public static float[] jointPositions;

    // Definindo velocidades angulares das juntas 
    public float TurnRate = 1.0f;

    // Limits
    [Range(0.0f, 360.0f)]
    public float maxRotationLimit = 180.0f;
    [Range(0.0f, 360.0f)]
    public float minRotationLimit = 0.0f;
    [Range(0.0f, 360.0f)]
    public float defaultPosition = 0;

    // Variaveis vindas do python
    [HideInInspector]
    public static JointPositions pythonPositions = new JointPositions();
    [HideInInspector]
    public static KinematicsInfo forwardKinematics = new KinematicsInfo();
    [HideInInspector]
    public static KinematicsInfo inverseKinematics = new KinematicsInfo();

    /// <summary>
    /// Altera componente de texto para exibir
    /// posi��o do end effector.
    /// </summary>
    public GameObject posOut;
    public GameObject rotOut;
    public GameObject estimatedFkOut;
    public GameObject estimatedIkOut;
    private TextMeshProUGUI posText;
    private TextMeshProUGUI rotText;
    private TextMeshProUGUI estimatedFkText;
    private TextMeshProUGUI estimatedIkText; 
    private List<TMP_InputField> inputs = new List<TMP_InputField>();

    #endregion

    #region M�todos

    private string floatToString(float number, int decimalNumbers = 2, float offset = 0)
    {
        float aux = (float)(Math.Round((double)number, decimalNumbers));
        return (aux + offset).ToString();
    }

    private float servo2Unity(float servoPos) { return (-1) * (servoPos - 90); }
    private float unity2Servo(float unityPos) { return - unityPos + 90; }

    public void resetArmPosition()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            setJointPosition(0, rotationReferences, i, true);
            updateJointsText();
        }
    }

    private float transformEulerAngles(Transform transform, AxisSelection axis, bool normalize = true)
    {
        // Armazena angulacao atual 
        float auxRot = 0;

        // Convertendo referencia de denavit para a do unity
        switch (axis)
        {
            case AxisSelection.X:
                auxRot = transform.localEulerAngles.z;
                break;
            case AxisSelection.Y:
                auxRot = transform.localEulerAngles.x;
                break;
            case AxisSelection.Z:
                auxRot = transform.localEulerAngles.y;
                break;
        }

        // Normalizando posi��o das juntas para corresponder a dos servos
        if (normalize)
        {
            if (auxRot > 90) auxRot = 90 - (auxRot - 360);
            else auxRot = 90 - auxRot;
        }

        // Retornando valor
        return auxRot;
    }

    public void ProcessSliderInput()
    {
        string rotAux = string.Empty;

        // Processando Juntas
        for (int i = 0; i < joints.Count; i++)
        {
            rotAux = rotText.text;

            if (sliders[i].value != 0)
            {
                rotAux = string.Empty;
                jointPositions[i] += (sliders[i].value * TurnRate);
                jointPositions[i] = Mathf.Clamp(jointPositions[i], minRotationLimit, maxRotationLimit);
                setJointPosition(servo2Unity(jointPositions[i]), rotationReferences, i);
                updateJointsText();
         
            }
        }
        
    }

    private void setJointPosition(float angularPos, List<object> axisSelection, int index, bool updatePositions = false)
    {
        // Atualizando posi��es se necess�rio
        if (updatePositions) { jointPositions[index] = unity2Servo(angularPos); }

        // Conferindo referencia da rotacao
        switch (axisSelection[index])
        {
            case AxisSelection.X:
                joints[index].localEulerAngles = new Vector3(joints[index].localEulerAngles.x, joints[index].localEulerAngles.y, angularPos);
                break;
            case AxisSelection.Z:
                joints[index].localEulerAngles = new Vector3(joints[index].localEulerAngles.x, angularPos, joints[index].localEulerAngles.z);
                break;
            case AxisSelection.Y:
                joints[index].localEulerAngles = new Vector3(angularPos, joints[index].localEulerAngles.y, joints[index].localEulerAngles.z);
                break;
        }
    }

    private void updateJointsText()
    {
        string rotAux = string.Empty;
        // Atualizando posicao final
        posText.text = "X: " + floatToString(-EndEffector.position.x) + "\nY: " + floatToString((EndEffector.position.z)) + "\nZ: " + floatToString(EndEffector.position.y);

        for (int i = 0; i < joints.Count; i++)
        {
            // Atualizando rotacao das juntas
            string auxVal = floatToString(transformEulerAngles(joints[i], (AxisSelection)rotationReferences[i]), decimalNumbers: 0);
            rotAux += "J" + (i + 1).ToString() + " = " + auxVal + "\n";
        }
        rotText.text = rotAux;
    }

    #endregion

    #region Eventos

    public void ResetSliders()
    {
        // Reseta os sliders de volta a 0 quando o click do mouse e leavantado
        foreach (UnityEngine.UI.Slider slider in sliders) { slider.value = 0; }
    }

    public void readInputFields()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i].text != string.Empty)
            {
                // Recuperando entrada de texto
                jointPositions[i] = float.Parse(inputs[i].text);
                jointPositions[i] = Mathf.Clamp(jointPositions[i], 0.0f, 180.0f);

                // Convertendo posi��o referenciada de servo motor para unity
                float anglePos = servo2Unity(jointPositions[i]);

                // Alterando posi��o da junta
                setJointPosition(anglePos, rotationReferences, i);

                // Reprocessando sliders
                updateJointsText();

                // Apgando entrada 
                inputs[i].text = string.Empty;

            }
        }
    }

    private void ProcessSocketData()
    {
        // Conferir se conex�o foi estabelecida 
        if (PythonCommunication.isConnected)
        {
            // Conferir aba selecionada
            switch (TabManager.selectedTabIndex)
            {
                case 0:

                    // Atualizar dados de posi��o de end effector calculadas pelo python 
                    estimatedFkText.text = "X: " + floatToString(forwardKinematics.position.x) + "\nY: " + floatToString((forwardKinematics.position.y)) + "\nZ: " + floatToString(forwardKinematics.position.z);
                    
                    // Enviando informacoes para o python
                    PythonCommunication.sendSimulatorData();
                    break;
            }
        }

        // Tab0: 
        // Tab0: Atualizar dados de posi��o de end effector calculada pelo python 

        // Tab1: 

    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Lendo arquivo de configura��es
        SettingsManager.Load();

        // Definindo quantidade de juntas
        jointPositions = new float[joints.Count];

        // Passando juntas a variaveis estaticas para utilizacao em outras classes
        simJoints = this.joints;

        // Armazenando posi��o inicial das juntas
        for (int i = 0; i < joints.Count; i++)
        {
            jointPositions[i] = unity2Servo(transformEulerAngles(joints[i], (AxisSelection)rotationReferences[i], false));
        }

        // Definindo valores iniciais dos sliders
        foreach (UnityEngine.UI.Slider slider in sliders)
        {
            slider.value = 0;   
            slider.minValue = -1;
            slider.maxValue = 1;
        }

        // Recuperando componentes de entrada de posi��o 
        for (int i = 0; i < inputFields.Count; i++) { inputs.Add(inputFields[i].GetComponent<TMP_InputField>()); }
        posText = posOut.GetComponent<TextMeshProUGUI>();
        rotText = rotOut.GetComponent<TextMeshProUGUI>();
        estimatedFkText = estimatedFkOut.GetComponent<TextMeshProUGUI>();
        rotText.text = string.Empty;

        // Exibindo angulos inciais 
        updateJointsText();

    }

    // Update is called once per frame
    void Update()
    {
        // Processando entradas
        if (Input.GetKeyDown(KeyCode.Return)) readInputFields();
        if (Input.GetKeyDown(KeyCode.R)) resetArmPosition();
        if (Input.GetKeyDown(KeyCode.P)) Python_Executer.ExecIt();
        ProcessSocketData();
        ProcessSliderInput();
    }
}
