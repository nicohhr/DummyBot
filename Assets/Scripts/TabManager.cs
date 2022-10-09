using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] public GameObject[] tabs;
    public void onTabSwitch(GameObject tab)
    {
        tab.SetActive(true);
        Image image = tab.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0.0f;
        image.color = tempColor;

        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i] != tab)
            {
                tabs[i].SetActive(false);
                image = tabs[i].GetComponent<Image>();
                tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
            }
        }
    }

    private void Start()
    {
        onTabSwitch(tabs[0]);
    }
}