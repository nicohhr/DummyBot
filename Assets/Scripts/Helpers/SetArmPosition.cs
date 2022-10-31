using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class SetArmPosition : MonoBehaviour
{
    // Definindo eixos de rotação
    enum AxisSelection { X, Y, Z }

    // Entrada de ângulos
    [SerializeField] public List<TMP_InputField> inputFields = new List<TMP_InputField>();

    // Coleção de juntas do manipulador
    public List<Transform> joints;

    // Variáveis de animação
    public static bool SetMode = false;
    private float[] diffs = new float[4];
    private float[] initialDiffs = new float[4];
    private bool[] iterationOver = new bool[4];
    private float[] desiredJointPosition = new float[4];
    private float rotationRate = 0.25F;

    // Funções
    private void setJointPosition(float angularPos, List<object> axisSelection, int index, bool updatePositions = false)
    {
        // Atualizando posições se necessário
        if (updatePositions) { Arm_Controller.jointPositions[index] = Arm_Controller.unity2Servo(angularPos); }

        // Invertendo rotação no caso da junta três
        if (index == 2 || index == 3) { angularPos = (-1) * angularPos; }

        // Conferindo referencia da rotacao
        switch ((AxisSelection)axisSelection[index])
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

    void SummSub(float[] deltas, int jointIndex)
    {
        // Recuperar posição e arredondar
        float actualPos = Arm_Controller.jointPositions[jointIndex];

        // Somando / Subtraindo valor da posição
        if (deltas[jointIndex] > 0) actualPos += rotationRate;
        else actualPos -= rotationRate;

        // Definindo posição das juntas
        Arm_Controller.jointPositions[jointIndex] = actualPos;
        Arm_Controller.jointPositions[jointIndex] = Mathf.Clamp(Arm_Controller.jointPositions[jointIndex], Arm_Controller.minRotLimit, Arm_Controller.maxRotLimit);
        setJointPosition(Arm_Controller.servo2Unity(Arm_Controller.jointPositions[jointIndex]), Arm_Controller.rotationReferences, jointIndex, false);
        Arm_Controller.updateText();
    }

    private void InitDiffs()
    {
        // Calculando deltas entre posição atual e desejada
        for (int i = 0; i < 4; i++)
        {
            // Calculando diferenças iniciais
            diffs[i] = desiredJointPosition[i] - Arm_Controller.jointPositions[i];

            // Conferindo se iteração é ou não necessária
            if (diffs[i] != 0) iterationOver[i] = false;
            else iterationOver[i] = true;
        }

        // Registrando diferenças iniciais 
        initialDiffs = diffs.ToArray();
    }

    // Atualizado uma vez por frame
    void Update()
    {
        // Recuperando instância atual de gamepad
        Gamepad gamepad = Gamepad.current;

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && TabManager.selectedTabIndex == 0 && !SetMode)
        {
            // Recuperando posição desejada 
            for (int i = 0; i < inputFields.Count; i++)
            {
                if (inputFields[i].text != string.Empty)
                {
                    desiredJointPosition[i] = float.Parse(inputFields[i].text);
                    desiredJointPosition[i] = Mathf.Clamp(desiredJointPosition[i], 0.0f, 180.0f);
                    inputFields[i].text = string.Empty;
                    print("DesiredPos -> " + desiredJointPosition[0].ToString());
                }
                else desiredJointPosition[i] = Arm_Controller.jointPositions[i];
            }

            SetMode = true;
            print("Seting Position");
            InitDiffs();

        }
        else if ((Input.GetKeyDown(KeyCode.R) || (gamepad != null && gamepad.yButton.wasReleasedThisFrame)) && TabManager.selectedTabIndex == 0 && !SetMode)
        {
            desiredJointPosition = new float[] { 90.0F, 90.0F , 90.0F , 90.0F };
            SetMode = true;
            InitDiffs();
        }

        if (SetMode)
        {

            string auxString = "diffs -> ";
            for (int i = 0; i < diffs.Length; i++)
            {
                auxString += diffs[i].ToString() + " | ";
            }
            print(auxString);

            if (!iterationOver[0] || !iterationOver[1] || !iterationOver[2] || !iterationOver[3])
            {
                for (int i = 0; i < 4; i++)
                {
                    // Conferindo se ainda não se chegou na posição desejada 
                    if (!iterationOver[i])
                    {
                        // Somando subtraindo posição
                        SummSub(diffs, i);

                        // Recalculando delta
                        diffs[i] = desiredJointPosition[i] - Arm_Controller.jointPositions[i];

                        // Conferindo se sinal mudou
                        if ((Mathf.Sign(initialDiffs[i]) != Mathf.Sign(diffs[i])) || diffs[i] == 0)
                        {
                            // Informando fim de iteração 
                            iterationOver[i] = true;

                            // Clamplar angulo em valor de angulo desejado
                            setJointPosition(Arm_Controller.servo2Unity(desiredJointPosition[i]), Arm_Controller.rotationReferences, i, false);
                            Arm_Controller.jointPositions[i] = desiredJointPosition[i];

                        }
                    }
                }
            }
            else SetMode = false;
        }
    }
}
