using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts;
using System.Linq;

public class Arm_Controller : MonoBehaviour
{
    public enum AxisSelection {X, Y, Z}

    #region Variaveis

    // Definindo colecao de juntas e de controles de juntas 
    public List<Transform> joints = new List<Transform>();
    public static List<Transform> simJoints;
    public List<UnityEngine.UI.Slider> fkSliders = new List<UnityEngine.UI.Slider>();
    public List<UnityEngine.UI.Slider> ikSliders = new List<UnityEngine.UI.Slider>();
    public Transform EndEffectorFk;
    public Transform EndEffectorIk;
    public Transform EndEffector;
    public static List<object> rotationReferences = new List<object>() { AxisSelection.Z, AxisSelection.Y, AxisSelection.Y, AxisSelection.Y };
    public static float[] jointPositions;

    // Definindo velocidades angulares das juntas 
    public float TurnRate = 1.0f;

    // Definindo limites das juntas 
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

    // Labels e entradas de texto
    [SerializeField] public TextMeshProUGUI estimatedFkText; 
    [SerializeField] public TextMeshProUGUI estimatedIkText; 

    // Componentes de texto para exibir posicao
    [SerializeField] private List<TextMeshProUGUI> fkPosTexts;
    [SerializeField] private List<TextMeshProUGUI> ikPosTexts;
    [SerializeField] private List<TextMeshProUGUI> desiredPosTexts;  

    // Componentes de texto para exibir rotacao 
    [SerializeField] private List<TextMeshProUGUI> rotTexts;

    // Variaveis estatics 
    public static float minRotLimit; 
    public static float maxRotLimit;
    public static List<TextMeshProUGUI> sFkPosTexts;
    public static List<TextMeshProUGUI> sIkPosTexts;
    public static List<TextMeshProUGUI> sRotTexts;
    public static Transform sEndEffectorFk;
    public static Transform sEndEffectorIk;
    public static Vector3 initialPos;

    // Objetos para esconder
    public List<GameObject> armHidenParts;

    #endregion

    #region Métodos

    public static string floatToString(float number, int decimalNumbers = 2, float offset = 0)
    {
        float aux = (float)(Math.Round((double)number, decimalNumbers));
        return (aux + offset).ToString();
    }

    public static float servo2Unity(float servoPos) 
    { 
        return (-1) * (servoPos - 90); 
    }

    public static float unity2Servo(float unityPos) { return - unityPos + 90; }

    public static float transformEulerAngles(Transform transform, AxisSelection axis, bool normalize = true)
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

        // Normalizando posição das juntas para corresponder a dos servos
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

        switch (TabManager.selectedTabIndex)
        {
            // Forward Kinematics tab
            case 0:

                // Processando Juntas
                for (int i = 0; i < joints.Count; i++)
                {
                    //rotAux = rotText.text;

                    if (fkSliders[i].value != 0)
                    {
                        jointPositions[i] += (fkSliders[i].value * TurnRate);
                        jointPositions[i] = Mathf.Clamp(jointPositions[i], minRotationLimit, maxRotationLimit);
                        setJointPosition(servo2Unity(jointPositions[i]), rotationReferences, i);
                        updateText();
                    }
                }
                break;

            // Inverse Kinematics tab
            case 1:

                if (PythonCommunication.isConnected)
                {
                    // Recuperando posição das juntas
                    float[] ikPositions = inverseKinematics.positions;

                    // Processando eixos cartesianos 
                    for (int i = 0; i < 3; i++)
                    {
                        // Aplicando variação dos sliders 
                        ikPositions[i] += (ikSliders[i].value * TurnRate/20);

                        // Clamping do valor
                        if (i == 1) ikPositions[i] = Mathf.Clamp(ikPositions[i], -50, 50);
                        else ikPositions[i] = Mathf.Clamp(ikPositions[i], 0, 50);

                        // Atualizando texto
                        updateText();
                    }

                    // Repassando valores para a instância kinetic info 
                    inverseKinematics.positions = ikPositions;
                }


                break; 
        } 
    }

    private void setJointPosition(float angularPos, List<object> axisSelection, int index, bool updatePositions = false)
    {
        //print("i " + index.ToString() + "-> " + jointPositions[index].ToString());x
        // Atualizando posições se necessário
        if (updatePositions)
        {
            jointPositions[index] = unity2Servo(angularPos);
        }

        // Invertendo rotação no caso da junta três
        if (index == 2 || index == 3) { angularPos = (-1) * angularPos; }

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

    public static void updateText()
    {
        // Atualizando exibicao de posicao fk 
        foreach (TextMeshProUGUI textMesh in sFkPosTexts)
        {
            // Atualizando posicao final
            textMesh.text = "X: " + floatToString(-sEndEffectorFk.position.x) + "\nY: " + floatToString((sEndEffectorFk.position.z)) + "\nZ: " + floatToString(sEndEffectorFk.position.y);
        }

        // Atualizando exibicao de posicao ik 
        foreach (TextMeshProUGUI textMesh in sIkPosTexts)
        {
            // Atualizando posicao final
            textMesh.text = "X: " + floatToString(-sEndEffectorIk.position.x) + "\nY: " + floatToString((sEndEffectorIk.position.z)) + "\nZ: " + floatToString(sEndEffectorIk.position.y);
        }

        // Atualizando exibicao de rotacao 
        foreach (TextMeshProUGUI textMesh in sRotTexts)
        {
            // String auxiliar
            string rotAux = string.Empty;

            for (int i = 0; i < simJoints.Count; i++)
            {
                // Atualizando rotacao das juntas
                //string auxVal = floatToString(transformEulerAngles(simJoints[i], (AxisSelection)rotationReferences[i]), decimalNumbers: 0);
                string auxVal = floatToString(jointPositions[i]).ToString();
                rotAux += "J" + (i + 1).ToString() + " = " + auxVal + "\n";
            }

            // Atualizando componentes de texto
            textMesh.text = rotAux;
        }
    }

    #endregion

    #region Eventos

    public void ResetSliders()
    {
        // Reseta os sliders de volta a 0 quando o click do mouse e leavantado
        foreach (UnityEngine.UI.Slider slider in fkSliders) { slider.value = 0; }
        foreach (UnityEngine.UI.Slider slider in ikSliders) { slider.value = 0; }
    }

    private void ProcessSocketData()
    {
        // Conferir se conexão foi estabelecida 
        if (PythonCommunication.isConnected)
        {
            // Conferir aba selecionada
            switch (TabManager.selectedTabIndex)
            {
                // Forward Kinematics tab
                case 0:

                    // Atualizar dados de posição de end effector calculadas pelo python 
                    estimatedFkText.text = "X: " + floatToString(forwardKinematics.position.x) + "\nY: " + floatToString((forwardKinematics.position.y)) + "\nZ: " + floatToString(forwardKinematics.position.z);
                    
                    // Enviando informacoes para o python
                    PythonCommunication.sendSimulatorData(ConnectionMode.FK);
                    break;

                // Inverse kinematics tab 
                case 1:

                    // Atualizando posicao desejada exibida
                    foreach (TextMeshProUGUI textMesh in desiredPosTexts)
                    {
                        textMesh.text = "X = " + floatToString(inverseKinematics.position.x) + " \nY = " + floatToString(inverseKinematics.position.y) + " \nZ = " + floatToString(inverseKinematics.position.z);
                    }

                    // Atualizar posição das juntas a partir dos dados recebidos do python 
                    for (int i = 0; i < joints.Count; i++)
                    {
                        setJointPosition(servo2Unity(pythonPositions.positions[i]), rotationReferences, i, true);
                    }

                    // Enviando dados de posicao desejada para o python 
                    PythonCommunication.sendSimulatorData(ConnectionMode.IK);
                    break;
            }
        } else
        {
            // Mostrando campo como vazio
            estimatedFkText.text = string.Empty;
            foreach (TextMeshProUGUI textMesh in desiredPosTexts) { textMesh.text = string.Empty; }
        }

    }

    public static void UpdateIkPos()
    {
        if (sEndEffectorIk != null) inverseKinematics.Set(-sEndEffectorIk.position.x, sEndEffectorIk.position.z, sEndEffectorIk.position.y);
    }

    public static void UpdateIK()
    {
        inverseKinematics.Set(-sEndEffectorIk.position.x, sEndEffectorIk.position.z, sEndEffectorIk.position.y);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Lendo arquivo de configurações
        SettingsManager.Load();

        // Definindo quantidade de juntas
        jointPositions = new float[joints.Count];

        // Passando juntas a variaveis estaticas para utilizacao em outras classes
        simJoints = this.joints;
        minRotLimit = minRotationLimit;
        maxRotLimit = maxRotationLimit;
        sFkPosTexts = fkPosTexts;
        sIkPosTexts = ikPosTexts;
        sRotTexts = rotTexts;
        sEndEffectorFk = EndEffectorFk;
        sEndEffectorIk = EndEffectorIk;

        // Armazenando posição inicial das juntas
        for (int i = 0; i < joints.Count; i++) { jointPositions[i] = unity2Servo(transformEulerAngles(joints[i], (AxisSelection)rotationReferences[i], false)); }

        // Definindo valores iniciais dos sliders fk
        foreach (UnityEngine.UI.Slider slider in fkSliders)
        {
            slider.value = 0;   
            slider.minValue = -1;
            slider.maxValue = 1;
        }

        // Definindo valores iniciais dos sliders ik
        foreach (UnityEngine.UI.Slider slider in ikSliders)
        {
            slider.value = 0;
            slider.minValue = -1;
            slider.maxValue = 1;
        }

        // Escondendo partes desejadas do braço 
        //foreach (GameObject armPart in armHidenParts) armPart.SetActive(false);

        // Definindo posicao desejada incial
        inverseKinematics.Set(-EndEffector.position.x, EndEffector.position.z, EndEffector.position.y);
        initialPos = EndEffector.transform.position;

        // Definindo posição de reset desejada
        SetEndEffectorPos.initialPosition[2] = initialPos.y;
        SetEndEffectorPos.initialPosition[1] = initialPos.z;
        SetEndEffectorPos.initialPosition[0] = -initialPos.x;
        Debug.Log("Initial Positions: " + SetEndEffectorPos.initialPosition[0].ToString() + " " + SetEndEffectorPos.initialPosition[1].ToString() + " " + SetEndEffectorPos.initialPosition[2].ToString());

        // Exibindo angulos inciais 
        updateText();

    }

    // Update is called once per frame
    void Update()
    {
        // Processando entradas
        ProcessSocketData();
        ProcessSliderInput();
    }
}