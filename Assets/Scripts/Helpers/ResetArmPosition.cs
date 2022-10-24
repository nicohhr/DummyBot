using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResetArmPosition : MonoBehaviour
{
    enum AxisSelection { X, Y, Z }

    // Coleção de juntas do manipulador
    public List<Transform> joints;

    public static bool RestartMode = false;
    private float[] desiredJointPosition = { 90.0F, 90.0F , 90.0F , 90.0F };
    private float[] diffs = new float[4];
    private float rotationRate = 0.1F;
    private int decimals = 1;

    private void setJointPosition(float angularPos, List<object> axisSelection, int index, bool updatePositions = false)
    {
        // Atualizando posições se necessário
        if (updatePositions) { Arm_Controller.jointPositions[index] = Arm_Controller.unity2Servo(angularPos); }

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
        // Recuperar posição e arredondar para duas casas decimais
        float actualPos = (float)(Math.Round((double)Arm_Controller.jointPositions[jointIndex], decimals));

        // Somando / Subtraindo valor da posição
        if (deltas[jointIndex] > 0) actualPos += rotationRate;
        else actualPos -= rotationRate;

        // Definindo posição das juntas
        Arm_Controller.jointPositions[jointIndex] = actualPos;
        Arm_Controller.jointPositions[jointIndex] = Mathf.Clamp(Arm_Controller.jointPositions[jointIndex], Arm_Controller.minRotLimit, Arm_Controller.maxRotLimit);
        setJointPosition(Arm_Controller.servo2Unity(Arm_Controller.jointPositions[jointIndex]), Arm_Controller.rotationReferences, jointIndex, false);
        Arm_Controller.updateText();
    }

    // Atualizado uma vez por frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R) && !RestartMode && !SetArmPosition.SetMode && TabManager.selectedTabIndex == 0)
        {
            RestartMode = true;
            print("Seting Position");
        }

        if (RestartMode)
        {
            // Calculando deltas entre posição atual e desejada
            for (int i = 0; i < 4; i++)
            {
                diffs[i] = desiredJointPosition[i] - Arm_Controller.jointPositions[i];
                diffs[i] = (float)(Math.Round((double)diffs[i], decimals));
            }

            if ((diffs[0] != 0) || (diffs[1] != 0) || (diffs[2] != 0) || (diffs[3] != 0))
            {
                for (int i = 0; i < 4; i++)
                {
                    // Conferindo se ainda não se chegou na posição desejada 
                    if (diffs[i] != 0)
                    {
                        // Somando subtraindo posição
                        SummSub(diffs, i);

                        // Recalculando delta
                        diffs[i] = desiredJointPosition[i] - Arm_Controller.jointPositions[i];
                        diffs[i] = (float)(Math.Round((double)diffs[i], decimals));
                    }
                }
            } else
            {
                RestartMode = false; 
            }
        }
    }
}
