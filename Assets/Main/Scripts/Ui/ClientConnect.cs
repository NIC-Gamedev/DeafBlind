using System.Net.Sockets;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet;
public class ClientConnect : MonoBehaviour
{
    public TMPro.TMP_InputField ipadressInputfield; // Поле ввода номера порта

    public void SaveSettingsAndStart()
    {
        PlayerPrefs.SetString("IsServer", "false");

      

        PlayerPrefs.SetString("ServerIp", ipadressInputfield.text);
        SceneManager.LoadScene("ProceduralGenTest"); // Перейти на сцену сервера
    }

  
}
