using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] public GameObject[] tabs;
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
    }

    private void Start()
    {
        onTabSwitch(tabs[selectedTabIndex]);
    }
}