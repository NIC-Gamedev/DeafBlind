using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSetup : MonoBehaviour
{
    public TMPro.TMP_InputField portInputField; // Поле ввода номера порта

    public void SaveSettingsAndStart()
    {
        if (int.TryParse(portInputField.text, out int port))
        {
            PlayerPrefs.SetInt("ProceduralGenTest", port);
            SceneManager.LoadScene("ProceduralGenTest"); // Перейти на сцену сервера
        }
        else
        {
            Debug.LogError("Некорректный формат порта. Введите целое число."+portInputField.text);
            // Дополнительно можно уведомить пользователя через UI об ошибке ввода
        }
    }
}
