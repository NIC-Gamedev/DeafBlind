using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Unity.Collections;
public class ServerManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private Tugboat _tugboat;

 

    private void OnLevelWasLoaded(int level)
    {
        _networkManager = GetComponent<NetworkManager>();
        _tugboat = GetComponent<Tugboat>();
        string isServer = PlayerPrefs.GetString("IsServer");

        if (Convert.ToBoolean(isServer))
        {


            Debug.Log("Server port is set: " + PlayerPrefs.GetInt("ServerPort", 80));
            // Получаем порт из PlayerPrefs
            int port = PlayerPrefs.GetInt("ServerPort", 80); // По умолчанию порт 80


            _tugboat.SetPort((ushort)port);
            // Запуск сервера
            _networkManager.ServerManager.StartConnection();

            // Опционально: запустить клиент (хост)
            _networkManager.ClientManager.StartConnection();
        }
        else
        {
            string serverip = PlayerPrefs.GetString("ServerIp");

            string[] parts = serverip.Split(':');

            string ip = parts.Length > 0 ? parts[0] : "";
            ushort port = Convert.ToUInt16(parts.Length > 1 ? parts[1] : "");
            
            _networkManager.ClientManager.StartConnection(ip, port);
        }
    }
}
