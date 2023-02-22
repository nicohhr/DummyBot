using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogBtn : MonoBehaviour
{
    [SerializeField] public Button button;
    public static Button btn;
    private static Color lastColor = new Color(0.1254902F, 0.3098039F, 0.4588236F, 0.3411765F);
    private static Color greenColor = new Color(0.2834483F, 0.8113208F, 0.2347217F, 0.5F);

    public void PressButton()
    {
        if (!LogData.hasStarted)
        {
            try
            {
                // Inicializando log de dados
                LogData.StartProcess();
                //ColorBlock cb = button.colors;
                Debug.Log(Application.persistentDataPath);

                // Alterando cor
                this.button.image.color = LogBtn.greenColor;
            }
            catch (IOException)
            {
                Debug.Log("Problema inicializando thread");
                LogBtn.TurnWhite();
            }
        }
        else
        {
            // Declarando finalização 
            LogData.Stop();

            // Alterando cor 
            this.button.image.color = lastColor;
        }
    }

    public static void TurnWhite()
    {
        LogBtn.btn.image.color = lastColor;
    }

    public static void TurnGreen()
    {
        LogBtn.btn.image.color = greenColor;
    }

    public static void MakeNonIteracteble()
    {
        LogBtn.btn.interactable = false;
        TurnWhite();
    }

    public static void MakeInteracteble()
    {
        LogBtn.btn.interactable = true;
    }

    private void Start()
    {
        // Passando botão para variável estática
        LogBtn.btn = this.button;
    }

    private void Update()
    {
        if (LogData.hasStarted)
        {
            LogBtn.TurnGreen();
        }
        else
        {
            LogBtn.TurnWhite();
        }
    }
}
