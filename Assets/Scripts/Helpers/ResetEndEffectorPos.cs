using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.ContentSizeFitter;

public class ResetEndEffectorPos : MonoBehaviour
{
    // Constantes
    private const int DECIMALS = 2;
    private const float ROTATION_RATE = 0.01F;

    // Variáveis
    public static bool SetMode = false;
    public static float[] desiredPosition = new float[3];
    private float[] diffs = new float[3];


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && TabManager.selectedTabIndex == 1 && !SetMode && !SetEndEffectorPos.SetMode && PythonCommunication.isConnected)
        {
            // Permitindo entrar em modo de reposicionamento
            SetMode = true;
            print("Reseting Position");
        }

        // Conferindo se modo de reposicionamento está ativo
        if (SetMode)
        {

            // Calculando deltas entre posição atual e desejada\\
            for (int i = 0; i < 3; i++)
            {
                diffs[i] = desiredPosition[i] - Arm_Controller.inverseKinematics.positions[i];
                diffs[i] = (float)(Math.Round((double)diffs[i], DECIMALS));
            }

            // Conferindo se há deltas com valor absoluto não nulo
            if ((diffs[0] != 0) || (diffs[1] != 0) || (diffs[2] != 0))
            {
                // Para cada eixo de referência
                for (int i = 0; i < 3; i++)
                {
                    // Conferindo se ainda não chegou na posição desejada 
                    if (diffs[i] != 0)
                    {
                        // Recuperar posição e arrendondar 
                        float actualPos = (float)(Math.Round((double)Arm_Controller.inverseKinematics.positions[i], DECIMALS));
                        print(actualPos);

                        // Somando / Subtraindo valor da posição 
                        if (diffs[i] > 0) actualPos += ROTATION_RATE;
                        else actualPos -= ROTATION_RATE;


                        // Definindo posição desejada do end effector
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
