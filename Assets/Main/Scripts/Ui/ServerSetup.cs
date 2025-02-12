using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ServerSetup : MonoBehaviour
{
    public TMPro.TMP_InputField portInputField; // Поле ввода номера порта
    public void SaveSettingsAndStart()
    {
        if (int.TryParse(portInputField.text, out int port))
        {
            PlayerPrefs.SetString("IsServer", "true");
            PlayerPrefs.SetInt("ServerPort", port);
            SceneManager.LoadScene("ProceduralGenTest"); // Перейти на сцену сервера
        }
        else
        {
            Debug.LogError("Incrorrect port type . Enter correct port format:"+portInputField.text);
            // Дополнительно можно уведомить пользователя через UI об ошибке ввода
        }
    }

    public void SetBlind()
    {
        PlayerPrefs.SetString("IsBlind", "true");
    }

    public void SetDeaf()
    {
        PlayerPrefs.SetString("IsBlind", "false");
    }


}
