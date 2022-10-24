using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetEndEffectorPos : MonoBehaviour
{
    // Constantes
    private const int DECIMALS = 2;
    private const float ROTATION_RATE = 0.01F;

    [SerializeField] public List<TMP_InputField> inputFields;
    public static bool SetMode = false;
    private float[] desiredPosition = new float[3];
    private float[] diffs = new float[3];
    

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && TabManager.selectedTabIndex == 1 && !SetMode && PythonCommunication.isConnected)
        {
            // Recuperando valores dos campos 
            for (int i = 0; i < 3; i++)
            {
                // Conferindo se h� texto no campo
                if (inputFields[i].text != string.Empty)
                {
                    desiredPosition[i] = float.Parse(inputFields[i].text);
                    inputFields[i].text = string.Empty;
                }
                else
                {
                    // No caso de campo vazio passar posi��o atual
                    desiredPosition[i] = Arm_Controller.inverseKinematics.positions[i];
                }
            }

            // Permitindo entrar em modo de reposicionamento
            SetMode = true;
        }

        // Conferindo se modo de reposicionamento est� ativo
        if (SetMode)
        {
            // Calculando deltas entre posi��o atual e desejada\\
            for (int i = 0; i < 3; i++)
            {
                diffs[i] = desiredPosition[i] - Arm_Controller.inverseKinematics.positions[i];
                diffs[i] = (float)(Math.Round((double)diffs[i], DECIMALS));
            }

            // Conferindo se h� deltas com valor absoluto n�o nulo
            if ((diffs[0] != 0) || (diffs[1] != 0) || (diffs[2] != 0))
            {
                // Para cada eixo de refer�ncia
                for (int i = 0; i < 3; i++)
                {
                    // Conferindo se ainda n�o chegou na posi��o desejada 
                    if (diffs[i] != 0)
                    {
                        // Recuperar posi��o e arrendondar 
                        float actualPos = (float)(Math.Round((double)Arm_Controller.inverseKinematics.positions[i], DECIMALS));
                        print(actualPos);

                        // Somando / Subtraindo valor da posi��o 
                        if (diffs[i] > 0) actualPos += ROTATION_RATE;
                        else actualPos -= ROTATION_RATE;


                        // Definindo posi��o desejada do end effector
                        Arm_Controller.inverseKinematics.setPosition(actualPos, i);

                        // Atualizando texto exibido
                        Arm_Controller.updateText();

                        // Recalculando delta 
                        diffs[i] = desiredPosition[i] - Arm_Controller.inverseKinematics.positions[i];
                        diffs[i] = (float)(Math.Round((double)diffs[i], DECIMALS));
                    }
                }
            }
            else SetMode = false;
        }
    }
}
