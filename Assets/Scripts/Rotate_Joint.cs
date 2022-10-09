using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotate_Joint : MonoBehaviour
{
    #region Variaveis 
    /// <summary>
    /// Armazena velocidade de giro das juntas 
    /// </summary>
    public static float turnSpeed = 50f;

    /// <summary>
    /// Coleção de Sliders presente na UI 
    /// </summary>
    public static List<Slider> sliders = new List<Slider>();

    #endregion

    #region Metodos
    /// <summary>
    /// Desativa todos os sliders no momento em que a tecla é pressionada
    /// </summary>
    private void deactivateSliders() { foreach (Slider slider in sliders) { slider.gameObject.SetActive(false); } }

    #endregion

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            deactivateSliders();

        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            deactivateSliders();
        }
    }
}
