using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TabManager : MonoBehaviour
{
    [SerializeField] public GameObject[] tabs;
    [SerializeField] public GameObject[] modelsToHide;
    [SerializeField] public bool hideObjects = true;
    [HideInInspector] public static int selectedTabIndex = 0; 
    public void onTabSwitch(GameObject tab)
    {
        tab.SetActive(true);
        Image image = tab.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0.0f;
        image.color = tempColor;

        // Atualizando aba selecionada
        selectedTabIndex = Array.IndexOf(tabs, tab);

        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i] != tab)
            {
                tabs[i].SetActive(false);
                image = tabs[i].GetComponent<Image>();
                PythonCommunication.StopConnection();
                Arm_Controller.UpdateIkPos();
                tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
            }
        }


        if (selectedTabIndex == 0)
        {
            if(hideObjects) foreach (GameObject armPart in modelsToHide) armPart.SetActive(true);
            Arm_Controller.SetServoColor(Arm_Controller.selectedJoint);
        }
        else if (selectedTabIndex == 1)
        {
            if(hideObjects) foreach (GameObject armPart in modelsToHide) armPart.SetActive(false);
            Arm_Controller.UncolorServos();
        }

    }

    private void Start()
    {
        onTabSwitch(tabs[selectedTabIndex]);
    }

    private void ProcessController()
    {
        // Recuperando instância atual de gamepad
        Gamepad gamepad = Gamepad.current;

        // Conferindo se há instâncias
        if (gamepad != null)
        {
            // Alterando posição das abas
            if (gamepad.rightShoulder.isPressed && selectedTabIndex < tabs.Length - 1) onTabSwitch(tabs[selectedTabIndex + 1]);
            else if (gamepad.leftShoulder.isPressed && selectedTabIndex > 0) onTabSwitch(tabs[selectedTabIndex - 1]);
        }
    }

    private void Update()
    {
        ProcessController();
    }
}