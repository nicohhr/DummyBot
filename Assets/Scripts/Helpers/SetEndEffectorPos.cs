using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetEndEffectorPos : MonoBehaviour
{
    // Constantes
    private static float turnRate = 0.035F;

    [SerializeField] public List<TMP_InputField> inputFields;
    [HideInInspector]public static float[] initialPosition = new float[3];
    public static bool SetMode = false;
    private float[] initialDiffs = new float[3];
    private bool[] iterationOver = new bool[3];
    private float[] desiredPosition = new float[3];
    private float[] diffs = new float[3];

    private void InitDiffs()
    {
        // Calculando deltas entre posição atual e desejada\\
        for (int i = 0; i < 3; i++)
        {
            // Calculando diferenças iniciais
            diffs[i] = desiredPosition[i] - Arm_Controller.inverseKinematics.positions[i];

            // Conferindo se iteração é ou não necessária
            if (diffs[i] != 0) iterationOver[i] = false;
            else iterationOver[i] = true;
        }

        // Registrando diferenças iniciais 
        initialDiffs = diffs.ToArray();
    }
    

    // Update is called once per frame
    void Update()
    {
        // Recuperando instância atual de gamepad
        Gamepad gamepad = Gamepad.current;

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && TabManager.selectedTabIndex == 1 && !SetMode && PythonCommunication.isConnected)
        {
            // Recuperando valores dos campos 
            for (int i = 0; i < 3; i++)
            {
                // Conferindo se há texto no campo
                if (inputFields[i].text != string.Empty)
                {
                    desiredPosition[i] = float.Parse(inputFields[i].text);
                    inputFields[i].text = string.Empty;
                }
                else
                {
                    // No caso de campo vazio passar posição atual
                    desiredPosition[i] = Arm_Controller.inverseKinematics.positions[i];
                }
            }

            // Permitindo entrar em modo de reposicionamento
            SetMode = true;
            InitDiffs();
            print("Seting Position");

        } else if ((Input.GetKeyDown(KeyCode.R) || (gamepad != null && gamepad.yButton.wasReleasedThisFrame)) && TabManager.selectedTabIndex == 1 && !SetMode && PythonCommunication.isConnected)
        {
            desiredPosition = initialPosition.ToArray();
            SetMode = true;
            InitDiffs();
            print("Reseting Position");
        }

        // Conferindo se modo de reposicionamento está ativo
        if (SetMode)
        {
            // Conferindo se há deltas com valor absoluto não nulo
            if (!iterationOver[0] || !iterationOver[1] || !iterationOver[2])
            {
                // Para cada eixo de referência
                for (int i = 0; i < 3; i++)
                {
                    // Conferindo se ainda não chegou na posição desejada 
                    if (!iterationOver[i])
                    {
                        // Recuperar posição e arrendondar 
                        float actualPos = Arm_Controller.inverseKinematics.positions[i];
                        print(actualPos);

                        // Somando / Subtraindo valor da posição 
                        if (diffs[i] >= 0) actualPos += turnRate;
                        else actualPos -= turnRate;

                        // Definindo posição desejada do end effector
                        Arm_Controller.inverseKinematics.setPosition(actualPos, i);

                        // Atualizando texto exibido
                        Arm_Controller.updateText();

                        // Recalculando delta 
                        diffs[i] = desiredPosition[i] - Arm_Controller.inverseKinematics.positions[i];

                        // Conferindo se sinal mudou
                        if ((Mathf.Sign(initialDiffs[i]) != Mathf.Sign(diffs[i])) || diffs[i] == 0)
                        {
                            // Informando fim de iteração 
                            iterationOver[i] = true;

                            // Clamplar angulo em valor de angulo desejado
                            Arm_Controller.inverseKinematics.setPosition(desiredPosition[i], i);

                        }
                    }
                }
            }
            else SetMode = false;
        }
    }
}
